using UnityEngine;

public class MeleeEnemy : EnemyBase
{
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
}
