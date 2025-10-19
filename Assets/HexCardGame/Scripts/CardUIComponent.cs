using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIComponent : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public Button cardButton;

    private HexCard card;
    private HexCardGameManager gameManager;

    public void Initialize(HexCard hexCard, HexCardGameManager manager)
    {
        card = hexCard;
        gameManager = manager;

        cardNameText.text = hexCard.cardName;
        attackText.text = $"ATK: {hexCard.attack}";
        defenseText.text = $"DEF: {hexCard.defense}";

        cardButton.onClick.AddListener(OnCardClicked);
    }

    void OnCardClicked()
    {
        gameManager.OnCardSelected(card);
    }
}
