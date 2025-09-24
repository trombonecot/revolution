using UnityEngine;

public class RecruitAction : Action
{
    public GameObject recruit;
    public int cost;

    public override void OnClick()
    {
        BaseToken currentToken = (BaseToken)UiManager.GetActiveToken();

        if (currentToken && currentToken.spawnLocation != null && InfluenceManager.GetCurrentInfluence() - cost > 0)
        {
            Instantiate(
                recruit,
                currentToken.spawnLocation.position,
                currentToken.spawnLocation.rotation
            );

            InfluenceManager.SubstractInfluence(cost);
        }

        UiManager.SetActiveToken(null);
    }

}
