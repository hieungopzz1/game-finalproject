using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    public float healAmount = 20f; // Hồi bao nhiêu máu?
    public float lifeTime = 3f;    // Tồn tại trong bao lâu?

    private float initialY;

    void Start()
    {
        initialY = transform.position.y;

        // Tự hủy sau 3 giây nếu không ai nhặt
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Hiệu ứng bay lên xuống nhè nhẹ (nhanh hơn súng chút cho sinh động)
        float newY = initialY + Mathf.Sin(Time.time * 3f) * 0.15f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có phải Player chạm vào không
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Gọi hàm hồi máu
                playerHealth.Heal(healAmount);

                // Ăn xong thì xóa item
                Destroy(gameObject);
            }
        }
    }
}