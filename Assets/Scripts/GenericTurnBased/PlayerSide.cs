using UnityEngine;

public class PlayerSide : MonoBehaviour
{
    public bool isIA = false;
    public string tokenName;

    void Start()
    {
        
    }

    void Update()
    {
        
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
            gameObjects[i].GetComponent<Token>().isBlocked = status;
        }
    }
}
