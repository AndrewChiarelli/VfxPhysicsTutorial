using UnityEngine;
using UnityEngine.VFX;

namespace AC.UnityExample
{
    public class MoveVfxFloorCollider : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private bool updateFloorCollider;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private float raycastRate = 0.1f;
        [SerializeField] private float raycastDistance = 5f;
        private Vector3 size;
        private float timer;
        private RaycastHit raycastHit;
        
        private void Start()
        {
            RaycastFloor();
        }

        // Update is called once per frame
        void Update()
        {
            CheckToRaycast();
        }

        private void CheckToRaycast()
        {
            if (!updateFloorCollider)
                return;

            timer += Time.deltaTime;

            if (timer >= raycastRate)
            {
                timer = 0;
                RaycastFloor();
            }
        }
        
        private void RaycastFloor()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out raycastHit, raycastDistance, floorLayer))
            {
                effect.SetVector3("ColliderCenter", raycastHit.point);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
        }
    }
}