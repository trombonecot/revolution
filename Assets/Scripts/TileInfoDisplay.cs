using UnityEngine;
using TMPro;

public class TileInfoDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI infoText;   // assign a TMP Text from your Canvas in Inspector

    private void Start()
    {
        if (infoText != null)
            infoText.text = "";
    }

    private void Update()
    {
        // Cast a ray from mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Tile"))
            {
                Tile tileData = hit.collider.GetComponent<Tile>();
                if (tileData != null && infoText != null)
                {
                    infoText.text = $"Influencia: {tileData.influence}";
                }
            }
            else
            {
                if (infoText != null) infoText.text = "";
            }
        }
        else
        {
            if (infoText != null) infoText.text = "";
        }
    }
}
