
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class BossHealthBar : MonoBehaviour
{
    // Singleton: Để Boss có thể tìm thấy script này từ bất kỳ đâu
    public static BossHealthBar instance;

    [Header("UI References")]
    public Slider slider;
    public TMP_Text hpText; // Kéo Text % vào đây (nếu có)
    public Image fillImage; // Kéo ảnh Fill vào đây (để đổi màu)
    public Gradient healthGradient; // Chỉnh dải màu (Xanh -> Đỏ)

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Mặc định ẩn thanh máu đi khi game bắt đầu
        gameObject.SetActive(false);
    }

    // Hàm này Boss sẽ gọi khi vừa sinh ra (Start)
    public void SetBoss(EnemyBase boss)
    {
        // 1. Hiện thanh máu lên
        gameObject.SetActive(true);

        // 2. Reset slider về 100%
        slider.value = 1f;
        UpdateColor(1f);

        // 3. Đăng ký lắng nghe sự kiện mất máu của Boss
        boss.OnHealthChanged += UpdateHealth;

        // 4. Lắng nghe sự kiện Boss chết để ẩn thanh máu
        // (Lưu ý: Bạn cần đảm bảo EnemyBase có sự kiện OnAnyEnemyDied hoặc xử lý riêng)
        // Ở đây mình dùng cách đơn giản: Boss chết thì thanh máu tự ẩn trong hàm UpdateHealth nếu về 0
    }

    public void UpdateHealth(float pct)
    {
        slider.value = pct;

        // Cập nhật màu
        UpdateColor(pct);

        // Cập nhật Text
        if (hpText != null)
        {
            hpText.text = (pct * 100f).ToString("F0") + "%";
        }

        // Nếu hết máu thì ẩn thanh UI đi (hoặc delay một chút rồi ẩn)
        if (pct <= 0)
        {
            // Ẩn sau 2 giây để người chơi kịp nhìn thấy Boss chết
            Invoke(nameof(HideBar), 2f);
        }
    }

    void UpdateColor(float pct)
    {
        if (fillImage != null)
        {
            fillImage.color = healthGradient.Evaluate(pct);
        }
    }

    void HideBar()
    {
        gameObject.SetActive(false);
    }
}