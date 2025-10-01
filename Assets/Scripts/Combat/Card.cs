using UnityEngine;

[System.Serializable]
public enum CardType
{
    Melee,      // Cos a cos
    Magic,      // Màgia
    Ranged      // Projectil
}

[System.Serializable]
public enum StatusEffect
{
    None,
    Fire,       // Foc: -1 Defensa següent ronda
    Poison,     // Verí: -1 Atac següent ronda
    Ice         // Gel: no pot activar habilitats següent ronda
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int attack;
    public int defense;
    public StatusEffect statusEffect;
    public Sprite cardImage;

    [TextArea(3, 5)]
    public string description;
}