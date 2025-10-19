using UnityEngine;

namespace HexGame
{
    /// <summary>
    /// Represents an enemy on a hex tile that triggers combat when encountered
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private int enemyLevel = 1;
        [SerializeField] private int maxHP = 20;

        [Header("Visual Feedback")]
        [SerializeField] private Color enemyColor = Color.red;

        private HexTile occupiedTile;
        private Renderer enemyRenderer;
        private Material enemyMaterial;

        public int EnemyLevel => enemyLevel;
        public int MaxHP => maxHP;
        public HexTile OccupiedTile => occupiedTile;

        private void Awake()
        {
            enemyRenderer = GetComponent<Renderer>();

            if (enemyRenderer != null)
            {
                // Create a unique material instance for this enemy
                enemyMaterial = enemyRenderer.material;
                SetColor(enemyColor);
            }
        }

        private void Start()
        {
            // Find the hex tile this enemy is on
            FindOccupiedTile();
        }

        /// <summary>
        /// Finds and registers this enemy with the hex tile it's positioned on
        /// </summary>
        private void FindOccupiedTile()
        {
            // First, try to find the closest hex tile by distance
            HexTile[] allTiles = FindObjectsOfType<HexTile>();
            HexTile closestTile = null;
            float closestDistance = float.MaxValue;

            foreach (HexTile tile in allTiles)
            {
                float distance = Vector3.Distance(transform.position, tile.CenterPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTile = tile;
                }
            }

            if (closestTile != null && closestDistance < 2f) // Within 2 units
            {
                occupiedTile = closestTile;
                closestTile.SetEnemy(this);
                Debug.Log($"<color=red>Enemy '{gameObject.name}' registered on tile at {closestTile.CenterPosition} (distance: {closestDistance:F2})</color>");
            }
            else
            {
                Debug.LogWarning($"<color=orange>Enemy '{gameObject.name}' could not find a nearby hex tile! Closest distance: {closestDistance:F2}</color>");
            }
        }

        /// <summary>
        /// Removes this enemy from the game world (called when defeated)
        /// </summary>
        public void RemoveFromGame()
        {
            if (occupiedTile != null)
            {
                occupiedTile.ClearEnemy();
            }

            Debug.Log($"Enemy on tile {occupiedTile?.CenterPosition} has been defeated and removed");
            Destroy(gameObject);
        }

        private void SetColor(Color color)
        {
            if (enemyMaterial != null)
            {
                // Try URP/Standard shader properties
                if (enemyMaterial.HasProperty("_BaseColor"))
                {
                    enemyMaterial.SetColor("_BaseColor", color);
                }
                else if (enemyMaterial.HasProperty("_Color"))
                {
                    enemyMaterial.color = color;
                }
            }
        }

        private void OnDestroy()
        {
            // Clean up tile reference when destroyed
            if (occupiedTile != null)
            {
                occupiedTile.ClearEnemy();
            }
        }
    }
}
