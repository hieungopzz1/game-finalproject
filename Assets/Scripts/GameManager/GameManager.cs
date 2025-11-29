using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Phát nhạc Menu
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMenuMusic();
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // --- HÀM THUA ---
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Lưu điểm khi chết
        SaveScore();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- HÀM THẮNG (Cần sửa chỗ này) ---
    public void Victory()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Victory!");

        // --- THÊM DÒNG NÀY: Lưu điểm khi thắng ---
        SaveScore();
        // ----------------------------------------

        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- HÀM PHỤ ĐỂ GỌI LƯU ĐIỂM (Cho gọn code) ---
    private void SaveScore()
    {
        if (HighScoreManager.instance != null && WaveManager.instance != null)
        {
            // Lấy wave hiện tại
            int currentWave = WaveManager.instance.currentWaveIndex;

            // Gọi HighScoreManager để lưu
            HighScoreManager.instance.TrySaveHighScore(currentWave);
        }
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}