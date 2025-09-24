using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TurnManager turnManager;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        if (Instance == null)
            return;

        Instance.turnManager.Initialize();
    }

    public static PlayerSide GetActiveSide()
    {
        if (Instance == null)
            return null;

        return Instance.turnManager.sides[Instance.turnManager.playerSideIndex];
    }

    public static int GetTurnNumer()
    {
        if (Instance == null)
            return 0;

        return Instance.turnManager.currentTurn;
    }
    public static void GoToNextTurn()
    {
        if (Instance == null)
            return;

        Instance.turnManager.NextTurn();
    }
}
