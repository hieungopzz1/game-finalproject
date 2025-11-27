using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro; // Nhớ có thư viện này để dùng Text UI

public class GunRaycastLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private TMP_Text ammoText;
    [Header("Settings")]
    [SerializeField] private float fireDistance = 50f;
    [Header("Ammo & Reload")]
    public int maxAmmo = 30;         
    public float reloadTime = 2.0f;  
    private int currentAmmo;
    private bool isReloading = false;


    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return; 
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentAmmo > 0)
            {
                ShootRaycast();
            }
        }
    }

    void ShootRaycast()
    {
        currentAmmo--;
        UpdateAmmoUI();

        Vector2 direction = firePoint.right;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, fireDistance, hitLayer);

        Debug.DrawRay(firePoint.position, direction * fireDistance, Color.red, 0.1f);

        if (hit.collider != null)
        {
            var enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                float damage = Random.Range(5f, 7f);
                enemy.TakeDamage(damage);
            }
        }

        StartCoroutine(ShowMuzzleFlash());

        if (AudioManager.instance != null && shootSound != null)
        {
            AudioManager.instance.PlaySFX(shootSound);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (ammoText != null) ammoText.text = "...";

        if (AudioManager.instance != null && reloadSound != null)
        {
            AudioManager.instance.PlaySFX(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateAmmoUI();
    }

    IEnumerator ShowMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            muzzleFlash.SetActive(false);
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}