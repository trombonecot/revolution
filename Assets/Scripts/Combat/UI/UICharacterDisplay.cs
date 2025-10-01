using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UICharacterDisplay : MonoBehaviour, IPointerClickHandler
{
    [Header("Character Data")]
    public Character characterData;

    [Header("UI References")]
    public Image characterPortrait;
    public TextMeshProUGUI characterNameText;
    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Image assignedCardPreview;
    public GameObject abilityReadyIcon;
    public GameObject statusEffectIcon;
    public TextMeshProUGUI statusEffectText;
    public GameObject deadOverlay;

    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color deadColor = Color.gray;
    public Image selectionBorder;

    private bool isSelected = false;

    public System.Action<UICharacterDisplay> OnCharacterClicked;

    void Start()
    {
        UpdateDisplay();
    }

    public void SetCharacter(Character character)
    {
        characterData = character;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (characterData == null) return;

        // Nom
        characterNameText.text = characterData.characterName;

        // HP
        hpBar.maxValue = characterData.maxHP;
        hpBar.value = characterData.currentHP;
        hpText.text = $"{characterData.currentHP}/{characterData.maxHP}";

        // Portrait
        if (characterPortrait != null && characterData.characterSprite != null)
            characterPortrait.sprite = characterData.characterSprite;

        // Carta assignada
        if (assignedCardPreview != null)
        {
            if (characterData.assignedCard != null && characterData.assignedCard.cardImage != null)
            {
                assignedCardPreview.gameObject.SetActive(true);
                assignedCardPreview.sprite = characterData.assignedCard.cardImage;
            }
            else
            {
                assignedCardPreview.gameObject.SetActive(false);
            }
        }

        // Habilitat disponible
        if (abilityReadyIcon != null)
            abilityReadyIcon.SetActive(!characterData.abilityUsed && characterData.canUseAbility);

        // Estat alterat
        if (statusEffectIcon != null)
        {
            bool hasStatus = characterData.activeStatus != StatusEffect.None;
            statusEffectIcon.SetActive(hasStatus);

            if (hasStatus && statusEffectText != null)
                statusEffectText.text = characterData.activeStatus.ToString();
        }

        // Mort
        if (deadOverlay != null)
            deadOverlay.SetActive(!characterData.isAlive);

        // Color segons estat
        if (characterPortrait != null)
        {
            characterPortrait.color = characterData.isAlive ? normalColor : deadColor;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionBorder != null)
            selectionBorder.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (characterData != null && characterData.isAlive)
        {
            OnCharacterClicked?.Invoke(this);
        }
    }
}