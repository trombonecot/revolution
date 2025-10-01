using UnityEngine;

[System.Serializable]
public enum AbilityType
{
    DamageBoost,    // Augmenta dany
    DefenseBoost,   // Augmenta defensa
    Heal,           // Cura HP
    StatusInflict,  // Aplica estat alterat
    SpeedBoost      // Augmenta iniciativa
}

[CreateAssetMenu(fileName = "New Ability", menuName = "Card Game/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public AbilityType abilityType;
    public int value; // Quantitat d'efecte
    public StatusEffect inflictStatus; // Si aplica estat

    [TextArea(2, 4)]
    public string description;

    public void Activate(Character caster, Character target = null)
    {
        switch (abilityType)
        {
            case AbilityType.DamageBoost:
                caster.tempAttackMod += value;
                break;

            case AbilityType.DefenseBoost:
                caster.tempDefenseMod += value;
                break;

            case AbilityType.Heal:
                caster.currentHP = Mathf.Min(caster.currentHP + value, caster.maxHP);
                break;

            case AbilityType.StatusInflict:
                if (target != null)
                    target.ApplyStatus(inflictStatus);
                break;

            case AbilityType.SpeedBoost:
                caster.speed += value;
                break;
        }

        caster.abilityUsed = true;
    }
}