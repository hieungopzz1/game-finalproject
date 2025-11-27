using UnityEngine;

public class SpeedPicker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float speed = 1.5f;
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
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();

            if (playerCtrl != null)
            {
                // Gọi hàm hồi máu
                playerCtrl.SpeedUp(speed);

                // Ăn xong thì xóa item
                Destroy(gameObject);
            }
        }
    }
}
