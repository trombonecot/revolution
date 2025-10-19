using UnityEngine;

namespace HexGame
{
    /// <summary>
    /// Represents a selectable unit that can move between hexes
    /// </summary>
    public class Unit : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.green;

        private Renderer unitRenderer;
        private Material unitMaterial;
        private bool isSelected = false;
        private bool isMoving = false;
        private Vector3 targetPosition;
        private Vector3 previousPosition;

        public bool IsMoving => isMoving;
        public Vector3 PreviousPosition => previousPosition;

        private void Awake()
        {
            unitRenderer = GetComponent<Renderer>();

            if (unitRenderer != null)
            {
                // Create a unique material instance for this unit
                unitMaterial = unitRenderer.material;
                SetColor(normalColor);
            }
        }

        private void Update()
        {
            if (isMoving)
            {
                MoveToTarget();
            }

            // Check for ESC key to deselect
            if (isSelected && Input.GetKeyDown(KeyCode.Escape))
            {
                Deselect();
                if (HexGameManager.Instance != null)
                {
                    HexGameManager.Instance.DeselectUnit();
                }
            }
        }

        /// <summary>
        /// Called when unit is clicked
        /// </summary>
        private void OnMouseDown()
        {
            // Only allow selection if not currently moving
            if (!isMoving && HexGameManager.Instance != null)
            {
                HexGameManager.Instance.SelectUnit(this);
            }
        }

        public void Select()
        {
            isSelected = true;
            SetColor(selectedColor);
        }

        public void Deselect()
        {
            isSelected = false;
            SetColor(normalColor);
        }

        /// <summary>
        /// Initiates movement to the target hex center
        /// </summary>
        /// <param name="hexCenterPosition">The center position of the hex to move to</param>
        public void MoveTo(Vector3 hexCenterPosition)
        {
            // Store previous position before moving
            previousPosition = transform.position;

            targetPosition = hexCenterPosition;
            targetPosition.y = transform.position.y; // Maintain current height
            isMoving = true;
            Deselect(); // Deselect while moving
        }

        private void MoveToTarget()
        {
            // Calculate direction to target
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Rotate towards target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if reached target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;

                // Notify manager that movement is complete
                if (HexGameManager.Instance != null)
                {
                    HexGameManager.Instance.OnUnitMovementComplete();
                }
            }
        }

        private void SetColor(Color color)
        {
            if (unitMaterial != null)
            {
                // Try URP/Standard shader properties
                if (unitMaterial.HasProperty("_BaseColor"))
                {
                    unitMaterial.SetColor("_BaseColor", color);
                }
                else if (unitMaterial.HasProperty("_Color"))
                {
                    unitMaterial.color = color;
                }
            }
        }
    }
}
