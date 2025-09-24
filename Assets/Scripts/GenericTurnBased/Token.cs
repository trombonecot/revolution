using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public bool isBlocked = false;
    public Action[] buttonActions;
    public int numActions = 1;
    public int maxNumActions = 1;
    public int health = 100;
    public int maxHealth = 100;

    public void RestoreActions()
    {
        numActions = maxNumActions;
        isBlocked = false;
    }

    public void SpendAction()
    {
        numActions--;
        if (numActions <= 0 )
        {
            isBlocked = true;
        }
    }

    public void RecieveHarm(int value)
    {
        this.health -= value;

        if (this.health <= 0 )
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
