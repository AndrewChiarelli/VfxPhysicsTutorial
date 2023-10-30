using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace AC.UnityExample
{
    public class SpawnPrefabRandomDirection : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool loop;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField, Range(0, 2)] private float rateInfluence = 1;
        [SerializeField, Range(0, 2)] private float timeToArriveInfluence = 0.9f;

        //Vfx variables
        private Vector2 lifeTime;
        private float randLifeTime;
        private Vector3 velocity, randVelocity;
        private float rate;

        //Script variables
        private float distance;
        private float timer;
        private float timeToNextRay;
        private RaycastHit hit;
        private float multiplier;

        private void Start()
        {
            Initialise();
            BurstParticles();
        }

        private void Initialise()
        {
            lifeTime = effect.GetVector2("Lifetime");
            velocity = effect.GetVector3("Velocity");
            rate = effect.GetFloat("Rate");
            timeToNextRay = 1 / (rate * rateInfluence);
            distance = ((lifeTime.x + lifeTime.y) / 2)  * velocity.magnitude;
        }
        
        private void BurstParticles()
        {
            if (loop)
                return;
            
            for (int i = 0; i < rate * rateInfluence; i++)
            {
                RaycastBlood();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!loop)
                return;
            
            timer += Time.deltaTime;

            if (timer >= timeToNextRay)
            {
                timer = 0;
                RaycastBlood();
            }
        }

        private void RaycastBlood()
        {
            randVelocity = new Vector3(GetRandomRange(), GetRandomRange(), GetRandomRange());

            if (Physics.Raycast(transform.position, randVelocity, out hit, distance, floorLayer))
            {
                StartCoroutine(DelaySpawn(GetTimeToArrive(Vector3.Distance(transform.position, hit.point), velocity.magnitude), 
                    hit.point + hit.normal * 0.01f,
                    Quaternion.LookRotation(hit.normal)));
            }
        }
        
        private float GetRandomRange()
        {
            return Random.Range(-1, 1);
        }
        
        private float GetTimeToArrive(float distance, float speed)
        {
            return distance / speed;
        }

        IEnumerator DelaySpawn(float timeToArrive, Vector3 position, Quaternion rotation)
        {
            yield return new WaitForSeconds(timeToArrive * timeToArriveInfluence);
            Instantiate(prefab, position, rotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + randVelocity * distance);
        }
    }
}