using UnityEngine;

public class InfluenceAction : Action
{
    public override void OnClick()
    {
        Token currentToken = UiManager.GetActiveToken();
        if (currentToken == null)
        {
            Debug.LogWarning("InfluenceAction: No active token found.");
            return;
        }

        GameObject[] computerTokens = GameObject.FindGameObjectsWithTag("ComputerToken");

        int nearbyCount = 0;
        foreach (GameObject token in computerTokens)
        {
            float distance = Vector3.Distance(currentToken.transform.position, token.transform.position);
            if (distance <= 3f)
            {
                nearbyCount++;
            }
        }

        float randomValue = Random.Range(0.5f, 1f);

        float increment = (nearbyCount > 0) ? randomValue / nearbyCount : randomValue;

        InfluenceManager.IncrementIncrement(increment);
        currentToken.SpendAction();
        UiManager.SetActiveToken(null);
    }
}
