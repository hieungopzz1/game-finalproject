using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu muốn hiện số máu dạng 100/100

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public TMP_Text hpText; // (Tuỳ chọn) Kéo Text vào nếu muốn hiện số

    private PlayerHealth playerHealth;

    void Start()
    {
        // Tự động tìm Player trong màn chơi
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Đăng ký nhận tin nhắn
            playerHealth.OnHealthChanged += UpdateHealthBar;

            // Cập nhật ngay lập tức (để tránh bị 0% lúc đầu)
            UpdateHealthBar(1f);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player hoặc PlayerHealth script!");
        }
    }

    void OnDestroy()
    {
        // Hủy đăng ký khi chuyển màn hoặc UI bị hủy
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void UpdateHealthBar(float pct)
    {
        if (healthSlider != null)
        {
            healthSlider.value = pct;
        }

        if (hpText != null)
        {
            // Hiện thị kiểu phần trăm (VD: 80%)
            hpText.text = (pct * 100f).ToString("F0") + "%";

            // Hoặc hiển thị kiểu số thực tế nếu bạn muốn logic phức tạp hơn
            // (nhưng ở đây mình dùng pct nên để % là tiện nhất)
        }
    }
}