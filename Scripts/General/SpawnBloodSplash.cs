using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace AC.General
{
    public class SpawnBloodSplash : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private GameObject bloodPrefab;
        [SerializeField] private bool loop;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField, Range(0, 2)] private float gravityInfluence = 0.7f;
        [SerializeField, Range(0, 2)] private float rateInfluence = 1;
        [SerializeField, Range(0, 2)] private float timeToArriveInfluence = 0.9f;
        [SerializeField, Range(0, 5)] private float spread = 1f;
        [SerializeField] private float addedTime = 0.2f;
        [SerializeField] private int stepCount = 10;
        [SerializeField] private float timeStep = 0.2f;
        [SerializeField] private int indexSubtractOnHit = 2;

        //Vfx variables
        private Vector2 lifeTime;
        private float randLifeTime;
        private Vector3 velocity, randVelocity;
        private float gravity;
        private float rate;

        //Script variables
        private float distance;
        private float timer;
        private float timeToNextRay;
        private RaycastHit hit;
        private float multiplier;
        private Collider[] colliders = new Collider[1];
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
            timeToNextRay = 1 / (rate * rateInfluence);
            distance = ((lifeTime.x + lifeTime.y) / 2)  * velocity.magnitude;
        }
        
        private void BurstParticles()
        {
            if (loop)
                return;
            
            index = 0;
            
            for (int i = 0; i < rate * rateInfluence; i++)
            {
                RaycastBlood(index);
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
                RaycastBlood(0);
            }
        }

        private void RaycastBlood(int startingIndex)
        {
            //Method1();
            Method2(startingIndex);
        }
        
        private void Method1()
        {
            float dot = Vector3.Dot(Vector3.up, transform.forward);
            float time = 0;
            
            if(dot > 0 && gravityInfluence != 0 && gravity != 0)
            {
                randVelocity = new Vector3(
                    Random.Range(-velocity.x, velocity.x) * spread, 
                    GetRandomRange(velocity.y), 
                    Mathf.Lerp(GetRandomRange(velocity.z), 0, dot / 2));
                time = Mathf.Lerp(0, addedTime, dot);
            }
            else
            {
                randVelocity = new Vector3(Random.Range(-velocity.x, velocity.x) * spread, 
                    GetRandomRange(velocity.y), 
                    GetRandomRange(velocity.z));
            }
     
            randVelocity = transform.TransformDirection(randVelocity);
            randVelocity += Vector3.up * (gravity * gravityInfluence);
            
            if (Physics.Raycast(transform.position, randVelocity, out hit, distance, floorLayer))
            {
                StartCoroutine(DelaySpawn(GetTimeToArrive(Vector3.Distance(transform.position, hit.point) + time, velocity.magnitude), 
                    hit.point + (Quaternion.LookRotation(hit.normal) * Vector3.forward) * 0.01f,
                    Quaternion.LookRotation(hit.normal)));
            }
        }
        
        private void Method2(int startingIndex)
        {
            randVelocity = new Vector3(Random.Range(-velocity.x, velocity.x) * spread, 
                GetRandomRange(velocity.y), 
                GetRandomRange(velocity.z));
            
            randVelocity = transform.TransformDirection(randVelocity);

            float t = 0;
            Vector3 pos = Vector3.zero;
            Vector3 curPos = transform.position;

            for (int i = startingIndex; i < stepCount && t < lifeTime.y; i++)
            {
                t = i * timeStep;
                pos = curPos + t * randVelocity;
                pos.y = curPos.y + randVelocity.y * t + gravity / 2 * t * t;

                if(Physics.Raycast(curPos, (pos - curPos).normalized, out hit, t * velocity.magnitude, floorLayer))
                {
                    index = Mathf.Clamp(i - indexSubtractOnHit, 0, 999);
                    randVelocity = (pos - curPos).normalized;

                    StartCoroutine(DelaySpawn(t * timeToArriveInfluence, 
                        hit.point + hit.normal * 0.01f,
                        Quaternion.LookRotation(hit.normal)));

                    Debug.Log(i);
                    break;
                }
            }
        }

        private float GetRandomRange(float f)
        {
            return Random.Range(f, f * multiplier);
        }
        
        private float GetTimeToArrive(float distance, float speed)
        {
            float gspeed = 0;
            
            if(gravity > 0)
                gspeed = distance * 2 / Mathf.Abs(gravity);
            
            return distance / (speed + gspeed);
        }

        IEnumerator DelaySpawn(float timeToArrive, Vector3 position, Quaternion rotation)
        {
            yield return new WaitForSeconds(timeToArrive);
            Instantiate(bloodPrefab, position, rotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + randVelocity * distance);
        }
    }
}