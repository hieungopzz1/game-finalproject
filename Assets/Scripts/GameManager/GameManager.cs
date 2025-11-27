using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public GameObject gameOverPanel; // Kéo cái Panel Game Over vào đây
    public GameObject victoryPanel; // 1. THÊM BIẾN NÀY

    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Hàm này sẽ được gọi khi Player chết
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Game Over!");

        // 1. Hiện bảng Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 2. Dừng game lại (đóng băng thời gian)
        Time.timeScale = 0f;
    }

    public void Victory()
    {
        if (isGameOver) return; // Nếu đã thắng/thua rồi thì thôi
        isGameOver = true;

        Debug.Log("Victory!");

        // Hiện bảng chiến thắng
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Dừng game lại (hoặc để chạy slow motion cho ngầu)
        Time.timeScale = 0f;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayVictoryMusic();
        }
    }
    // Gắn vào nút Restart
    public void RestartGame()
    {
        // Mở lại thời gian trước khi load
        Time.timeScale = 1f;
        // Load lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Gắn vào nút Menu
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Nhớ điền đúng tên Scene Menu
    }
}