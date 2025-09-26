using UnityEngine;

public class HideAction : Action
{
    public override void OnActivate(Token token, Token target)
    {
        GameObject[] computerTokens = GameObject.FindGameObjectsWithTag("ComputerToken");

        foreach (GameObject enemy in computerTokens)
        {
            HunterToken hunter = enemy.GetComponent<HunterToken>();

            if (hunter != null && hunter.detectedEnemy == token)
            {
                if (SuccessManager.IsSuccessful((PersonaToken)token, hunter, this)) {
                    hunter.detectedEnemy = null;
                }
            }
        }

        token.SpendAction();
        UiManager.SetActiveToken(null);
    }
}
