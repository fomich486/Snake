using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class of bullet characteristics
    /// </summary>
    [Serializable]
    public class BulletData
    {
        /// <summary>
        /// Transform of bullet
        /// </summary>
        public Transform BulletTransform;
        /// <summary>
        /// Bullet flying speed
        /// </summary>
        public float Speed;
        /// <summary>
        /// Bullet type
        /// </summary>
        public BulletType Type;
        /// <summary>
        /// Damage of bullet
        /// </summary>
        public float BulletDamage;
        /// <summary>
        /// Rarius of explosion(for object hitting)
        /// </summary>
        public float ExplosionRadius;
        /// <summary>
        /// Lifetime of bullet
        /// </summary>
        public float LifeTime = 5f;
        /// <summary>
        /// Flag if bullet have to destroy after lifetime ending
        /// </summary>
        public bool EndDestroy = true;
        /// <summary>
        /// Owner of bullet (who launched it)
        /// </summary>
        public Transform Owner;
    } 
}