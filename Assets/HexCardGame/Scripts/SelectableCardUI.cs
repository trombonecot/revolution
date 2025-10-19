using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectableCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public Toggle cardToggle;
    public Image backgroundImage;

    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private HexCard card;
    private CardSelectionUI selectionManager;

    public HexCard Card => card;

    public void Initialize(HexCard hexCard, CardSelectionUI manager)
    {
        card = hexCard;
        selectionManager = manager;

        cardNameText.text = hexCard.cardName;
        attackText.text = $"ATK: {hexCard.attack}";
        defenseText.text = $"DEF: {hexCard.defense}";

        cardToggle.isOn = false;
        cardToggle.onValueChanged.AddListener(OnToggleChanged);

        UpdateVisuals();
    }

    void OnToggleChanged(bool isOn)
    {
        selectionManager.OnCardToggled(card, isOn);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        backgroundImage.color = cardToggle.isOn ? selectedColor : normalColor;
    }

    public void SetSelected(bool selected)
    {
        cardToggle.isOn = selected;
    }

    public void SetInteractable(bool interactable)
    {
        cardToggle.interactable = interactable;
    }
}
