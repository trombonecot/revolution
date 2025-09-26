using Unity.VisualScripting;
using UnityEngine;

public class DetectAction : Action
{
    public PersonaToken detectedTarget;

    public override void OnActivate(Token token, Token target)
    {
        float radius = 4f;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        Debug.Log($"{name} detected with roll {hits.Length}");

        foreach (Collider hit in hits)
        {
            PersonaToken player = hit.GetComponent<PersonaToken>();
            if (player != null && player.gameObject.tag == "PlayerToken")
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);

                if (distance > 0f)
                {

                    if (SuccessManager.IsSuccessful((PersonaToken)token, player, this))
                    {
                        detectedTarget = player;
                        Debug.Log($"{name} detected {player.name}");
                        return;
                    }
                }
            }
        }

        // If no one detected, log it (optional)
        Debug.Log($"{name} detected no players.");
    }
}
