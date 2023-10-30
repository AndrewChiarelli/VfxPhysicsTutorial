using UnityEngine;

namespace AC.General
{
    public class SpawnPrefab : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        
        public void SpawnObject(Vector3 position, Quaternion rotation)
        {
            Instantiate(prefab, position, rotation);
        }
    }
}