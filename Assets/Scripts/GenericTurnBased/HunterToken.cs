using UnityEngine;
using UnityEngine.AI;

public class HunterToken : EnemyToken
{
    public PersonaToken detectedEnemy;

    private DetectAction detectAction;
    private AttackAction attackAction;

    void Start()
    {
        detectAction = GetComponent<DetectAction>();
        attackAction = GetComponent<AttackAction>();
    }

    public override void DoSomething()
    {
        if (detectedEnemy && Vector3.Distance(detectedEnemy.transform.position, transform.position) < 5f)
        {
            this.Hunt();
        } else
        {
            detectedEnemy = null;
            detectAction.OnActivate(this, null);
            detectedEnemy = detectAction.detectedTarget;
            detectAction.detectedTarget = null;

            if (!detectedEnemy)
            {
                if (Random.Range(0,10) < 5 )
                {
                    this.MoveAround();
                } else if (SuccessManager.IsSuccessfulInHabilityScore(this, HabilitiesEnum.CHARISMA))
                {
                    InfluenceManager.SubstractInfluence(this.getInfluenceScore());
                }
            }
        }
        SpendAction();
    }

    private void MoveAround()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 6f;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 3f, NavMesh.AllAreas))
            {
                agent.stoppingDistance = 0f;
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            Debug.LogWarning("Active token has no NavMeshAgent!");
        }
    }

    private void Hunt()
    {
        if (detectedEnemy != null)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.stoppingDistance = 0.5f;

                float distance = Vector3.Distance(detectedEnemy.transform.position, transform.position);

                if (distance > agent.stoppingDistance)
                {
                    agent.SetDestination(detectedEnemy.transform.position);
                }
                else
                {
                    attackAction.OnActivate(this, detectedEnemy);
                }
            }
            else
            {
                Debug.LogWarning("Active token has no NavMeshAgent!");
            }
        }
    }

}
