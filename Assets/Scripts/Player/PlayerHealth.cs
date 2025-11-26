using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]private Animator ani;
    public float maxHealth = 10f;
    [SerializeField]private float currentHealth;
    public event Action<float> OnHealthChanged;
    public DamageFlash damageFlash;
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (damageFlash != null)
        {
            damageFlash.CallFlash();
        }

        if (DamagePopupManager.Instance != null)
        {
            DamagePopupManager.Instance.ShowDamage(
                amount,
                transform.position + Vector3.up * 0.8f
            );
        }
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        Debug.Log("Player Health: " + currentHealth);
        ani.SetTrigger("Hit");
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        this.gameObject.SetActive(false);
    }
}
