using UnityEngine;

namespace AC.General
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private Shoot shoot;
        
        // Update is called once per frame
        void Update()
        {
            HandleShootInput();
        }
        
        private void HandleShootInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                shoot.FireProjectile();
            }
        }
    }
}