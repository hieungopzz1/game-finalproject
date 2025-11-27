using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 dir;
    private float speed;
    private float damage;
    public float lifeTime = 5f;   
    private Rigidbody2D rb;

    public void Init(Vector2 dir, float speed, float damage)
    {
        this.dir = dir.normalized;
        this.speed = speed;
        this.damage = damage;

        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = this.dir * this.speed;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
              //  Debug.Log("Enemy bullet hit player, dmg = " + damage);
            }

            Destroy(gameObject);
        }
    }
}
