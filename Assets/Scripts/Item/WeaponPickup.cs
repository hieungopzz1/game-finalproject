using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Tooltip("Tên này PHẢI GIỐNG HỆT tên GameObject súng gắn trên người Player")]
    public string weaponNameTarget = "MachineGun";
    public float lifeTime = 3f;

    // Hiệu ứng bay bay cho đẹp (Copy từ RangedEnemy của bạn)
    private float initialY;

    void Start()
    {
        initialY = transform.position.y;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Làm vật phẩm nhấp nhô cho dễ nhìn
        float newY = initialY + Mathf.Sin(Time.time * 2f) * 0.2f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Tìm script quản lý súng trên người Player
            var weaponManager = other.GetComponent<PlayerWeaponManager>();

            if (weaponManager != null)
            {
                // 2. Yêu cầu đổi súng
                weaponManager.PickupWeapon(weaponNameTarget);

                // 3. Xóa vật phẩm dưới đất đi
                Debug.Log("Nhặt được súng: " + weaponNameTarget);
                Destroy(gameObject);
            }
        }
    }
}