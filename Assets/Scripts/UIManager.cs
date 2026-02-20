using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelHeaderText;
    [SerializeField] private TextMeshProUGUI enemyCounterText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button previousLevelButton;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);

        if (previousLevelButton != null)
            previousLevelButton.onClick.AddListener(OnPreviousLevelClicked);

        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Playing)
        {
            UpdateEnemyCounter();
        }
        else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Won)
        {
            ShowWinScreen();
        }
        else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Lost)
        {
            ShowLoseScreen();
        }
    }

    private void UpdateUI()
    {
        if (levelManager == null) return;

        LevelData currentLevel = levelManager.CurrentLevel;
        if (currentLevel != null)
        {
            if (levelHeaderText != null)
            {
                levelHeaderText.text = $"Level {currentLevel.LevelNumber}: {currentLevel.LevelName}";
            }
        }

        UpdateEnemyCounter();
    }

    private void UpdateEnemyCounter()
    {
        if (enemyCounterText == null) return;

        int activeEnemies = GameManager.Instance.GetAllActiveEnemies().Count;
        enemyCounterText.text = $"Enemies: {activeEnemies}";
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private void ShowWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        ShowMessage("You Win!");
    }

    private void ShowLoseScreen()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
        ShowMessage("You were caught!");
    }

    private void HideWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    private void HideLoseScreen()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    private void OnResetClicked()
    {
        HideWinScreen();
        HideLoseScreen();
        GameManager.Instance.ResetLevel();
        if (levelManager != null)
        {
            levelManager.ReloadCurrentLevel();
        }
        UpdateUI();
    }

    private void OnNextLevelClicked()
    {
        HideWinScreen();
        HideLoseScreen();
        if (levelManager != null)
        {
            levelManager.NextLevel();
        }
        UpdateUI();
    }

    private void OnPreviousLevelClicked()
    {
        HideWinScreen();
        HideLoseScreen();
        if (levelManager != null)
        {
            levelManager.PreviousLevel();
        }
        UpdateUI();
    }
}
