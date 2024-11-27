using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winTimeText;
    [SerializeField] private Button loseScreenRestartButton;
    [SerializeField] private Button winScreenRestartButton;
    [SerializeField] private float roundTime = 60f;

    private float roundTimeStart;
    private float elapsedTime;
    private bool isGameOver = false;
    private bool isPaused = false;
    private bool hasStarted = false;  

    private void Start()
    {
        roundTimeStart = roundTime;

        loseScreen.SetActive(false);
        winScreen.SetActive(false);

        loseScreenRestartButton.onClick.AddListener(RestartScene);
        winScreenRestartButton.onClick.AddListener(RestartScene);

        PlayerController.OnWin += HandleWin;
        PlayerController.OnDefeat += HandleDefeat;
        PlayerController.OnStart += StartTimer;  

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (hasStarted && !isGameOver)
        {
            if (!isPaused)
            {
                roundTime -= Time.deltaTime;
                timerText.text = "Time Left: " + Mathf.Max(0, roundTime).ToString("F2");

                if (roundTime <= 0)
                {
                    ShowLoseScreen();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartScene();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RestartScene();
            }
        }
    }

    private void StartTimer()
    {
        hasStarted = true;
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleWin()
    {
        isGameOver = true;
        winScreen.SetActive(true);
        elapsedTime = roundTimeStart - roundTime;
        winTimeText.text = "Your score: " + elapsedTime.ToString("F2") + " seconds";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleDefeat()
    {
        isGameOver = true;
        ShowLoseScreen();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
    }

    private void RestartScene()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        PlayerController.OnWin -= HandleWin;
        PlayerController.OnDefeat -= HandleDefeat;
        PlayerController.OnStart -= StartTimer;  }
}
