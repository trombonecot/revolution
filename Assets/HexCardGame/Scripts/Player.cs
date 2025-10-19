using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Player Deck")]
    public List<HexCard> fullDeck = new List<HexCard>();

    [Header("Player Stats")]
    public int maxHP = 20;
    private int currentHP;

    // Cards selected for this combat (5 cards)
    private List<HexCard> selectedCombatCards = new List<HexCard>();

    // Cards already used during combat
    private List<HexCard> usedCards = new List<HexCard>();

    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = Mathf.Max(0, value);
    }

    public List<HexCard> SelectedCombatCards => selectedCombatCards;
    public List<HexCard> UsedCards => usedCards;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void SetSelectedCards(List<HexCard> cards)
    {
        selectedCombatCards = new List<HexCard>(cards);
        usedCards.Clear();
    }

    public List<HexCard> GetAvailableCards()
    {
        List<HexCard> available = new List<HexCard>();
        foreach (HexCard card in selectedCombatCards)
        {
            if (!usedCards.Contains(card))
            {
                available.Add(card);
            }
        }
        return available;
    }

    public void UseCard(HexCard card)
    {
        if (!usedCards.Contains(card))
        {
            usedCards.Add(card);
        }
    }

    public void ResetForNewCombat()
    {
        currentHP = maxHP;
        selectedCombatCards.Clear();
        usedCards.Clear();
    }

    public bool HasAvailableCards()
    {
        return GetAvailableCards().Count > 0;
    }
}
