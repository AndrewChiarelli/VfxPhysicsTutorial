using UnityEngine;
using UnityEngine.Events;

namespace AC.General
{
    public class Health : MonoBehaviour, IDamage
    {
        [SerializeField] private float maxHealth = 1000;
        private float currentHealth;
        
        public UnityEvent<Vector3, Quaternion> OnDamageTaken;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void Damage(float damage, Vector3 position, Quaternion hitNormal)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            OnDamageTaken?.Invoke(position, hitNormal);
        }
    }
}