using Combat;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerController
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private string playerTag = "Player";

        private Health _targetHealth;
        private bool _subscribed;

        private void Update()
        {
            // Si aún no tenemos referencia al player (se instancia después del countdown)
            if (!_targetHealth)
            {
                var player = GameObject.FindGameObjectWithTag(playerTag);
                if (player)
                {
                    _targetHealth = player.GetComponent<Health>();
                    if (_targetHealth && !_subscribed)
                    {
                        _targetHealth.OnHealthChanged += HandleHealthChanged;
                        _subscribed = true;
                        HandleHealthChanged(_targetHealth.CurrentHealth, _targetHealth.MaxHealth);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_targetHealth != null && _subscribed)
            {
                _targetHealth.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (!slider) return;

            slider.value = max > 0f ? current / max : 0f;
        }
    }
}