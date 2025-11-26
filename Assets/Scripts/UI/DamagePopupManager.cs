using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    public DamagePopup damagePopupPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Nếu muốn manager sống qua scene khác:
        // DontDestroyOnLoad(gameObject);
    }

    public void ShowDamage(float amount, Vector3 worldPos)
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogWarning("DamagePopupManager: Chưa gán prefab!");
            return;
        }

        DamagePopup popup = Instantiate(damagePopupPrefab, worldPos, Quaternion.identity);
        popup.Setup(amount);
    }
}
