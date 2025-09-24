using UnityEngine;
using System.Collections;

public class PlayerSide : MonoBehaviour
{
    public bool isIA = false;
    public string tokenName;

    void Start()
    {
        
    }

    public void RunIA()
    {
        StartCoroutine(IARoutine());
    }

    private IEnumerator IARoutine()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tokenName);

        foreach (GameObject go in gameObjects)
        {
            EnemyToken token = go.GetComponent<EnemyToken>();
            if (token != null && !token.isBlocked)
            {
                token.DoSomething();
            }

            yield return new WaitForSeconds(1f);
        }

        GameManager.GoToNextTurn();
    }

    public void BlockTokens()
    {
        this.ChangeBlockStatusOfTokens(true);
    }

    public void UnBlockTokens()
    {
        this.ChangeBlockStatusOfTokens(false);
    }

    private void ChangeBlockStatusOfTokens(bool status)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tokenName);

        for (int i = 0; i < gameObjects.Length; i++)
        {
            Token token = gameObjects[i].GetComponent<Token>();
            token.isBlocked = status;

            if (status == false)
            {
                token.RestoreActions();
            }
        }
    }
}
