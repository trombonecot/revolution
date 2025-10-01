using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    [Header("Combat Settings")]
    public int maxRounds = 5;
    public int currentRound = 0;

    [Header("Teams")]
    public List<Character> playerTeam = new List<Character>();
    public List<Character> enemyTeam = new List<Character>();

    [Header("Deck")]
    public List<Card> playerDeck = new List<Card>();
    public List<Card> playerHand = new List<Card>();
    public int handSize = 10;

    [Header("Combat State")]
    public bool isPlayerTurn = true;
    public bool waitingForCardSelection = false;
    public bool waitingForAbilityActivation = false;

    [Header("Events")]
    public UnityEvent<string> OnCombatLog;
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent<bool> OnCombatEnd; // true = player wins

    // Combos
    private Dictionary<string, int> playerComboCount = new Dictionary<string, int>();
    private Dictionary<string, int> enemyComboCount = new Dictionary<string, int>();

    void Start()
    {
        InitializeCombat();
    }

    void InitializeCombat()
    {
        // Determinar HP segons número de personatges
        int teamSize = playerTeam.Count;
        int baseHP = GetBaseHP(teamSize);

        foreach (var character in playerTeam)
        {
            character.maxHP = baseHP;
            character.currentHP = baseHP;
            character.isAlive = true;
            character.abilityUsed = false;
        }

        foreach (var character in enemyTeam)
        {
            character.maxHP = baseHP;
            character.currentHP = baseHP;
            character.isAlive = true;
            character.abilityUsed = false;
        }

        // Barrejar i repartir mà inicial
        ShuffleDeck();
        DrawInitialHand();

        StartCoroutine(CombatLoop());
    }

    int GetBaseHP(int teamSize)
    {
        switch (teamSize)
        {
            case 1: return 10;
            case 2: return 8;
            case 3: return 7;
            case 4: return 6;
            default: return 10;
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            Card temp = playerDeck[i];
            int randomIndex = Random.Range(i, playerDeck.Count);
            playerDeck[i] = playerDeck[randomIndex];
            playerDeck[randomIndex] = temp;
        }
    }

    void DrawInitialHand()
    {
        playerHand.Clear();
        for (int i = 0; i < Mathf.Min(handSize, playerDeck.Count); i++)
        {
            playerHand.Add(playerDeck[i]);
        }
    }

    IEnumerator CombatLoop()
    {
        while (currentRound < maxRounds)
        {
            currentRound++;
            OnRoundStart?.Invoke();
            LogMessage($"--- Ronda {currentRound} ---");

            // Fase 1: Selecció de cartes
            yield return StartCoroutine(CardSelectionPhase());

            // Fase 2: Activació d'habilitats
            yield return StartCoroutine(AbilityPhase());

            // Fase 3: Resolució per iniciativa
            yield return StartCoroutine(InitiativeResolution());

            // Fase 4: Combat carta vs carta
            yield return StartCoroutine(CardCombatPhase());

            // Fase 5: Actualització d'estats
            UpdateStatusEffects();

            // Comprovar condició de victòria
            if (CheckVictoryCondition(out bool playerWins))
            {
                EndCombat(playerWins);
                yield break;
            }

            OnRoundEnd?.Invoke();
            yield return new WaitForSeconds(1f);
        }

        // Final per temps: guanya qui té més HP
        EndCombatByHP();
    }

    IEnumerator CardSelectionPhase()
    {
        LogMessage("Selecciona cartes per als teus personatges...");

        // Player selecciona cartes
        waitingForCardSelection = true;
        yield return new WaitUntil(() => !waitingForCardSelection);

        // IA selecciona cartes
        AISelectCards();

        yield return new WaitForSeconds(0.5f);
    }

    void AISelectCards()
    {
        foreach (var enemy in enemyTeam.Where(c => c.isAlive))
        {
            // IA simple: tria carta random del deck
            Card randomCard = playerDeck[Random.Range(0, playerDeck.Count)];
            enemy.assignedCard = randomCard;
        }
    }

    IEnumerator AbilityPhase()
    {
        LogMessage("Fase d'activació d'habilitats...");

        // Player pot activar habilitats
        waitingForAbilityActivation = true;
        yield return new WaitUntil(() => !waitingForAbilityActivation);

        // IA activa habilitats (50% probabilitat)
        foreach (var enemy in enemyTeam.Where(c => c.isAlive && !c.abilityUsed && c.canUseAbility))
        {
            if (Random.value > 0.5f && enemy.uniqueAbility != null)
            {
                Character target = playerTeam.FirstOrDefault(c => c.isAlive);
                enemy.uniqueAbility.Activate(enemy, target);
                LogMessage($"{enemy.characterName} usa {enemy.uniqueAbility.abilityName}!");
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator InitiativeResolution()
    {
        // Ordenar tots els personatges vius per velocitat
        List<Character> allCharacters = new List<Character>();
        allCharacters.AddRange(playerTeam.Where(c => c.isAlive));
        allCharacters.AddRange(enemyTeam.Where(c => c.isAlive));

        allCharacters = allCharacters.OrderByDescending(c => c.speed).ToList();

        LogMessage("Ordre d'iniciativa: " + string.Join(", ", allCharacters.Select(c => c.characterName)));

        yield return new WaitForSeconds(1f);
    }

    IEnumerator CardCombatPhase()
    {
        LogMessage("Resolent combats...");

        // Combat 1v1 entre personatges vius
        var alivePlayers = playerTeam.Where(c => c.isAlive).ToList();
        var aliveEnemies = enemyTeam.Where(c => c.isAlive).ToList();

        int combatCount = Mathf.Min(alivePlayers.Count, aliveEnemies.Count);

        for (int i = 0; i < combatCount; i++)
        {
            yield return StartCoroutine(ResolveCombat(alivePlayers[i], aliveEnemies[i]));
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ResolveCombat(Character player, Character enemy)
    {
        LogMessage($"\n{player.characterName} vs {enemy.characterName}");

        // Player ataca
        int playerAttack = player.GetTotalAttack();
        int enemyDefense = enemy.GetTotalDefense();
        int damageToEnemy = Mathf.Max(0, playerAttack - enemyDefense);

        enemy.TakeDamage(damageToEnemy);
        LogMessage($"{player.characterName} fa {damageToEnemy} dany! ({enemy.characterName}: {enemy.currentHP}/{enemy.maxHP} HP)");

        // Aplicar efecte d'estat de la carta del player
        if (player.assignedCard != null && player.assignedCard.statusEffect != StatusEffect.None)
        {
            enemy.ApplyStatus(player.assignedCard.statusEffect);
            LogMessage($"{enemy.characterName} pateix {player.assignedCard.statusEffect}!");
        }

        yield return new WaitForSeconds(0.5f);

        // Enemy contraataca (si encara viu)
        if (enemy.isAlive)
        {
            int enemyAttack = enemy.GetTotalAttack();
            int playerDefense = player.GetTotalDefense();
            int damageToPlayer = Mathf.Max(0, enemyAttack - playerDefense);

            player.TakeDamage(damageToPlayer);
            LogMessage($"{enemy.characterName} fa {damageToPlayer} dany! ({player.characterName}: {player.currentHP}/{player.maxHP} HP)");

            // Aplicar efecte d'estat de la carta de l'enemic
            if (enemy.assignedCard != null && enemy.assignedCard.statusEffect != StatusEffect.None)
            {
                player.ApplyStatus(enemy.assignedCard.statusEffect);
                LogMessage($"{player.characterName} pateix {enemy.assignedCard.statusEffect}!");
            }
        }

        // Comprovar combos
        CheckCombo(player, true);
        CheckCombo(enemy, false);
    }

    void CheckCombo(Character character, bool isPlayer)
    {
        if (character.assignedCard == null) return;

        string comboKey = character.assignedCard.cardType.ToString();
        var comboDict = isPlayer ? playerComboCount : enemyComboCount;

        if (!comboDict.ContainsKey(comboKey))
            comboDict[comboKey] = 0;

        comboDict[comboKey]++;

        if (comboDict[comboKey] >= 2)
        {
            character.tempAttackMod += 1;
            LogMessage($"COMBO! {character.characterName} guanya +1 Atac!");
            comboDict[comboKey] = 0; // Reset combo
        }
    }

    void UpdateStatusEffects()
    {
        foreach (var character in playerTeam.Concat(enemyTeam))
        {
            if (character.isAlive)
            {
                character.UpdateStatus();
            }
        }
    }

    bool CheckVictoryCondition(out bool playerWins)
    {
        bool allPlayersDead = playerTeam.All(c => !c.isAlive);
        bool allEnemiesDead = enemyTeam.All(c => !c.isAlive);

        if (allPlayersDead)
        {
            playerWins = false;
            return true;
        }

        if (allEnemiesDead)
        {
            playerWins = true;
            return true;
        }

        playerWins = false;
        return false;
    }

    void EndCombatByHP()
    {
        int playerTotalHP = playerTeam.Sum(c => c.currentHP);
        int enemyTotalHP = enemyTeam.Sum(c => c.currentHP);

        bool playerWins = playerTotalHP > enemyTotalHP;

        LogMessage($"\nCombat finalitzat! HP totals: Player {playerTotalHP} vs Enemy {enemyTotalHP}");
        EndCombat(playerWins);
    }

    void EndCombat(bool playerWins)
    {
        LogMessage(playerWins ? "\n🎉 VICTÒRIA! 🎉" : "\n💀 DERROTA 💀");
        OnCombatEnd?.Invoke(playerWins);
    }

    void LogMessage(string message)
    {
        Debug.Log(message);
        OnCombatLog?.Invoke(message);
    }

    // Mètodes públics per UI
    public void AssignCardToCharacter(Character character, Card card)
    {
        character.assignedCard = card;
        playerHand.Remove(card);

        // Comprovar si s'han assignat cartes a tots els personatges vius
        if (playerTeam.Where(c => c.isAlive).All(c => c.assignedCard != null))
        {
            waitingForCardSelection = false;
        }
    }

    public void ActivateCharacterAbility(Character character, Character target = null)
    {
        if (!character.abilityUsed && character.canUseAbility && character.uniqueAbility != null)
        {
            character.uniqueAbility.Activate(character, target);
            LogMessage($"{character.characterName} usa {character.uniqueAbility.abilityName}!");
        }
    }

    public void FinishAbilityPhase()
    {
        waitingForAbilityActivation = false;
    }
}