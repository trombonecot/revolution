using UnityEngine;

[System.Serializable]
public enum CardType
{
    Melee,      // Cos a cos
    Magic,      // M�gia
    Ranged      // Projectil
}

[System.Serializable]
public enum StatusEffect
{
    None,
    Fire,       // Foc: -1 Defensa seg�ent ronda
    Poison,     // Ver�: -1 Atac seg�ent ronda
    Ice         // Gel: no pot activar habilitats seg�ent ronda
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