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

    public void Heal(float amount)
    {
        if (currentHealth >= maxHealth) return; // Đầy máu rồi thì thôi

        currentHealth += amount;

        // Không cho vượt quá maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Cập nhật UI (Thanh máu xanh lại)
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        Debug.Log($"Đã hồi {amount} máu. HP hiện tại: {currentHealth}");
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
