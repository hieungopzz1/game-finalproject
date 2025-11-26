using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    [Header("Settings")]
    public Image damageImage;
    public float flashSpeed = 5f; // Tốc độ mờ dần (càng cao càng nhanh hết)
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Màu đỏ, độ đậm 0.5

    private bool damaged = false;

    void Start()
    {
        if (damageImage == null)
            damageImage = GetComponent<Image>();
    }

    void Update()
    {
        if (damaged)
        {
            // Nếu vừa bị đánh -> Gán màu đỏ ngay lập tức
            damageImage.color = flashColor;
            damaged = false; // Reset cờ
        }
        else
        {
            // Mờ dần về trong suốt (Color.clear)
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    // Hàm này sẽ được Player gọi khi mất máu
    public void CallFlash()
    {
        damaged = true;
    }
}