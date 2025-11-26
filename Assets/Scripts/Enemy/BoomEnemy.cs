using UnityEngine;

public class BoomEnemy : EnemyBase
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;

    protected override void HandleBehaviour()
    {
        if (!IsTargetInRange(detectionRange))
        {
            return;
        }

        if (!IsTargetInRange(attackRange))
        {
            MoveTowardsTarget();
            return;
        }


        if (!CanAttack()) return;

        anim.SetTrigger("Attack");
        DoMeleeHit();
        MarkJustAttacked();
    }

    public void DoMeleeHit()
    {
        if (IsTargetInRange(attackRange + 0.2f))
        {
            DamagePlayer();
        }
    }

    protected override void Die()
    {
        if (bombPrefab != null)
        {
            Instantiate(bombPrefab, transform.position, Quaternion.identity);
        }

        base.Die();
    }
}
