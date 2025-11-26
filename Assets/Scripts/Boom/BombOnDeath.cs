using UnityEngine;

public class BombOnDeath : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float fuseTime = 3f;          // tự nổ sau 3s
    public float explosionRadius = 2f;   // bán kính nổ
    public float explosionDamage = 10f;  // damage khi nổ
    public LayerMask targetLayers;       // layer chứa Player

    [Header("VFX")]
    public GameObject explosionVfxPrefab;

    private bool exploded = false;

    void Start()
    {
        // tự nổ sau fuseTime giây
        Invoke(nameof(Explode), fuseTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu player chạm → nổ ngay lập tức
        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return; // tránh nổ 2 lần
        exploded = true;

        // Spawn hiệu ứng nổ
        if (explosionVfxPrefab != null)
        {
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        // Gây damage
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayers);
        foreach (var hit in hits)
        {
            var hp = hit.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(explosionDamage);
            }
        }

        // Xóa bomb object
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
