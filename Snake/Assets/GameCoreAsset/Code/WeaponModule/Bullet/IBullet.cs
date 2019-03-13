using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Interface of bullet 
    /// </summary>
    public interface IBullet
    {
        /// <summary>
        /// Call it to launch bullet
        /// </summary>
        void Launch();

        /// <summary>
        /// Call it to set bullet's data
        /// </summary>
        /// <param name="_data"></param>
        void SetData(BulletData _data);

        /// <summary>
        /// Call it to get transform of bullet
        /// </summary>
        /// <returns></returns>
        Transform GetTransform();

        /// <summary>
        /// Call it to get bullet's data
        /// </summary>
        /// <returns></returns>
        BulletData GetData(); 
    }

}