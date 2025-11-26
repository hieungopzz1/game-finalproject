using UnityEngine;
using UnityEngine.InputSystem;

public class GunAim : MonoBehaviour
{
    public Transform gunPivot;   
    public Transform body;      

    void Update()
    {
        RotateGunToMouse();
    }

    private void RotateGunToMouse()
    {
        if (gunPivot == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector2 dir = (worldPos - gunPivot.position);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        gunPivot.rotation = Quaternion.Euler(0, 0, angle);

        if (worldPos.x < gunPivot.position.x)
        {
            gunPivot.localScale = new Vector3(1, -1, 1);  
        }
        else
        {
            gunPivot.localScale = new Vector3(1, 1, 1);    
        }
    }
}
