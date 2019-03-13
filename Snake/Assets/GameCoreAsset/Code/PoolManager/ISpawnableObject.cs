using UnityEngine;

namespace PoolManagerModule
{
    /// <summary>
    /// Interface for SpawnableObject. Each object in pool manager need behaviour, that realeases this interface
    /// </summary>
    public interface ISpawnableObject
    {
        /// <summary>
        /// Call it to get template, from which this object was generated
        /// </summary>
        /// <returns></returns>
        Transform GetTemplate();

        /// <summary>
        /// Call it to set template, from which this object was generated
        /// </summary>
        /// <param name="_template"></param>
        void SetTemplate(Transform _template);

        /// <summary>
        /// Call this, when you already installed object in your pool
        /// </summary>
        void SetInstalled();
    }
}