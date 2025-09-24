using UnityEngine;

public class InfluenceManager : MonoBehaviour
{
    public float currentInfluence = 10;

    public float currentInfluenceIncrement = 1;
    public static InfluenceManager Instance { get; private set; }

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
    public static float GetCurrentInfluence()
    {
        if (Instance == null)
            return 0;

        return Instance.currentInfluence;
    }

    public static float GetCurrentInfluenceIncrement()
    {
        if (Instance == null)
            return 0;

        return Instance.currentInfluenceIncrement;
    }

    public static void IncrementInfluence()
    {
        if (Instance == null)
            return;

        Instance.currentInfluence += Instance.currentInfluenceIncrement;
    }
    public static void IncrementIncrement(float value)
    {
        if (Instance == null)
            return;

        Instance.currentInfluenceIncrement += value;
    }


    public static void SubstractInfluence(float value)
    {
        if (Instance == null)
            return;

        Instance.currentInfluence -= value;
    }
}
