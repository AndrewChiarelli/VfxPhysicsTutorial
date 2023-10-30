using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace AC.UnityExample
{
    public class VfxPhysicsSpawnPrefab : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private GameObject prefab;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField, Range(0, 5)] private float spread = 1f;
        [SerializeField, Range(0, 2)] private float rateInfluence = 1;
        [SerializeField, Range(0, 2)] private float timeToArriveInfluence = 0.9f;
        [SerializeField] private int stepCount = 10;
        [SerializeField] private float timeStep = 0.2f;
        [SerializeField] private int indexSubtractOnHit = 2;
        
        //Vfx variables
        private Vector2 lifeTime;
        private Vector3 velocity;
        private float gravity;
        private float rate;
        
        //Script variables
        private RaycastHit hit;
        private float multiplier;
        private int index;

        private void Start()
        {
            Initialise();
            BurstParticles();
        }

        private void Initialise()
        {
            lifeTime = effect.GetVector2("Lifetime");
            velocity = effect.GetVector3("Velocity");
            gravity = effect.GetVector3("Gravity").y;
            rate = effect.GetFloat("Rate");
            multiplier = effect.GetFloat("Multiplier");
        }
        
        private void BurstParticles()
        {
            index = 0;
            
            for (int i = 0; i < rate * rateInfluence; i++)
            {
                Raycast(index);
            }
        }

        private void Raycast(int startingIndex)
        {
            float t = 0;
            Vector3 targetPos = Vector3.zero;
            Vector3 curPos = transform.position;
            Vector3 randVelocity;
            
            randVelocity = new Vector3(Random.Range(-velocity.x, velocity.x) * spread, 
                GetRandomRange(velocity.y), 
                GetRandomRange(velocity.z));
            
            randVelocity = transform.TransformDirection(randVelocity);

            for (int i = startingIndex; i < stepCount && t < lifeTime.y; i++)
            {
                t = i * timeStep;
                targetPos = GetLookAheadPosition(t, curPos, randVelocity);

                if(Physics.Raycast(curPos, GetDirectionFromTarget(curPos, targetPos), out hit, GetDistance(t, velocity.magnitude), floorLayer))
                {
                    index = Mathf.Clamp(i - indexSubtractOnHit, 0, 999);

                    StartCoroutine(DelaySpawnCoroutine(t * timeToArriveInfluence, 
                        hit.point + hit.normal * 0.01f,
                        Quaternion.LookRotation(hit.normal)));
                    
                    break;
                }
            }
        }
        
        private float GetDistance(float time, float speed)
        {
            return time * speed;
        }
        
        private Vector3 GetLookAheadPosition(float t, Vector3 curPos, Vector3 currentVelocity)
        {
            Vector3 targetPos;

            targetPos = curPos + t * currentVelocity;
            targetPos.y = curPos.y + currentVelocity.y * t + gravity / 2 * t * t;

            return targetPos;
        }
        
        private Vector3 GetDirectionFromTarget(Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }
        
        private float GetRandomRange(float f)
        {
            return Random.Range(f, f * multiplier);
        }

        private IEnumerator DelaySpawnCoroutine(float timeToArrive, Vector3 position, Quaternion rotation)
        {
            yield return new WaitForSeconds(timeToArrive);
            Instantiate(prefab, position, rotation);
        }
    }
}