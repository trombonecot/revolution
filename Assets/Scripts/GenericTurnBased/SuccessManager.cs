using Unity.VisualScripting;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class SuccessManager : MonoBehaviour
{
    public static SuccessManager Instance { get; private set; }

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

    public static bool IsSuccessful(
        PersonaToken active,
        PersonaToken target,
        Action action
        ) 
    {
        if (target.isImmune(action))
        {
            return false;
        }

        int activeHabilityRating = active.getRating(action.activeHability);

        bool activeSuccess = Random.Range(1, 100) < activeHabilityRating;

        if (activeSuccess && target != null)
        {
            Debug.Log($"{active.name} is success with {activeHabilityRating} on {action.activeHability} as active on {action.name}");

            int targetHabilityRating = target.getRating(action.targetHability);
            bool targetSuccess = Random.Range(1, 100) < targetHabilityRating;

            if (targetSuccess)
            {
                Debug.Log($"{target.name} is success as target on {action.name}");
            }

            return !targetSuccess;
        }

        return false;
    }

    public static bool IsSuccessfulInHabilityScore(
        PersonaToken active,
        HabilitiesEnum hability
        )
    {
       int activeHabilityRating = active.getRating(hability);

       return Random.Range(1, 100) < activeHabilityRating;
    }

}
