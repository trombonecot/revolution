using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UICardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Card Data")]
    public Card cardData;

    [Header("UI References")]
    public Image cardImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI descriptionText;
    public Image typeIcon;
    public GameObject statusEffectIcon;

    [Header("Visual Settings")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float hoverSpeed = 5f;

    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isSelected = false;

    public System.Action<UICardDisplay> OnCardClicked;

    void Start()
    {
        originalScale = transform.localScale;
        UpdateCardDisplay();
    }

    void Update()
    {
        // Animació de hover
        Vector3 targetScale = isHovered ? hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * hoverSpeed);
    }

    public void SetCard(Card card)
    {
        cardData = card;
        UpdateCardDisplay();
    }

    void UpdateCardDisplay()
    {
        if (cardData == null) return;

        cardNameText.text = cardData.cardName;
        attackText.text = cardData.attack.ToString();
        defenseText.text = cardData.defense.ToString();
        descriptionText.text = cardData.description;

        if (cardImage != null && cardData.cardImage != null)
            cardImage.sprite = cardData.cardImage;

        // Mostrar icona d'efecte d'estat
        if (statusEffectIcon != null)
            statusEffectIcon.SetActive(cardData.statusEffect != StatusEffect.None);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClicked?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        // Pots afegir efecte visual aquí (per exemple, canviar color del border)
    }
}