using UnityEngine;

public class MouseHoverHighlightURP : MonoBehaviour
{
    public Color highlightColor = Color.yellow;

    private Material material;
    private Color originalEmissionColor;
    private bool isHighlighted = false;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        material.EnableKeyword("_EMISSION");

        originalEmissionColor = material.GetColor("_EmissionColor");

        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        DynamicGI.SetEmissive(GetComponent<Renderer>(), originalEmissionColor);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            if (!isHighlighted)
                Highlight(true);
        }
        else
        {
            if (isHighlighted)
                Highlight(false);
        }
    }

    void Highlight(bool enable)
    {
        if (enable)
        {
            material.SetColor("_EmissionColor", highlightColor);
            material.EnableKeyword("_EMISSION");

            DynamicGI.SetEmissive(GetComponent<Renderer>(), highlightColor);

            isHighlighted = true;
        }
        else
        {
            material.SetColor("_EmissionColor", originalEmissionColor);
            material.EnableKeyword("_EMISSION");

            DynamicGI.SetEmissive(GetComponent<Renderer>(), originalEmissionColor);

            isHighlighted = false;
        }
    }
}
