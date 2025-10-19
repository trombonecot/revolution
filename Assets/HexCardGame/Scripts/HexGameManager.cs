using UnityEngine;

namespace HexGame
{
    /// <summary>
    /// Manages the hex game logic including unit selection and movement
    /// </summary>
    public class HexGameManager : MonoBehaviour
    {
        private static HexGameManager instance;
        public static HexGameManager Instance => instance;

        private Unit selectedUnit = null;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Selects a unit and deselects any previously selected unit
        /// </summary>
        /// <param name="unit">The unit to select</param>
        public void SelectUnit(Unit unit)
        {
            // Deselect previous unit if any
            if (selectedUnit != null)
            {
                selectedUnit.Deselect();
            }

            // Select new unit
            selectedUnit = unit;
            selectedUnit.Select();

            Debug.Log($"Unit selected: {unit.gameObject.name}");
        }

        /// <summary>
        /// Deselects the currently selected unit
        /// </summary>
        public void DeselectUnit()
        {
            if (selectedUnit != null)
            {
                selectedUnit.Deselect();
                selectedUnit = null;
                Debug.Log("Unit deselected");
            }
        }

        /// <summary>
        /// Checks if there is a unit currently selected
        /// </summary>
        /// <returns>True if a unit is selected, false otherwise</returns>
        public bool HasSelectedUnit()
        {
            return selectedUnit != null;
        }

        /// <summary>
        /// Moves the selected unit to the specified hex tile
        /// </summary>
        /// <param name="hex">The hex tile to move to</param>
        public void MoveUnitToHex(HexTile hex)
        {
            if (selectedUnit == null)
            {
                Debug.LogWarning("No unit selected for movement");
                return;
            }

            Debug.Log($"<color=cyan>MoveUnitToHex called for tile at {hex.CenterPosition}</color>");
            Debug.Log($"<color=cyan>Hex.HasEnemy = {hex.HasEnemy}, Hex.EnemyOnTile = {hex.EnemyOnTile?.gameObject.name ?? "null"}</color>");

            // Check if the hex has an enemy
            if (hex.HasEnemy)
            {
                Debug.Log($"<color=yellow>ENEMY DETECTED on hex at {hex.CenterPosition}! Initiating combat...</color>");

                // Trigger combat instead of moving
                if (CombatTransitionManager.Instance != null)
                {
                    Debug.Log("<color=green>CombatTransitionManager found! Starting combat...</color>");
                    CombatTransitionManager.Instance.StartCombat(
                        selectedUnit,
                        hex.EnemyOnTile,
                        hex.CenterPosition,
                        selectedUnit.transform.position
                    );
                }
                else
                {
                    Debug.LogError("<color=red>CombatTransitionManager not found in scene! Please add it to the scene.</color>");
                }

                // Clear selection
                selectedUnit = null;
            }
            else
            {
                Debug.Log($"<color=white>No enemy on tile. Moving {selectedUnit.gameObject.name} to hex at {hex.CenterPosition}</color>");

                // Initiate movement (no enemy on tile)
                selectedUnit.MoveTo(hex.CenterPosition);

                // Clear selection (unit will be deselected in Unit.MoveTo)
                selectedUnit = null;
            }
        }

        /// <summary>
        /// Called when a unit completes its movement
        /// </summary>
        public void OnUnitMovementComplete()
        {
            Debug.Log("Unit movement complete - ready for new selection");
        }
    }
}
