using UnityEngine;

namespace Combat
{
    public class DamageOnTouch : MonoBehaviour
    {
        [SerializeField] private float damagePerSecond = 10f;
        [SerializeField] private string targetTag = "Player";

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(targetTag)) return;

            var health = other.GetComponentInParent<Health>();
            if (health == null) return;

            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
