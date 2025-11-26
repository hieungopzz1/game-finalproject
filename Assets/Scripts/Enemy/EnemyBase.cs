using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    public event Action<float> OnHealthChanged;
    public static Action<EnemyBase> OnAnyEnemyDied;

    [Header("Stats")]
    public float maxHealth = 10f;
    public float damage = 1f;
    public float moveSpeed = 2f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("References")]
    public Transform target;
    public Animator anim;

    [Header("Damage / Invulnerable")]
    public bool startInvulnerable = false;

    protected float currentHealth;
    protected float lastAttackTime;
    protected bool isDead;
    protected bool canTakeDamage = true;   

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        if (anim == null)
            anim = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        canTakeDamage = !startInvulnerable;

    }

    protected virtual void Start()
    {
        // Gửi sự kiện lần đầu để thanh máu cập nhật trạng thái đầy (100%)
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    protected virtual void Update()
    {
        if (isDead || target == null) return;
        HandleBehaviour();
    }

    protected abstract void HandleBehaviour();

    protected bool IsTargetInRange(float range)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= range;
    }

    protected void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * (moveSpeed * Time.deltaTime);

        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

    }



    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        if (!canTakeDamage) return;  

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (DamagePopupManager.Instance != null)
        {
            DamagePopupManager.Instance.ShowDamage(
                amount,
                transform.position + Vector3.up * 0.8f 
            );
        }
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (anim != null)
            anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnAnyEnemyDied?.Invoke(this);

        if (anim != null)
            anim.SetTrigger("Die");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.5f);
    }



    protected bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    protected void MarkJustAttacked()
    {
        lastAttackTime = Time.time;
    }

    protected void DamagePlayer()
    {
        if (target == null)
        {
            Debug.LogWarning($"{name}: target == null, không đánh được player");
            return;
        }

        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log($"{name} gây {damage} dame lên player");
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning($"{name}: target không có PlayerHealth");
        }
    }

    public void SetInvulnerable(bool value)
    {
        canTakeDamage = !value;
    }

}
