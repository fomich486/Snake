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
    /// Class of rocket logic processing
    /// </summary>
    public class Rocket : MonoBehaviour, IBullet, ISpawnableObject
    {
        #region VARIABLES
        /// <summary>
        /// Template(prefab)
        /// </summary>
        [SerializeField]
        private Transform template;
        /// <summary>
        /// Rocket data
        /// </summary>
        [SerializeField]
        public BulletData data;
        /// <summary>
        /// Current activity state
        /// </summary>
        private State curState = State.Idle;
        /// <summary>
        /// Array of hit sounds
        /// </summary>
        [SerializeField]
        private AudioClip[] hitSound;
        /// <summary>
        /// Array of hit FXs
        /// </summary>
        [SerializeField]
        private Transform[] hitFx;
        /// <summary>
        /// Raycast layer mask for interacting with them
        /// </summary>
        [SerializeField]
        private LayerMask raycastMask; 
        /// <summary>
        /// Socket for raycasting from
        /// </summary>
        [SerializeField]
        private Transform raycastSocket;
        /// <summary>
        /// Lifetime of rocket
        /// </summary>
        private float lifeTime = 1f;
        /// <summary>
        /// Tag of master
        /// </summary>
        private string creatorTag = "default";
        /// <summary>
        /// Flag if its installed in pool
        /// </summary>
        public bool InstalledInPool = false;
        /// <summary>
        /// Smoke ps
        /// </summary>
        public ParticleSystem ps; 
        #endregion

        #region METHODS
        /// <summary>
        /// Call it to set custom rocket data
        /// </summary>
        /// <param name="_data"></param>
        public void SetData(BulletData _data)
        {
            lifeTime = _data.LifeTime;
            data = _data;
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
            }
        }

        /// <summary>
        /// Call it to launch rocket
        /// </summary>
        public void Launch()
        {
            curState = State.Flying;
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
        /// Call it to get transform
        /// </summary>
        /// <returns></returns>
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
        /// <param name="_template"></param>
        public void SetTemplate(Transform _template)
        {
            template = _template; 
        }

        /// <summary>
        /// Called when become disabled. Annulate all variables
        /// </summary>
        private void OnDisable()
        {
            curState = State.Idle;
        }

        private void LateUpdate()
        { 
            if (curState == State.Flying && ps != null)
            {
                //proccess of moving particles permanently

                //ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[ps.particleCount];
                //int _count = ps.GetParticles(_particles);
                //for (int i = 0; i < _particles.Length; i++)
                //{
                //    float yVel = GameManager.Instance.Speed;
                //    _particles[i].velocity = new Vector3(0, 0, -yVel);
                //}
                //ps.SetParticles(_particles, _count);
                //lastSpeed = GameManager.Instance.Speed;
            }
        }

        /// <summary>
        /// called each frame
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
                        //move here particles if you need

                        //ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[ps.particleCount];
                        //for (int i = 0; i < _particles.Length; i++)
                        //{
                        //    float yVel = GameManager.Instance.Speed;
                        //    _particles[i].velocity = new Vector3(0, 0, _particles[i].velocity.z - lastSpeed + yVel);
                        //}
                        //lastSpeed = GameManager.Instance.Speed;

                        //moving
                        //transform.Translate(transform.forward * TimeScaleManager.Instance.GetDelta() * data.Speed - Vector3.forward * TimeScaleManager.Instance.GetDelta() * GameManager.Instance.Speed, Space.World);
                        //transform.Translate(-Vector3.forward * TimeScaleManager.Instance.GetDelta() * GameManager.Instance.Speed, Space.World);

                        //find hit and closest available object
                        RaycastHit[] _hits = Physics.RaycastAll(raycastSocket.position, transform.forward, TimeScaleManager.Instance.GetDelta() * data.Speed * 2, raycastMask);
                        if (_hits.Length > 0)
                        {
                            GoClosestPoint(_hits);
                            curState = State.Hited;
                            Destroy();
                            return;
                        }

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
        /// Call it to go to the closest hit point!
        /// </summary>
        /// <param name="_hits"></param>
        void GoClosestPoint(RaycastHit[] _hits)
        {
            Vector3 _newPos = Vector3.zero;  
            float _minDist = Mathf.Infinity;
            for (int i = _hits.Length - 1; i >= 0; i--)
            {
                ActiveObject _ao = _hits[i].collider.gameObject.GetComponent<ActiveObject>();
                if (_ao == null)
                    _ao = _hits[i].collider.gameObject.GetComponentInParent<ActiveObject>();

                if(_ao!=null && _ao.gameObject.tag == creatorTag)
                {
                    //DO NOTHING
                }
                else
                { 
                    float _curDist = Vector3.SqrMagnitude(_hits[i].point - transform.position);
                    if (_curDist < _minDist)
                    {
                        if (_ao != null && _ao.GetHitSocket() != null)
                            this.transform.position = _ao.GetHitSocket().position;

                        _newPos = _hits[i].point;
                    }
                }
            }

            //if (_newPos != Vector3.zero)
            //    transform.position = _newPos;
        }

        /// <summary>
        /// Call it to add hited bullet to statistics
        /// </summary>
        void CheckForStatisticsDataHited()
        {
            if (creatorTag == "Player")
            { 
            }
        }

        /// <summary>
        /// Call it to destroy rocket(with explosion)
        /// </summary>
        void Destroy()
        { 
            //imitate sphere explosion
            Collider[] _hitedColliders = Physics.OverlapSphere(transform.position, data.ExplosionRadius, raycastMask);
            foreach (Collider _col in _hitedColliders)
            {
                //hit all objects in sphere
                ActiveObject _aoHitting = _col.gameObject.GetComponent<ActiveObject>();
                if (_aoHitting == null)
                    _aoHitting = _col.gameObject.GetComponentInParent<ActiveObject>();

                if (_aoHitting != null && _aoHitting.gameObject.tag != creatorTag)
                { 
                    _aoHitting.OnHit(this);
                    CheckForStatisticsDataHited();
                }
            }

            MakeExplosion();

            //despawning
            Despawn();
        }

        void MakeExplosion()
        { 
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
        /// Call it to despawn
        /// </summary>
        void Despawn()
        { 
            //despawning
            PoolManager.Instance.Despawn(transform, GetTemplate());
        }

        /// <summary>
        /// Call it to set installed in pool manager
        /// </summary>
        public void SetInstalled()
        {
            InstalledInPool = true;
        }

        /// <summary>
        /// States of rocket
        /// </summary>
        enum State
        {
            Idle,
            Flying,
            Hited,
        }

        #endregion
    }
}