using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Danh sách súng trên người")]
    // Kéo tất cả súng (GameObject con của Player) vào đây
    public GameObject[] weapons;

    [Header("Súng mặc định")]
    public int startingWeaponIndex = 0;

    private void Start()
    {
        // Khi game bắt đầu, chỉ bật súng mặc định, tắt hết súng khác
        EquipWeapon(startingWeaponIndex);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogWarning("Index súng không hợp lệ!");
            return;
        }

        // 1. Tắt hết tất cả súng
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(false);
            }
        }

        // 2. Bật súng được chọn
        if (weapons[index] != null)
        {
            weapons[index].SetActive(true);
           // Debug.Log($"Đã đổi sang súng: {weapons[index].name}");
        }
    }

    // Hàm này để vật phẩm gọi
    public void PickupWeapon(string weaponName)
    {
        // Tìm súng theo tên
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].name == weaponName)
            {
                EquipWeapon(i);
                return;
            }
        }
        Debug.LogWarning($"Không tìm thấy súng tên là {weaponName} trên người Player!");
    }
}