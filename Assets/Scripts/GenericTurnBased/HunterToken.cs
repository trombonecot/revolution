using UnityEngine;
using UnityEngine.AI;

public class HunterToken : EnemyToken
{
    public Token detectedEnemy;

    public override void DoSomething()
    {
        if (detectedEnemy)
        {
            this.Hunt();
        } else
        {
            this.Detect();
        }
        SpendAction();
    }

    private void Detect()
    {
        float radius = 6f;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        Debug.Log($"{name} detected with roll {hits.Length}");

        foreach (Collider hit in hits)
        {
            Token player = hit.GetComponent<Token>();
            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);

                if (distance > 0f) 
                {
                    float detectionChance = (1f / distance) * 100f;
                    int roll = Random.Range(1, 101);

                    if (roll < detectionChance)
                    {
                        detectedEnemy = player;
                        Debug.Log($"{name} detected {player.name} with roll {roll} < {detectionChance}");
                        return;
                    }
                }
            }
        }

        // If no one detected, log it (optional)
        Debug.Log($"{name} detected no players.");
    }


    private void Hunt()
    {
        if (detectedEnemy != null)
        {
            float distance = Vector3.Distance(detectedEnemy.transform.position, this.gameObject.transform.position);

            if (distance > 1)
            {
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(detectedEnemy.transform.position);
                }
                else
                {
                    Debug.LogWarning("Active token has no NavMeshAgent!");
                }
            } else
            {
                detectedEnemy.RecieveHarm(50);
            }
        }
    }
}
