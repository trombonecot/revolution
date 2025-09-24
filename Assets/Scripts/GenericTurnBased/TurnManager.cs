using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int currentTurn = 0;
    public int playerSideIndex = 0;
    public PlayerSide[] sides;

    public void Initialize()
    {
        sides[0].UnBlockTokens();
        sides[1].BlockTokens();

        currentTurn = 1;
    }

    public void NextTurn()
    {
       currentTurn++;

        sides[playerSideIndex].BlockTokens();
        if (playerSideIndex == sides.Length - 1)
        {
            playerSideIndex = 0;
        }
        else
        {
            playerSideIndex++;
        }
        sides[playerSideIndex].UnBlockTokens();
    }

}
