using UnityEngine;

public class InfluenceAction : Action
{
    public PersonaToken counterInfluencer;

    public override void OnActivate(Token active, Token target)
    {

        GameObject[] computerTokens = GameObject.FindGameObjectsWithTag("ComputerToken");

        foreach (GameObject enemy in computerTokens)
        {
            float distance = Vector3.Distance(active.transform.position, enemy.transform.position);
            if (distance <= 3f)
            {
                counterInfluencer = enemy.GetComponent<PersonaToken>();
            }
        }

        PersonaToken influencer = (PersonaToken)active;

        if (!counterInfluencer || SuccessManager.IsSuccessful(influencer, counterInfluencer, this))
        {
            InfluenceManager.IncrementIncrement(influencer.getInfluenceScore());

        }

        active.SpendAction();
        UiManager.SetActiveToken(null);
    }
}
