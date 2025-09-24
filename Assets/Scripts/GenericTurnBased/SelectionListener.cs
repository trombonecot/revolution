using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionListener : MonoBehaviour
{
    public bool selectionEnabled = true;

    public static SelectionListener Instance { get; private set; }

    public static void SetSelectionEnabled(bool enabled)
    {
        if (Instance == null)
        {
            return;
        }

        Instance.selectionEnabled = enabled;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Token token = hit.collider.gameObject.GetComponent<Token>();

                if (token && !token.isBlocked)
                {
                    UiManager.SetActiveToken(token);

                    return;
                }
            }

            UiManager.SetActiveToken(null);
        }
    }
}
