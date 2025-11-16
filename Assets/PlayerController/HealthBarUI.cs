using Combat;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlayerController
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider slider;

        [Header("Damage Flash")]
        [SerializeField] private CanvasGroup damageFlashCanvasGroup;
        [SerializeField] private float flashDuration = 0.2f;

        [Header("Camera Shake")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float shakeDuration = 0.15f;
        [SerializeField] private float shakeIntensity = 0.2f;

        [Header("Player")]
        [SerializeField] private string playerTag = "Player";

        private Health _targetHealth;
        private bool _subscribed;
        private float _lastHealth = -1f;

        private void Update()
        {
            // Buscar player cuando aparece
            if (!_targetHealth)
            {
                var player = GameObject.FindGameObjectWithTag(playerTag);
                if (player)
                {
                    _targetHealth = player.GetComponent<Health>();

                    // Buscar cámara automáticamente si no se asignó
                    if (!cameraTransform && Camera.main)
                        cameraTransform = Camera.main.transform;

                    if (_targetHealth && !_subscribed)
                    {
                        _subscribed = true;
                        _lastHealth = _targetHealth.CurrentHealth;
                        _targetHealth.OnHealthChanged += HandleHealthChanged;
                        HandleHealthChanged(_targetHealth.CurrentHealth, _targetHealth.MaxHealth);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_targetHealth && _subscribed)
            {
                _targetHealth.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            // Actualizar barra de vida
            if (slider)
                slider.value = max > 0 ? current / max : 0;

            // Detectar daño (vida bajó)
            if (_lastHealth > 0 && current < _lastHealth)
            {
                if (damageFlashCanvasGroup)
                    StartCoroutine(FlashCoroutine());

                if (cameraTransform)
                    StartCoroutine(ShakeCoroutine());
            }

            _lastHealth = current;
        }

        private IEnumerator FlashCoroutine()
        {
            damageFlashCanvasGroup.alpha = 1f;

            float t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float normalized = 1f - (t / flashDuration);
                damageFlashCanvasGroup.alpha = normalized;
                yield return null;
            }

            damageFlashCanvasGroup.alpha = 0f;
        }

        private IEnumerator ShakeCoroutine()
        {
            Vector3 originalPos = cameraTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float strength = shakeIntensity * (1f - (elapsed / shakeDuration));
                cameraTransform.localPosition = originalPos + (Random.insideUnitSphere * strength);
                yield return null;
            }

            cameraTransform.localPosition = originalPos;
        }
    }
}
