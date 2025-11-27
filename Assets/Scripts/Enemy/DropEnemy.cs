using UnityEngine;

public class DropEnemy : EnemyBase
{
    [Header("Drop Settings")]
    public GameObject[] dropItems;

    [Range(0f, 1f)]
    public float dropChance = 1f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 8f;

    [Header("Reposition Settings")]
    [Tooltip("Khoảng cách an toàn nó muốn giữ với player")]
    public float safeRadius = 6f;

    [Tooltip("Mỗi bao lâu thì chọn lại vị trí mới quanh player")]
    public float repositionInterval = 2f;

    [Tooltip("Khoảng cách coi như đã tới vị trí mục tiêu")]
    public float reachThreshold = 0.2f;

    private Vector3 currentMoveTarget;
    private float nextRepositionTime;

    protected override void Awake()
    {
        base.Awake();
        currentMoveTarget = transform.position;
        nextRepositionTime = Time.time + Random.Range(0f, repositionInterval);

        Debug.Log($"[DropEnemy {name}] Awake. safeRadius={safeRadius}, repositionInterval={repositionInterval}");
    }

    protected override void HandleBehaviour()
    {
        if (target == null)
        {
            return;
        }

        if (!IsTargetInRange(detectionRange))
        {
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, target.position);
        bool shouldReposition =
            Time.time >= nextRepositionTime ||
            Vector2.Distance(transform.position, currentMoveTarget) <= reachThreshold;

        if (shouldReposition)
        {
            PickNewPositionAroundPlayer();
            nextRepositionTime = Time.time + repositionInterval;
        }

        MoveTowardsPoint(currentMoveTarget);

        if (CanAttack() && IsTargetInRange(detectionRange))
        {
            FireProjectile();
            MarkJustAttacked();
        }
    }

    private void PickNewPositionAroundPlayer()
    {
        if (target == null) return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        Vector3 desiredPos = target.position + (Vector3)(dir * safeRadius);
        desiredPos.z = transform.position.z;

        currentMoveTarget = desiredPos;
    }

    private void MoveTowardsPoint(Vector3 point)
    {
        Vector2 dir = (point - transform.position).normalized;
        transform.position += (Vector3)dir * (moveSpeed * Time.deltaTime);

        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || target == null)
        {
            return;
        }

        Vector2 dir = (target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, rot);

        var proj = bulletObj.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.Init(dir, projectileSpeed, damage);
        }
        else
        {
            Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * projectileSpeed;
            }
        }

        if (anim != null)
            anim.SetTrigger("Attack");
    }

    protected override void Die()
    {
        TryDropItem();
        base.Die();
    }

    void TryDropItem()
    {
        if (dropItems == null || dropItems.Length == 0)
        {
            return;
        }

        if (Random.value <= dropChance)
        {
            GameObject prefab = dropItems[Random.Range(0, dropItems.Length)];
            Instantiate(prefab, transform.position, Quaternion.identity);
           
        }
        else
        {
            Debug.Log($"[DropEnemy {name}] Không rơi đồ (trượt tỉ lệ).");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, safeRadius);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentMoveTarget, 0.2f);
    }
}
