using UnityEngine;

namespace HexGame
{
    /// <summary>
    /// Represents a single hex tile that units can move to
    /// </summary>
    public class HexTile : MonoBehaviour
    {
        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;

        private Renderer hexRenderer;
        private Material hexMaterial;
        private Color originalColor;

        public Vector3 CenterPosition => transform.position;

        private void Awake()
        {
            hexRenderer = GetComponent<Renderer>();

            if (hexRenderer != null)
            {
                // Create a unique material instance for this hex
                hexMaterial = hexRenderer.material;

                // Try to get the current color from the material
                if (hexMaterial.HasProperty("_Color"))
                {
                    originalColor = hexMaterial.color;
                }
                else if (hexMaterial.HasProperty("_BaseColor"))
                {
                    originalColor = hexMaterial.GetColor("_BaseColor");
                }
                else
                {
                    originalColor = normalColor;
                }

                SetColor(normalColor);
            }
        }

        /// <summary>
        /// Called when mouse enters the hex
        /// </summary>
        private void OnMouseEnter()
        {
            if (HexGameManager.Instance != null && HexGameManager.Instance.HasSelectedUnit())
            {
                Highlight();
            }
        }

        /// <summary>
        /// Called when mouse exits the hex
        /// </summary>
        private void OnMouseExit()
        {
            ResetHighlight();
        }

        /// <summary>
        /// Called when hex is clicked
        /// </summary>
        private void OnMouseDown()
        {
            if (HexGameManager.Instance != null && HexGameManager.Instance.HasSelectedUnit())
            {
                HexGameManager.Instance.MoveUnitToHex(this);
            }
        }

        public void Highlight()
        {
            SetColor(highlightColor);
        }

        public void ResetHighlight()
        {
            SetColor(normalColor);
        }

        private void SetColor(Color color)
        {
            if (hexMaterial != null)
            {
                // Try URP/Standard shader properties
                if (hexMaterial.HasProperty("_BaseColor"))
                {
                    hexMaterial.SetColor("_BaseColor", color);
                }
                else if (hexMaterial.HasProperty("_Color"))
                {
                    hexMaterial.color = color;
                }
            }
        }
    }
}
