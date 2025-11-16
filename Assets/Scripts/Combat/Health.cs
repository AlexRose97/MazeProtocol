using System;
using UnityEngine;

namespace Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath = false;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            NotifyHealthChanged();
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || CurrentHealth <= 0f) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            NotifyHealthChanged();

            if (CurrentHealth <= 0f)
            {
                HandleDeath();
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || CurrentHealth <= 0f) return;

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            NotifyHealthChanged();
        }

        private void HandleDeath()
        {
            OnDeath?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        private void NotifyHealthChanged()
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}