using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolManagerModule;
using SFXManagerModule;
using DataSystem; 

namespace WeaponSystem
{
    /// <summary>
    /// Class that performs mine logic
    /// </summary>
    public class Mine : ActiveObject, IBullet
    {
        #region VARIABLES
        /// <summary>
        /// Template of prefab
        /// </summary>
        [SerializeField]
        private Transform template;
        /// <summary>
        /// Bullet data with info
        /// </summary>
        [SerializeField]
        public BulletData data;
        /// <summary>
        /// Current state of activity
        /// </summary>
        private State curState = State.Idle;
        /// <summary>
        /// List of hit sounds
        /// </summary>
        [SerializeField]
        private AudioClip[] hitSound;
        /// <summary>
        /// List of hit FXs
        /// </summary>
        [SerializeField]
        private Transform[] hitFx;
        /// <summary>
        /// LayerMask for raycast
        /// </summary>
        [SerializeField]
        private LayerMask raycastMask;
        /// <summary>
        /// Multiplier of current plane speed. 1 == full plane speed, 
        /// </summary>
        [SerializeField]
        private float forwardSpeedMultiplier = 0.3f;
        /// <summary>
        /// Speed of plane in the start of moving
        /// </summary>
        [SerializeField]
        private float currentPlaneSpeed = 10f;
        /// <summary>
        /// Current move direction
        /// </summary>
        private Vector3 moveDirection = Vector3.zero;
        /// <summary>
        /// lifetime of bobm
        /// </summary>
        private float lifeTime = 1f;
        /// <summary>
        /// GI const (base acceleration) of the bomb
        /// </summary>
        public float GiConst = 20f;
        /// <summary>
        /// Master's tag
        /// </summary>
        private string creatorTag = "default"; 
        /// <summary>
        /// Cashed start damage
        /// </summary>
        private int baseDamage;
        #endregion

        #region METHODS 
        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            base.Update();
            if (transform.position.z < -400f)
            {
                Despawn();
            }
        } 

        /// <summary>
        /// Call it to launch mine
        /// </summary>
        public void Launch()
        {
            //nothing to do
        }

        /// <summary>
        /// Call it to set data
        /// </summary>
        /// <param name="_data"></param>
        public void SetData(BulletData _data)
        {
            data = _data;
        }

        /// <summary>
        /// Call it to get transform
        /// </summary>
        /// <returns></returns>
        public Transform GetTransform()
        {
            return transform;
        }

        /// <summary>
        /// Call it to get data
        /// </summary>
        /// <returns></returns>
        public BulletData GetData()
        {
            return data;
        }

        /// <summary>
        /// Called when something enter to trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            //check here who got in trigger end process hit like in example

            ////if touched object - player - charge his fuel 
            ActiveObject _controller = other.GetComponentInParent<ActiveObject>();
            if (_controller != null)
            {
                Log.Write("Hited player's collider ", LogColors.Red);
                _controller.OnHit(this);
                MakeExplosion();
                Despawn();
            }
        }

        /// <summary>
        /// Call it to spawn explosion VFXs
        /// </summary>
        void MakeExplosion()
        {
            SpawnExplosion(); 
        }

        #endregion

        /// <summary>
        /// Mine's logic states
        /// </summary>
        enum State
        {
            Idle,
            Flying,
            Hited,
        }
    }

}