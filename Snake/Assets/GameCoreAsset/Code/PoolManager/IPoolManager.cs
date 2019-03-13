using UnityEngine;

namespace PoolManagerModule
{
    /// <summary>
    /// Interface for pool manager. 
    /// </summary>
    public interface IPoolManager
    {
        Transform Spawn(Transform _template, Vector3 _pos, Quaternion _rot, Transform _parent);
        Transform SpawnNow(Transform _template, Vector3 _pos, Quaternion _rot, Transform _parent);
        void Despawn(Transform _obj, float _time);
        void DespawnForever(Transform _obj, float _time);
    }
}
