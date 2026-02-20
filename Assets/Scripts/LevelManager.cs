using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject mudPrefab;
    [SerializeField] private GameObject goalPrefab;

    private int currentLevelIndex = 0;
    private GameObject currentPlayerInstance;
    private List<GameObject> currentEnemyInstances = new List<GameObject>();
    private List<GameObject> currentObstacleInstances = new List<GameObject>();
    private List<GameObject> currentMudInstances = new List<GameObject>();
    private GameObject currentGoalInstance;

    public int CurrentLevelIndex => currentLevelIndex;
    public int TotalLevels => levels.Count;
    public LevelData CurrentLevel => currentLevelIndex >= 0 && currentLevelIndex < levels.Count ? levels[currentLevelIndex] : null;

    private void Start()
    {
        if (levels.Count > 0)
        {
            LoadLevel(0);
        }
        else
        {
            Debug.LogError("No levels assigned to LevelManager!");
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError($"Level index {levelIndex} out of range!");
            return;
        }

        currentLevelIndex = levelIndex;
        ClearLevel();

        LevelData levelData = levels[levelIndex];
        Debug.Log($"Loading level: {levelData.LevelName}");

        // Setup GameManager
        GameManager.Instance.SetupLevel(levelData.PlayerStart, levelData.GoalPosition,
                                       levelData.Obstacles, levelData.MudPatches);

        // Instantiate player
        if (playerPrefab != null)
        {
            currentPlayerInstance = Instantiate(playerPrefab);
            PlayerController playerController = currentPlayerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPosition(levelData.PlayerStart);
            }
        }

        // Instantiate goal
        if (goalPrefab != null)
        {
            currentGoalInstance = Instantiate(goalPrefab,
                GridHelper.GridToWorld(levelData.GoalPosition), Quaternion.identity);
        }

        // Instantiate obstacles
        foreach (var obstaclePos in levelData.Obstacles)
        {
            if (obstaclePrefab != null)
            {
                GameObject obstacle = Instantiate(obstaclePrefab,
                    GridHelper.GridToWorld(obstaclePos), Quaternion.identity);
                currentObstacleInstances.Add(obstacle);
            }
        }

        // Instantiate mud patches
        foreach (var mudPos in levelData.MudPatches)
        {
            if (mudPrefab != null)
            {
                GameObject mud = Instantiate(mudPrefab,
                    GridHelper.GridToWorld(mudPos), Quaternion.identity);
                currentMudInstances.Add(mud);
            }
        }

        // Instantiate enemies
        foreach (var enemySpawn in levelData.Enemies)
        {
            if (enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab,
                    GridHelper.GridToWorld(enemySpawn.position), Quaternion.identity);

                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    // Set enemy properties
                    controller.gameObject.name = $"Enemy_{enemySpawn.type}";
                }

                currentEnemyInstances.Add(enemy);
            }
        }

        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
    }

    public void NextLevel()
    {
        if (currentLevelIndex < levels.Count - 1)
        {
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            LoadLevel(currentLevelIndex - 1);
        }
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    private void ClearLevel()
    {
        // Destroy player
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
        }

        // Destroy enemies
        foreach (var enemy in currentEnemyInstances)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        currentEnemyInstances.Clear();

        // Destroy obstacles
        foreach (var obstacle in currentObstacleInstances)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        currentObstacleInstances.Clear();

        // Destroy mud patches
        foreach (var mud in currentMudInstances)
        {
            if (mud != null)
                Destroy(mud);
        }
        currentMudInstances.Clear();

        // Destroy goal
        if (currentGoalInstance != null)
        {
            Destroy(currentGoalInstance);
        }
    }
}
