using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject selectionPanel;
    public Transform cardSelectionGrid;
    public GameObject cardSelectionPrefab;
    public Button confirmButton;
    public TextMeshProUGUI instructionText;

    [Header("Settings")]
    public int requiredCardCount = 5;

    private List<HexCard> selectedCards = new List<HexCard>();
    private List<SelectableCardUI> cardUIComponents = new List<SelectableCardUI>();
    private Player player;
    private System.Action<List<HexCard>> onSelectionComplete;

    void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmSelection);
        confirmButton.interactable = false;
    }

    public void ShowSelectionPanel(Player playerRef, System.Action<List<HexCard>> callback)
    {
        player = playerRef;
        onSelectionComplete = callback;
        selectedCards.Clear();

        selectionPanel.SetActive(true);
        DisplayAllCards();
        UpdateUI();
    }

    void DisplayAllCards()
    {
        // Clear existing card UIs
        foreach (Transform child in cardSelectionGrid)
        {
            Destroy(child.gameObject);
        }
        cardUIComponents.Clear();

        // Create UI for each card in player's full deck
        foreach (HexCard card in player.fullDeck)
        {
            GameObject cardUI = Instantiate(cardSelectionPrefab, cardSelectionGrid);
            SelectableCardUI cardComponent = cardUI.GetComponent<SelectableCardUI>();
            cardComponent.Initialize(card, this);
            cardUIComponents.Add(cardComponent);
        }
    }

    public void OnCardToggled(HexCard card, bool isSelected)
    {
        if (isSelected)
        {
            if (selectedCards.Count < requiredCardCount)
            {
                selectedCards.Add(card);
            }
            else
            {
                // Deselect the card if we already have enough
                SelectableCardUI cardUI = cardUIComponents.Find(c => c.Card == card);
                if (cardUI != null)
                {
                    cardUI.SetSelected(false);
                }
                return;
            }
        }
        else
        {
            selectedCards.Remove(card);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        instructionText.text = $"Select {requiredCardCount} cards for combat ({selectedCards.Count}/{requiredCardCount})";
        confirmButton.interactable = selectedCards.Count == requiredCardCount;

        // Disable unselected cards if we've reached the limit
        if (selectedCards.Count >= requiredCardCount)
        {
            foreach (SelectableCardUI cardUI in cardUIComponents)
            {
                if (!selectedCards.Contains(cardUI.Card))
                {
                    cardUI.SetInteractable(false);
                }
            }
        }
        else
        {
            foreach (SelectableCardUI cardUI in cardUIComponents)
            {
                cardUI.SetInteractable(true);
            }
        }
    }

    void OnConfirmSelection()
    {
        if (selectedCards.Count == requiredCardCount)
        {
            selectionPanel.SetActive(false);
            onSelectionComplete?.Invoke(new List<HexCard>(selectedCards));
        }
    }

    public void HidePanel()
    {
        selectionPanel.SetActive(false);
    }
}
