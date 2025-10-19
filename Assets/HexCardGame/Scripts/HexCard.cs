using UnityEngine;

[CreateAssetMenu(fileName = "New Hex Card", menuName = "Hex Card Game/Card")]
public class HexCard : ScriptableObject
{
    public string cardName;
    public int attack;
    public int defense;
    [TextArea(3, 5)]
    public string description;
}
