using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance;

    [Header("UI")]
    public TMP_Text highScoreText;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        UpdateHighScoreUI();
    }

    public void UpdateHighScoreUI()
    {
        // Khi hiển thị thì vẫn để mặc định là 0 để hiện "Best Wave: 1" cho đẹp
        int bestWave = PlayerPrefs.GetInt("BestWave", 0);

        if (highScoreText != null)
        {
            highScoreText.text = "BEST WAVE: " + (bestWave + 1);
        }
    }

    public void TrySaveHighScore(int currentWaveIndex)
    {
        // --- SỬA Ở ĐÂY: Đổi số 0 thành -1 ---
        // Tại sao? Để nếu kỷ lục đang trống, nó coi như là -1. 
        // Khi đó Wave 1 (index 0) > -1 sẽ kích hoạt lưu ngay lần đầu chơi.
        int oldRecord = PlayerPrefs.GetInt("BestWave", -1);

        // Debug để kiểm tra xem giá trị thực tế là bao nhiêu
        Debug.Log($"[Check HighScore] Current: {currentWaveIndex} | Old: {oldRecord}");

        if (currentWaveIndex > oldRecord)
        {
            PlayerPrefs.SetInt("BestWave", currentWaveIndex);
            PlayerPrefs.Save();

            Debug.Log("Đã lưu kỷ lục mới: Wave " + (currentWaveIndex + 1));

            UpdateHighScoreUI();
        }
        else
        {
            Debug.Log("Chưa phá kỷ lục (Hoặc bằng kỷ lục cũ).");
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("BestWave");
        UpdateHighScoreUI();
        Debug.Log("Đã Reset dữ liệu điểm cao!");
    }
}