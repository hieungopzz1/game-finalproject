using UnityEngine;

public class ShieldPulse : MonoBehaviour
{
    [Header("Cài đặt độ mờ")]
    [Range(0f, 1f)] public float minAlpha = 0.2f; // Mờ nhất (0 là tàng hình)
    [Range(0f, 1f)] public float maxAlpha = 0.7f; // Rõ nhất (1 là đặc)

    [Header("Tốc độ nháy")]
    public float pulseSpeed = 5f; // Càng cao nháy càng nhanh

    private SpriteRenderer sr;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            baseColor = sr.color; // Lưu lại màu gốc (ví dụ màu Xanh)
        }
    }

    void Update()
    {
        if (sr == null) return;

        // Công thức tạo nhịp thở (Sine Wave)
        // Mathf.Sin trả về giá trị từ -1 đến 1.
        // Ta chuyển đổi nó thành khoảng từ 0 đến 1 để dùng cho Lerp.
        float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        // Tính độ Alpha mới dựa trên wave
        float newAlpha = Mathf.Lerp(minAlpha, maxAlpha, wave);

        // Gán lại màu với Alpha mới
        Color newColor = baseColor;
        newColor.a = newAlpha;
        sr.color = newColor;
    }
}