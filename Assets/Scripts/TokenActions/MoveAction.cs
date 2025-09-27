using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MoveAction : Action
{
    private Token currentToken;
    private GameObject moveRangeCircle;
    private float moveRadius = 3f;
    private Vector3 tokenGroundCenter;

    public override void OnActivate(Token active, Token target)
    {
        SelectionListener.SetSelectionEnabled(false);
        currentToken = UiManager.GetActiveToken();

        if (currentToken == null)
        {
            Debug.LogWarning("No active token found in UIManager!");
            SelectionListener.SetSelectionEnabled(true);
            return;
        }

        moveRangeCircle = RangeCircleUtils.ShowRangeCircle(currentToken, moveRadius);

        tokenGroundCenter = new Vector3(currentToken.transform.position.x, -0.8f, currentToken.transform.position.z);

        GameManager.Instance.StartCoroutine(WaitForTerrainClick());
    }

    private IEnumerator WaitForTerrainClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Taulell") && currentToken != null)
                    {
                        float horizontalDist = Vector2.Distance(
                            new Vector2(tokenGroundCenter.x, tokenGroundCenter.z),
                            new Vector2(hit.point.x, hit.point.z)
                        );

                        if (horizontalDist <= moveRadius + 0.001f)
                        {
                            NavMeshAgent agent = currentToken.GetComponent<NavMeshAgent>();
                            if (agent != null)
                            {
                                agent.SetDestination(hit.point);
                                currentToken.SpendAction();
                                UiManager.SetActiveToken(null);
                            }
                            else
                            {
                                Debug.LogWarning("Active token has no NavMeshAgent!");
                            }

                            break;
                        }
                        else
                        {
                            Debug.Log($"Click outside move range ({horizontalDist:F2} > {moveRadius:F2}). Ignored.");
                        }
                    }
                }
            }
            yield return null;
        }

        RangeCircleUtils.HideRangeCircle(moveRangeCircle);
        SelectionListener.SetSelectionEnabled(true);
    }
}
