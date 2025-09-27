using UnityEngine;
using System.Linq;

public class PersonaToken : Token
{
    public int level = 1;
    public int body = 40;
    public int mind = 50;
    public int charisma = 40;
    public int loyalty = 50;
    public int shadow = 30;

    public Action[] proficencies;
    public Action[] flaws;
    public Action[] immunities;

    public bool isImmune(Action action)
    {
        return immunities != null && immunities.Contains(action);
    }

    public int getRating(HabilitiesEnum hability)
    {
        switch (hability)
        {
            case HabilitiesEnum.BODY:
                return body;
            case HabilitiesEnum.MIND:
                return mind;
            case HabilitiesEnum.LOYALTY:
                return loyalty;
            case HabilitiesEnum.CHARISMA:
                return charisma;
            case HabilitiesEnum.SHADOW:
                return shadow;
        }

        return 0;
    }

    public int getAttackScore()
    {
        int score = (Random.Range(5, body / 2));

        return score;
    }

    public int getInfluenceScore()
    {
        int score = (Random.Range(0, (charisma + loyalty/10)));

        return score;
    }

}
