using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HexGame
{
    /// <summary>
    /// Manages transitions between the hex game and card combat scenes
    /// Persists across scene loads to maintain combat state
    /// </summary>
    public class CombatTransitionManager : MonoBehaviour
    {
        private static CombatTransitionManager instance;
        public static CombatTransitionManager Instance => instance;

        [Header("Scene Names")]
        [SerializeField] private string hexGameSceneName = "HexGame";
        [SerializeField] private string combatSceneName = "CardGame";

        // Combat state data
        private Enemy currentEnemy;
        private Unit playerUnit;
        private Vector3 targetHexPosition;
        private Vector3 previousHexPosition;
        private Vector3 enemyPosition; // Store enemy position before scene change
        private bool combatVictory = false;

        // Player data to transfer
        private int playerHP;
        private int playerMaxHP;
        private List<HexCard> playerFullDeck;
        private List<HexCard> playerSelectedCards;
        private int playerHPAfterCombat;

        // Enemy data to transfer
        private int enemyLevel;
        private int enemyMaxHP;

        public Enemy CurrentEnemy => currentEnemy;
        public bool CombatVictory => combatVictory;
        public int PlayerHP => playerHP;
        public int PlayerMaxHP => playerMaxHP;
        public List<HexCard> PlayerFullDeck => playerFullDeck;
        public List<HexCard> PlayerSelectedCards => playerSelectedCards;
        public int PlayerHPAfterCombat { get => playerHPAfterCombat; set => playerHPAfterCombat = value; }
        public int EnemyLevel => enemyLevel;
        public int EnemyMaxHP => enemyMaxHP;

        private void Awake()
        {
            // Singleton pattern with DontDestroyOnLoad
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initiates combat by storing the combat context and loading the combat scene
        /// </summary>
        /// <param name="unit">The player unit entering combat</param>
        /// <param name="enemy">The enemy to fight</param>
        /// <param name="targetPos">The hex position the unit is trying to reach</param>
        /// <param name="previousPos">The hex position the unit came from</param>
        public void StartCombat(Unit unit, Enemy enemy, Vector3 targetPos, Vector3 previousPos)
        {
            playerUnit = unit;
            currentEnemy = enemy;
            targetHexPosition = targetPos;
            previousHexPosition = previousPos;
            combatVictory = false;

            // Get Player component from the Unit to extract HP and cards
            Player playerComponent = unit.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerHP = playerComponent.CurrentHP;
                playerMaxHP = playerComponent.maxHP;
                playerFullDeck = new List<HexCard>(playerComponent.fullDeck);
                playerSelectedCards = new List<HexCard>(playerComponent.SelectedCombatCards);
            }
            else
            {
                Debug.LogError("<color=red>Unit does not have a Player component! HP and cards will not transfer.</color>");
                playerHP = 20;
                playerMaxHP = 20;
                playerFullDeck = new List<HexCard>();
                playerSelectedCards = new List<HexCard>();
            }

            // Extract enemy stats and position before scene change (enemy reference will be destroyed)
            enemyLevel = enemy.EnemyLevel;
            enemyMaxHP = enemy.MaxHP;
            enemyPosition = enemy.transform.position;

            Debug.Log($"<color=green>===== COMBAT TRANSITION START =====</color>");
            Debug.Log($"<color=green>Player: HP={playerHP}/{playerMaxHP}, FullDeck={playerFullDeck.Count} cards, SelectedCards={playerSelectedCards.Count}</color>");
            Debug.Log($"<color=green>Enemy: Level={enemyLevel}, MaxHP={enemyMaxHP}, Position={enemyPosition}</color>");
            Debug.Log($"<color=green>Stored in CombatTransitionManager: EnemyLevel={this.enemyLevel}, EnemyMaxHP={this.enemyMaxHP}</color>");
            Debug.Log($"<color=green>====================================</color>");

            // Clear enemy reference (will be destroyed when scene changes)
            currentEnemy = null;

            // Load combat scene
            SceneManager.LoadScene(combatSceneName);
        }

        /// <summary>
        /// Called by the combat scene when combat ends
        /// </summary>
        /// <param name="playerWon">True if player achieved complete victory</param>
        public void EndCombat(bool playerWon)
        {
            combatVictory = playerWon;

            Debug.Log($"Combat ended. Player won: {playerWon}");

            // Return to hex game scene
            SceneManager.sceneLoaded += OnHexGameSceneLoaded;
            SceneManager.LoadScene(hexGameSceneName);
        }

        /// <summary>
        /// Called when the hex game scene finishes loading after combat
        /// </summary>
        private void OnHexGameSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnHexGameSceneLoaded;

            // Wait one frame for scene initialization
            StartCoroutine(ApplyCombatResultsCoroutine());
        }

        /// <summary>
        /// Applies combat results to the hex game
        /// </summary>
        private System.Collections.IEnumerator ApplyCombatResultsCoroutine()
        {
            // Wait for scene to fully initialize
            yield return null;

            // Find the unit and enemy in the reloaded scene
            Unit[] units = FindObjectsOfType<Unit>();
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            Unit reloadedUnit = null;
            Enemy reloadedEnemy = null;

            // Find matching unit by position (approximate)
            foreach (Unit u in units)
            {
                if (Vector3.Distance(u.transform.position, previousHexPosition) < 0.5f ||
                    Vector3.Distance(u.transform.position, targetHexPosition) < 0.5f)
                {
                    reloadedUnit = u;
                    break;
                }
            }

            // Find matching enemy by position (use stored position, not destroyed reference)
            foreach (Enemy e in enemies)
            {
                if (Vector3.Distance(e.transform.position, enemyPosition) < 0.5f)
                {
                    reloadedEnemy = e;
                    break;
                }
            }

            // Apply combat results
            if (combatVictory)
            {
                // Player won - move to target position and remove enemy
                if (reloadedUnit != null)
                {
                    reloadedUnit.transform.position = targetHexPosition;
                    Debug.Log($"Unit moved to target position: {targetHexPosition}");

                    // Update HP after combat
                    Player playerComponent = reloadedUnit.GetComponent<Player>();
                    if (playerComponent != null)
                    {
                        playerComponent.CurrentHP = playerHPAfterCombat;
                        Debug.Log($"<color=green>Unit HP updated to {playerHPAfterCombat}/{playerComponent.maxHP}</color>");
                    }
                }

                if (reloadedEnemy != null)
                {
                    reloadedEnemy.RemoveFromGame();
                    Debug.Log("Enemy defeated and removed from hex game");
                }
            }
            else
            {
                // Player lost or partial victory - return to previous position
                if (reloadedUnit != null)
                {
                    reloadedUnit.transform.position = previousHexPosition;
                    Debug.Log($"Unit returned to previous position: {previousHexPosition}");

                    // Update HP after combat (even if lost)
                    Player playerComponent = reloadedUnit.GetComponent<Player>();
                    if (playerComponent != null)
                    {
                        playerComponent.CurrentHP = playerHPAfterCombat;
                        Debug.Log($"<color=orange>Unit HP updated to {playerHPAfterCombat}/{playerComponent.maxHP} (combat lost)</color>");
                    }
                }
            }

            // Clear combat state
            currentEnemy = null;
            playerUnit = null;
        }
    }
}
