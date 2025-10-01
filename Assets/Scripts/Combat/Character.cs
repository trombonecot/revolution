using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public int maxHP;
    public int currentHP;
    public int speed; // Per determinar iniciativa
    public bool isAlive;

    // Habilitat única
    public Ability uniqueAbility;
    public bool abilityUsed; // Només 1 cop per combat

    // Estats alterats actuals
    public StatusEffect activeStatus;
    public int statusDuration; // Rondes restants

    // Carta assignada aquesta ronda
    public Card assignedCard;

    // Modificadors temporals
    public int tempAttackMod;
    public int tempDefenseMod;
    public bool canUseAbility;

    public Sprite characterSprite;

    public Character(string name, int hp, int spd, Ability ability)
    {
        characterName = name;
        maxHP = hp;
        currentHP = hp;
        speed = spd;
        uniqueAbility = ability;
        isAlive = true;
        abilityUsed = false;
        activeStatus = StatusEffect.None;
        canUseAbility = true;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            isAlive = false;
        }
    }

    public void ApplyStatus(StatusEffect effect)
    {
        activeStatus = effect;
        statusDuration = 1; // Dura 1 ronda

        switch (effect)
        {
            case StatusEffect.Fire:
                tempDefenseMod = -1;
                break;
            case StatusEffect.Poison:
                tempAttackMod = -1;
                break;
            case StatusEffect.Ice:
                canUseAbility = false;
                break;
        }
    }

    public void UpdateStatus()
    {
        if (statusDuration > 0)
        {
            statusDuration--;
            if (statusDuration == 0)
            {
                ClearStatus();
            }
        }
    }

    private void ClearStatus()
    {
        activeStatus = StatusEffect.None;
        tempAttackMod = 0;
        tempDefenseMod = 0;
        canUseAbility = true;
    }

    public int GetTotalAttack()
    {
        if (assignedCard == null) return 0;
        return assignedCard.attack + tempAttackMod;
    }

    public int GetTotalDefense()
    {
        if (assignedCard == null) return 0;
        return assignedCard.defense + tempDefenseMod;
    }
}