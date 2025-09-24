using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MoveAction : Action
{

    private Token currentToken;
    void Start()
    {
        actionName = "Move now!";
    }

    public override void OnClick()
    {
        SelectionListener.SetSelectionEnabled(false);
        currentToken = UiManager.GetActiveToken();

        UiManager.Instance.StartCoroutine(WaitForTerrainClick());
    }

    private IEnumerator WaitForTerrainClick()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Taulell")
            {
                if (currentToken != null)
                {
                    NavMeshAgent agent = currentToken.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.SetDestination(hit.point);
                        currentToken.isBlocked = true;
                        UiManager.SetActiveToken(null);
                    }
                    else
                    {
                        Debug.LogWarning("Active token has no NavMeshAgent!");
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
