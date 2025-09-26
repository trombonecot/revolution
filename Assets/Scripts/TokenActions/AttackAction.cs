using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AttackAction : Action
{
    private PersonaToken currentToken;

    void Start()
    {
        actionName = "Attack";
    }

    public override void OnActivate(Token active, Token target)
    {
        SelectionListener.SetSelectionEnabled(false);
        currentToken = (PersonaToken)active;

        if (target)
        {
            this.Attack((PersonaToken)target);
        } else
        {
            GameManager.Instance.StartCoroutine(WaitForEnemyClick());
        }
    }

    public void Attack(PersonaToken enemyToken)
    {
        if (SuccessManager.IsSuccessful(currentToken, enemyToken, this))
        {
            enemyToken.RecieveHarm(currentToken.getAttackScore());
        }
        else
        {
            Debug.Log("Enemy dodged the attack");
        }


        currentToken.SpendAction();
        UiManager.SetActiveToken(null);
    }

    private IEnumerator WaitForEnemyClick()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "ComputerToken")
            {
                if (currentToken != null)
                {
                    NavMeshAgent agent = currentToken.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        PersonaToken enemyToken = hit.collider.gameObject.GetComponent<PersonaToken>();
                        if (enemyToken != null)
                        {
                            float distance = Vector3.Distance(currentToken.transform.position, enemyToken.transform.position);
                            if (distance < 1f)
                            {
                                this.Attack(enemyToken);
                            }
                            else
                            {
                                Debug.Log("Enemy is too far away to attack.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Hit object has no Token component!");
                        }
                    }

                }
                else
                {
                    Debug.LogWarning("No active token found in UIManager!");
                }
            }
        }

        SelectionListener.SetSelectionEnabled(true);
    }
}
