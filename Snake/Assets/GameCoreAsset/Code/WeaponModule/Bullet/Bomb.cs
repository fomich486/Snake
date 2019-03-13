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
    /// Class for bomb logic performing
    /// </summary>
    public class Bomb : MonoBehaviour, IBullet, ISpawnableObject
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
        /// Flag if object is installed in pool
        /// </summary>
        public bool InstalledInPool = false;
        /// <summary>
        /// Cashed start damage
        /// </summary>
        private int baseDamage;
        #endregion

        #region METHODS

        private void Awake()
        {
            baseDamage = (int)data.BulletDamage;
        }

        /// <summary>
        /// Call it to set custom bomb data
        /// </summary>
        /// <param name="_data"></param>
        public void SetData(BulletData _data)
        {
            data = _data;
            lifeTime = data.LifeTime;
            ActiveObject _ao = _data.Owner.GetComponentInParent<ActiveObject>();
            if (_ao != null)
            {
                creatorTag = _ao.gameObject.tag;
            }
            CheckForStatisticsData();
        }

        /// <summary>
        /// Call it to add shooted bullet to statistics
        /// </summary>
        void CheckForStatisticsData()
        {
            if (creatorTag == "Player")
            { 
                //put this to statistics
            }
        }

        /// <summary>
        /// Call it to add hited bullet to statistics
        /// </summary>
        void CheckForStatisticsDataHited()
        {
            if (creatorTag == "Player")
            {
                //put this to statistics
            }
        }

        /// <summary>
        /// Call it to launch bomb
        /// </summary>
        public void Launch()
        {
            curState = State.Flying;
            
            //this is example how you can remember direction of launching object

            //calculation of side speed (right in time of bomb dropping) 
            //float _lastPlayerSideSpeed = data.Owner.GetComponentInParent<PlayerController>().LastXDifference / Time.deltaTime;
            //moveDirection = -Vector3.forward * GameManager.Instance.Speed * (1f -   forwardSpeedMultiplier) + Vector3.right * _lastPlayerSideSpeed; 
            //data.BulletTransform = transform;
        }

        /// <summary>
        /// Call it to get bomb data
        /// </summary>
        /// <returns></returns>
        public BulletData GetData()
        {
            return data;
        }

        /// <summary>
        /// Call it to get transform
        /// </summary>
        /// <returns>Transform of bomb</returns>
        public Transform GetTransform()
        {
            return transform;
        }

        /// <summary>
        /// Call it to get template(prefab)
        /// </summary>
        /// <returns></returns>
        public Transform GetTemplate()
        {
            return template;
        }

        /// <summary>
        /// Call it to set template(prefab)
        /// </summary>
        /// <param name="_template">Template(prefab)</param>
        public void SetTemplate(Transform _template)
        {
            template = _template;
        }

        /// <summary>
        /// Called when become disabled (anullate all variables here)
        /// </summary>
        private void OnDisable()
        {
            data.BulletDamage = baseDamage;
            curState = State.Idle;
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            switch (curState)
            {
                case State.Idle:
                    {
                        //nothing to do yet
                    }
                    break;
                case State.Flying:
                    {
                        //add gravity
                        moveDirection += -Vector3.up * GiConst * TimeScaleManager.Instance.GetDelta();
                        //move
                        transform.Translate(moveDirection * TimeScaleManager.Instance.GetDelta(), Space.World);
                        //check for lifetime
                        lifeTime -= TimeScaleManager.Instance.GetDelta();
                        if (lifeTime <= 0)
                        {
                            if (data.EndDestroy)
                                Destroy();
                            else
                                Despawn();
                        }
                    }
                    break;
                case State.Hited:
                    {
                        //noting to do yet
                    }
                    break;
            }
        }

        /// <summary>
        /// Call it to find closest hit poin with ActiveObjet
        /// </summary>
        /// <param name="_hits"></param>
        /// <returns></returns>
        ActiveObject GetFirstActiveObject(RaycastHit[] _hits)
        {
            ActiveObject _closest = null;
            Vector3 _newPos = Vector3.zero;
            float _minDist = Mathf.Infinity;
            for (int i = _hits.Length - 1; i >= 0; i--)
            {
                ActiveObject _ao = _hits[i].collider.gameObject.GetComponent<ActiveObject>();
                if (_ao != null && _ao.gameObject.tag != creatorTag)
                {
                    float _curDist = Vector3.SqrMagnitude(_hits[i].point - transform.position);
                    if (_curDist < _minDist)
                    {
                        _newPos = _hits[i].point;
                        _closest = _ao;
                        _minDist = _curDist;
                    }
                }
            }

            //if we found - imitate that we already here
            if (_closest != null)
            {
                transform.position = _newPos;
            }

            return _closest;
        }

        /// <summary>
        /// Called when trigger touches something
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            bool _hitedSomething = false;
            //looking for hit targets

            ActiveObject _ao = other.gameObject.GetComponentInParent<ActiveObject>();
            if(_ao != null && _ao.gameObject.tag == creatorTag)
            {
                //do nothing
            }
            else
            {
                curState = State.Hited;
                Destroy();
            } 
        } 

        /// <summary>
        /// Call it to make hits in objects with some radius
        /// </summary>
        void MakeExplosion()
        {
            Collider[] _hitedColliders = Physics.OverlapSphere(transform.position, data.ExplosionRadius, raycastMask); 
            foreach (Collider _col in _hitedColliders)
            {
                //hit all objects in sphere
                ActiveObject _aoHitting = _col.gameObject.GetComponent<ActiveObject>();
                if (_aoHitting == null)
                    _aoHitting = _col.gameObject.GetComponentInParent<ActiveObject>();

                if (_aoHitting != null && _aoHitting.gameObject.tag != creatorTag)
                {
                    transform.position = _aoHitting.transform.position;
                    _aoHitting.OnHit(this);
                    CheckForStatisticsDataHited();
                }
                else if (_aoHitting != null)
                {

                }
            }
        }

        /// <summary>
        /// Call it to destroy bullet
        /// </summary>
        void Destroy()
        {
            MakeExplosion();

            Despawn();
             
            //Playing hit sound
            if (hitSound.Length > 0)
            {
                int _randomedIndex = UnityEngine.Random.Range(0, hitSound.Length);
                if (hitSound[_randomedIndex] != null)
                    SFXManager.Instance.PlaySound(hitSound);
            }

            //Playing hit FX
            if (hitFx.Length > 0)
            {
                int _ramdomedIndex = UnityEngine.Random.Range(0, hitFx.Length);
                if (hitFx[_ramdomedIndex] != null)
                    PoolManager.Instance.Spawn(hitFx[_ramdomedIndex], transform.position, transform.rotation);
            }
        }

        /// <summary>
        /// Call it to despawn it in pool manager
        /// </summary>
        void Despawn()
        {
            //despawning
            PoolManager.Instance.Despawn(transform, GetTemplate());
        }

        /// <summary>
        /// Call it to install pool
        /// </summary>
        public void SetInstalled()
        {
            InstalledInPool = true;
        }
         
        #endregion

        enum State
        {
            Idle,
            Flying,
            Hited,
        }
    }

}