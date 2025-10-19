using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HexCardGameManager : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public CardSelectionUI cardSelectionUI;

    [Header("Enemy Settings")]
    public int enemyLevel = 1;
    public int enemyMaxHP = 20;
    private int enemyCurrentHP;
    private int enemyCurrentAttack;
    private int enemyCurrentDefense;

    [Header("Game State")]
    private int currentRound = 1;
    private const int maxRounds = 5;
    private HexCard selectedCard;
    private bool combatStarted = false;

    [Header("UI References")]
    public Transform cardHandParent;
    public GameObject cardUIPrefab;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI enemyStatsText;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button nextTurnButton;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    void Start()
    {
        resultPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        nextTurnButton.onClick.AddListener(OnNextTurnClicked);
        restartButton.onClick.AddListener(RestartGame);

        // Show card selection panel at the start
        ShowCardSelection();
    }

    void ShowCardSelection()
    {
        cardSelectionUI.ShowSelectionPanel(player, OnCardsSelected);
    }

    void OnCardsSelected(List<HexCard> selectedCards)
    {
        player.SetSelectedCards(selectedCards);
        combatStarted = true;
        enemyCurrentHP = enemyMaxHP;
        StartNewRound();
    }

    void StartNewRound()
    {
        if (currentRound > maxRounds)
        {
            // Determine winner by HP comparison
            CheckPartialWin();
            return;
        }

        // Generate random enemy stats based on level
        enemyCurrentAttack = Random.Range(1, enemyLevel + 1);
        enemyCurrentDefense = Random.Range(1, enemyLevel + 1);

        UpdateUI();
        DisplayAvailableCards();
    }

    void CheckPartialWin()
    {
        if (player.CurrentHP > enemyCurrentHP)
        {
            EndGame(true, true); // Player partial win
        }
        else if (enemyCurrentHP > player.CurrentHP)
        {
            EndGame(false, true); // Enemy partial win
        }
        else
        {
            // Tie - you can decide how to handle this (e.g., player wins on tie)
            EndGame(true, true); // Player wins on tie
        }
    }

    void DisplayAvailableCards()
    {
        // Clear existing cards
        foreach (Transform child in cardHandParent)
        {
            Destroy(child.gameObject);
        }

        // Display available cards from player's selected deck
        List<HexCard> availableCards = player.GetAvailableCards();
        foreach (HexCard card in availableCards)
        {
            GameObject cardUI = Instantiate(cardUIPrefab, cardHandParent);
            CardUIComponent cardComponent = cardUI.GetComponent<CardUIComponent>();
            cardComponent.Initialize(card, this);
        }
    }

    public void OnCardSelected(HexCard card)
    {
        selectedCard = card;
        player.UseCard(card);

        // Calculate combat results
        int damageToEnemy = Mathf.Max(0, selectedCard.attack - enemyCurrentDefense);
        int damageToPlayer = Mathf.Max(0, enemyCurrentAttack - selectedCard.defense);

        enemyCurrentHP -= damageToEnemy;
        player.CurrentHP -= damageToPlayer;

        // Clamp HP values
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP);

        UpdateUI();
        ShowResult(damageToEnemy, damageToPlayer);

        // Clear card hand
        foreach (Transform child in cardHandParent)
        {
            Destroy(child.gameObject);
        }

        // Check win/loss conditions
        if (player.CurrentHP <= 0)
        {
            EndGame(false, false);
        }
        else if (enemyCurrentHP <= 0)
        {
            EndGame(true, false);
        }
    }

    void ShowResult(int damageToEnemy, int damageToPlayer)
    {
        resultPanel.SetActive(true);
        resultText.text = $"<b>Round {currentRound} Results</b>\n\n" +
                         $"Your card: {selectedCard.cardName} (ATK:{selectedCard.attack} DEF:{selectedCard.defense})\n" +
                         $"Enemy stats: ATK:{enemyCurrentAttack} DEF:{enemyCurrentDefense}\n\n" +
                         $"Damage to Enemy: {damageToEnemy}\n" +
                         $"Damage to You: {damageToPlayer}";
    }

    void OnNextTurnClicked()
    {
        resultPanel.SetActive(false);
        currentRound++;
        StartNewRound();
    }

    void UpdateUI()
    {
        playerHPText.text = $"Player HP: {player.CurrentHP}/{player.maxHP}";
        enemyHPText.text = $"Enemy HP: {enemyCurrentHP}/{enemyMaxHP}";
        roundText.text = $"Round: {currentRound}/{maxRounds}";
        enemyStatsText.text = $"Enemy ATK: {enemyCurrentAttack} | DEF: {enemyCurrentDefense}";
    }

    void EndGame(bool playerWon, bool isPartial)
    {
        gameOverPanel.SetActive(true);
        resultPanel.SetActive(false);

        if (isPartial)
        {
            if (playerWon)
            {
                gameOverText.text = $"PARTIAL VICTORY!\n\nYou survived all 5 rounds!\n\nFinal HP: You {player.CurrentHP} - Enemy {enemyCurrentHP}";
            }
            else
            {
                gameOverText.text = $"PARTIAL DEFEAT!\n\nYou survived all 5 rounds but had less HP.\n\nFinal HP: You {player.CurrentHP} - Enemy {enemyCurrentHP}";
            }
        }
        else
        {
            if (playerWon)
            {
                gameOverText.text = "COMPLETE VICTORY!\n\nYou defeated the enemy!";
            }
            else
            {
                gameOverText.text = "DEFEAT!\n\nYou have been defeated!";
            }
        }
    }

    void RestartGame()
    {
        currentRound = 1;
        combatStarted = false;
        player.ResetForNewCombat();
        gameOverPanel.SetActive(false);
        ShowCardSelection();
    }
}
