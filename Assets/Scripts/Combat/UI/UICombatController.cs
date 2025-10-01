using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICombatController : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;

    [Header("Character UI")]
    public Transform playerCharactersContainer;
    public Transform enemyCharactersContainer;
    public GameObject characterDisplayPrefab;

    [Header("Hand UI")]
    public Transform handContainer;
    public GameObject cardDisplayPrefab;

    [Header("Buttons")]
    public Button confirmCardSelectionButton;
    public Button skipAbilityPhaseButton;
    public Button endTurnButton;

    [Header("Info UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI instructionText;
    public ScrollRect combatLogScroll;
    public TextMeshProUGUI combatLogText;

    [Header("Panels")]
    public GameObject abilitySelectionPanel;
    public Transform abilityButtonsContainer;
    public GameObject abilityButtonPrefab;

    [Header("Victory/Defeat")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private List<UICharacterDisplay> playerCharacterDisplays = new List<UICharacterDisplay>();
    private List<UICharacterDisplay> enemyCharacterDisplays = new List<UICharacterDisplay>();
    private List<UICardDisplay> handCardDisplays = new List<UICardDisplay>();

    private UICharacterDisplay selectedCharacter;
    private UICardDisplay selectedCard;

    private enum CombatPhase
    {
        CardSelection,
        AbilityActivation,
        Combat,
        WaitingForNextRound
    }

    private CombatPhase currentPhase;

    void Start()
    {
        // Connectar events del CombatManager
        combatManager.OnCombatLog.AddListener(AddCombatLog);
        combatManager.OnRoundStart.AddListener(OnRoundStart);
        combatManager.OnRoundEnd.AddListener(OnRoundEnd);
        combatManager.OnCombatEnd.AddListener(OnCombatEnd);

        // Configurar botons
        confirmCardSelectionButton.onClick.AddListener(ConfirmCardSelection);
        skipAbilityPhaseButton.onClick.AddListener(SkipAbilityPhase);

        // Crear displays
        CreateCharacterDisplays();
        CreateHandDisplays();

        // Panells inicials
        abilitySelectionPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    void CreateCharacterDisplays()
    {
        // Crear displays per personatges del jugador
        foreach (var character in combatManager.playerTeam)
        {
            GameObject go = Instantiate(characterDisplayPrefab, playerCharactersContainer);
            UICharacterDisplay display = go.GetComponent<UICharacterDisplay>();
            display.SetCharacter(character);
            display.OnCharacterClicked = OnCharacterClicked;
            playerCharacterDisplays.Add(display);
        }

        // Crear displays per enemics
        foreach (var character in combatManager.enemyTeam)
        {
            GameObject go = Instantiate(characterDisplayPrefab, enemyCharactersContainer);
            UICharacterDisplay display = go.GetComponent<UICharacterDisplay>();
            display.SetCharacter(character);
            display.OnCharacterClicked = OnEnemyCharacterClicked;
            enemyCharacterDisplays.Add(display);
        }
    }

    void CreateHandDisplays()
    {
        foreach (var card in combatManager.playerHand)
        {
            GameObject go = Instantiate(cardDisplayPrefab, handContainer);
            UICardDisplay display = go.GetComponent<UICardDisplay>();
            display.SetCard(card);
            display.OnCardClicked = OnCardClicked;
            handCardDisplays.Add(display);
        }
    }

    void OnRoundStart()
    {
        roundText.text = $"Ronda {combatManager.currentRound}/{combatManager.maxRounds}";
        currentPhase = CombatPhase.CardSelection;
        UpdatePhaseUI();

        // Reset seleccions
        selectedCharacter = null;
        selectedCard = null;

        // Reset cartes assignades visuals
        foreach (var display in playerCharacterDisplays)
        {
            display.characterData.assignedCard = null;
            display.UpdateDisplay();
        }
    }

    void OnRoundEnd()
    {
        UpdateAllDisplays();
    }

    void UpdatePhaseUI()
    {
        switch (currentPhase)
        {
            case CombatPhase.CardSelection:
                phaseText.text = "Fase: Selecció de Cartes";
                instructionText.text = "Selecciona un personatge i després una carta per assignar-la";
                confirmCardSelectionButton.gameObject.SetActive(true);
                skipAbilityPhaseButton.gameObject.SetActive(false);
                break;

            case CombatPhase.AbilityActivation:
                phaseText.text = "Fase: Activació d'Habilitats";
                instructionText.text = "Selecciona un personatge per activar la seva habilitat (opcional)";
                confirmCardSelectionButton.gameObject.SetActive(false);
                skipAbilityPhaseButton.gameObject.SetActive(true);
                break;

            case CombatPhase.Combat:
                phaseText.text = "Fase: Combat";
                instructionText.text = "Resolent combats...";
                confirmCardSelectionButton.gameObject.SetActive(false);
                skipAbilityPhaseButton.gameObject.SetActive(false);
                break;

            case CombatPhase.WaitingForNextRound:
                phaseText.text = "Ronda completada";
                instructionText.text = "Preparant següent ronda...";
                confirmCardSelectionButton.gameObject.SetActive(false);
                skipAbilityPhaseButton.gameObject.SetActive(false);
                break;
        }
    }

    void OnCharacterClicked(UICharacterDisplay display)
    {
        if (currentPhase == CombatPhase.CardSelection)
        {
            // Deseleccionar anterior
            if (selectedCharacter != null)
                selectedCharacter.SetSelected(false);

            // Seleccionar nou
            selectedCharacter = display;
            selectedCharacter.SetSelected(true);

            // Si ja tenim carta seleccionada, assignar-la
            if (selectedCard != null)
            {
                AssignCardToCharacter();
            }
        }
        else if (currentPhase == CombatPhase.AbilityActivation)
        {
            ShowAbilityPanel(display);
        }
    }

    void OnEnemyCharacterClicked(UICharacterDisplay display)
    {
        // Només es fa servir quan s'activa una habilitat que necessita target
    }

    void OnCardClicked(UICardDisplay display)
    {
        if (currentPhase != CombatPhase.CardSelection) return;

        // Deseleccionar carta anterior
        if (selectedCard != null)
            selectedCard.SetSelected(false);

        // Seleccionar nova carta
        selectedCard = display;
        selectedCard.SetSelected(true);

        // Si ja tenim personatge seleccionat, assignar
        if (selectedCharacter != null)
        {
            AssignCardToCharacter();
        }
    }

    void AssignCardToCharacter()
    {
        if (selectedCharacter != null && selectedCard != null)
        {
            // Assignar carta al personatge
            combatManager.AssignCardToCharacter(selectedCharacter.characterData, selectedCard.cardData);

            // Actualitzar visuals
            selectedCharacter.UpdateDisplay();

            // Eliminar carta de la mà visual
            handCardDisplays.Remove(selectedCard);
            Destroy(selectedCard.gameObject);

            // Reset seleccions
            selectedCharacter.SetSelected(false);
            selectedCharacter = null;
            selectedCard = null;

            // Comprovar si s'han assignat totes les cartes
            CheckAllCardsAssigned();
        }
    }

    void CheckAllCardsAssigned()
    {
        bool allAssigned = combatManager.playerTeam
            .Where(c => c.isAlive)
            .All(c => c.assignedCard != null);

        confirmCardSelectionButton.interactable = allAssigned;
    }

    void ConfirmCardSelection()
    {
        combatManager.waitingForCardSelection = false;
        currentPhase = CombatPhase.AbilityActivation;
        UpdatePhaseUI();
    }

    void ShowAbilityPanel(UICharacterDisplay display)
    {
        if (display.characterData.abilityUsed || !display.characterData.canUseAbility)
        {
            AddCombatLog("Aquesta habilitat ja s'ha utilitzat o no està disponible!");
            return;
        }

        abilitySelectionPanel.SetActive(true);

        // Crear botó d'habilitat
        foreach (Transform child in abilityButtonsContainer)
            Destroy(child.gameObject);

        if (display.characterData.uniqueAbility != null)
        {
            GameObject btnGo = Instantiate(abilityButtonPrefab, abilityButtonsContainer);
            Button btn = btnGo.GetComponent<Button>();
            TextMeshProUGUI btnText = btnGo.GetComponentInChildren<TextMeshProUGUI>();

            btnText.text = display.characterData.uniqueAbility.abilityName;

            btn.onClick.AddListener(() => {
                // Determinar si necessita target
                Character target = null;
                if (display.characterData.uniqueAbility.abilityType == AbilityType.StatusInflict)
                {
                    target = combatManager.enemyTeam.FirstOrDefault(c => c.isAlive);
                }

                combatManager.ActivateCharacterAbility(display.characterData, target);
                display.UpdateDisplay();
                abilitySelectionPanel.SetActive(false);
            });
        }
    }

    void SkipAbilityPhase()
    {
        abilitySelectionPanel.SetActive(false);
        combatManager.FinishAbilityPhase();
        currentPhase = CombatPhase.Combat;
        UpdatePhaseUI();
    }

    void UpdateAllDisplays()
    {
        foreach (var display in playerCharacterDisplays)
            display.UpdateDisplay();

        foreach (var display in enemyCharacterDisplays)
            display.UpdateDisplay();
    }

    void AddCombatLog(string message)
    {
        combatLogText.text += message + "\n";

        // Auto-scroll al final
        Canvas.ForceUpdateCanvases();
        combatLogScroll.verticalNormalizedPosition = 0f;
    }

    void OnCombatEnd(bool playerWins)
    {
        if (playerWins)
            victoryPanel.SetActive(true);
        else
            defeatPanel.SetActive(true);
    }
}