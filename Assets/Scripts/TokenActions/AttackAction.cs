using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AttackAction : Action
{

    private Token currentToken;
    void Start()
    {
        actionName = "Attack";
    }

    public override void OnClick()
    {
        SelectionListener.SetSelectionEnabled(false);
        currentToken = UiManager.GetActiveToken();

        GameManager.Instance.StartCoroutine(WaitForEnemyClick());
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
                        Token enemyToken = hit.collider.gameObject.GetComponent<Token>();
                        if (enemyToken != null)
                        {
                            float distance = Vector3.Distance(currentToken.transform.position, enemyToken.transform.position);
                            if (distance < 1f)
                            {
                                enemyToken.RecieveHarm(50);
                                currentToken.SpendAction();
                                UiManager.SetActiveToken(null);
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
