using UnityEngine;

namespace AC.General
{
    public interface IDamage
    {
        void Damage(float damage, Vector3 position, Quaternion hitNormal);
    }
}