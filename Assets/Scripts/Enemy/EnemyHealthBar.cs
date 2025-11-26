using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public EnemyBase enemy;  // Kéo script Enemy (Melee/Ranged...) vào đây
    public Slider slider;    // Kéo component Slider vào đây
    public Transform targetToFollow; // (Tuỳ chọn) Nếu muốn thanh máu bay theo điểm cụ thể
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Độ cao của thanh máu so với quái


    void Start()
    {
        if (enemy == null)
            enemy = GetComponentInParent<EnemyBase>();

        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        // Đăng ký nhận sự kiện thay đổi máu
        if (enemy != null)
        {
            enemy.OnHealthChanged += UpdateHealthBar;
        }

        UpdateHealthBar(1f);
        // Ẩn thanh máu nếu chưa cần thiết (tuỳ chọn), hoặc để hiện luôn
        // gameObject.SetActive(true); 
    }

    void OnDestroy()
    {
        // Hủy đăng ký để tránh lỗi bộ nhớ
        if (enemy != null)
        {
            enemy.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void LateUpdate()
    {
        // 1. Giữ vị trí (nếu bạn để Canvas là con của Enemy thì không cần dòng này, 
        // nhưng nếu muốn thanh máu không bị rung lắc khi quái hoạt hình thì dùng dòng dưới)
        transform.position = enemy.transform.position + offset;

        // 2. Luôn quay mặt về phía Camera (Billboarding)
        // Giúp thanh máu không bị lật ngược khi quái quay đầu
        transform.rotation = Camera.main.transform.rotation;
    }

    void UpdateHealthBar(float pct)
    {
        if (slider != null)
        {
            slider.value = pct;
        }

    }
}