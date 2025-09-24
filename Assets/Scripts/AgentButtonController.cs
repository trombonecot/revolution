using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AgentButtonController : MonoBehaviour
{
    [Header("UI References")]
    public Button button1;
    public Button button2;

    [Header("Target Object")]
    public GameObject agent;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;   // units per second
    private bool isMoving = false;

    private bool button1Selected = false;

    [Header("Agent Tile")]
    public Tile currentTile; // ✅ Set the default starting tile from Unity Inspector

    void Start()
    {
        // Make sure both buttons are disabled at start
        SetButtonsActive(false);

        // Assign listeners
        button1.onClick.AddListener(() => OnButtonPressed(1));
        button2.onClick.AddListener(() => OnButton2Pressed());
    }

    void Update()
    {
        if (isMoving) return; // block clicks during movement

        // ✅ Don't process clicks if over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0)) // left mouse click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == agent)
                {
                    // Enable buttons when clicking on agent
                    SetButtonsActive(true);
                }
                else if (hit.collider.CompareTag("Tile"))
                {
                    HandleTileClick(hit.collider.GetComponent<Tile>());
                }
                else
                {
                    // Disable buttons if clicked anywhere else
                    SetButtonsActive(false);
                    ResetButtonSelections();
                }
            }
            else
            {
                // Clicked empty space
                SetButtonsActive(false);
                ResetButtonSelections();
            }
        }
    }

    void OnButtonPressed(int buttonId)
    {
        ResetButtonSelections(); // only one button can be active at a time

        if (buttonId == 1)
        {
            button1Selected = true;
            Debug.Log("Button 1 selected. Now click a Tile.");
        }
    }

    void OnButton2Pressed()
    {
        if (currentTile != null)
        {
            currentTile.influence += 10;
            Debug.Log("Increased influence of tile " + currentTile.name + " to " + currentTile.influence);
        }
        else
        {
            Debug.LogWarning("No tile assigned to agent!");
        }

        // Hide buttons after use
        SetButtonsActive(false);
        ResetButtonSelections();
    }

    void HandleTileClick(Tile tile)
    {
        if (button1Selected)
        {
            Debug.Log("Button 1 action: Move agent to " + tile.name);
            StartCoroutine(MoveAgentTo(tile));
        }

        // Reset after action
        ResetButtonSelections();
        SetButtonsActive(false);
    }

    IEnumerator MoveAgentTo(Tile tile)
    {
        isMoving = true;
        Vector3 targetPos = tile.transform.position;

        while (Vector3.Distance(agent.transform.position, targetPos) > 0.05f)
        {
            agent.transform.position = Vector3.MoveTowards(
                agent.transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        agent.transform.position = targetPos; // snap to final position
        isMoving = false;

        // ✅ Update current tile to the tile we moved to
        currentTile = tile;
        Debug.Log("Agent is now on tile: " + currentTile.name);
    }

    void SetButtonsActive(bool active)
    {
        button1.gameObject.SetActive(active);
        button2.gameObject.SetActive(active);
    }

    void ResetButtonSelections()
    {
        button1Selected = false;
    }
}
