using UnityEngine;

namespace AC.General
{
    public class Shoot : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private float maxDistance = 100;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float damage = 10;
        private RaycastHit rayHit;

        public void FireProjectile()
        {
            if(Physics.Raycast(camera.transform.position, camera.transform.forward, out rayHit, maxDistance, layerMask))
            {
                if(rayHit.transform.TryGetComponent(out IDamage enemy))
                    enemy.Damage(damage, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }
    }
}