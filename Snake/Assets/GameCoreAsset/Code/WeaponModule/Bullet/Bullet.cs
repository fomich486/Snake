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
    /// Class for performing bullet's logic
    /// </summary>
    public class Bullet : ActiveObject, IBullet, ISpawnableObject
    {
        #region VARIABLES
        /// <summary>
        /// Data of bullet
        /// </summary>
        [SerializeField]
        public BulletData data;
        /// <summary>
        /// Current action state
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
        /// LayerMask for interacting
        /// </summary>
        [SerializeField]
        private LayerMask raycastMask;
        /// <summary>
        /// Flag if hited object was destroyed(if true - we havent play hit fx and audio)
        /// </summary>
        private bool destroyed = false;
        /// <summary>
        /// Socket for raycasting from
        /// </summary>
        [SerializeField]
        private Transform raycastSocket; 
        /// <summary>
        /// Local lifetime of bullet
        /// </summary>
        private float lifeTime = 1f;
        /// <summary>
        /// Tag name of creator
        /// </summary>
        private string creatorTag = "default";
        /// <summary>
        /// Flag if we have to cast next frame
        /// </summary>
        private bool raycast = false;
        /// <summary>
        /// Array of line length
        /// </summary>
        public int[] LineLength = new int[7] {6,4,6,4,6,4,15};
        /// <summary>
        /// Array of line width
        /// </summary>
        public float[] LineWidth = new float[3] { 0.3f, 0.4f, 0.5f };
        /// <summary>
        /// LineRenderer cashed component
        /// </summary>
        public LineRenderer Trail;
        /// <summary>
        /// Flag if we have to build trail after bullet
        /// </summary>
        public bool TrailBack = false;
        /// <summary>
        /// Prefab for hope
        /// </summary>
        public Transform HolePrefab;
        #endregion

        #region METHODS
        /// <summary>
        /// Call it to set custom bullet data
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
                CheckForStatisticsData();
            } 
        }

        /// <summary>
        /// Call it to generate holes
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_lookAt"></param>
        void GenerateHole(Vector3 _pos, Vector3 _lookAt)
        {
            if (HolePrefab != null)
            {
                Transform _spawnedHole = PoolManager.Instance.Spawn(HolePrefab, _pos);
                if (_spawnedHole != null)
                {
                    _spawnedHole.LookAt(_lookAt);
                    _spawnedHole.localEulerAngles = new Vector3(_spawnedHole.localEulerAngles.x, _spawnedHole.localEulerAngles.y, UnityEngine.Random.RandomRange(0f, 360f));
                    //Log.Write("WE SPAWNED BULLET HOLE!", LogColors.Yellow);
                }
                else
                {
                    Log.Write("There is no spawned hole with name(" + HolePrefab.name + ") for this bullet:" + gameObject.name);
                }
            }
            else
            {
                Log.Write("There is no set hole for bullet...");
            }
        }

        /// <summary>
        /// Call it to launch bullet
        /// </summary>
        public void Launch()
        {
            curState = State.Flying;
            if (Trail != null)
            {
                //TO DO
                float _length = LineLength[(int)UnityEngine.Random.Range(0, LineLength.Length)];
                Trail.SetPosition(1, new Vector3(0f, 0f, -_length));

                if (!TrailBack)
                    transform.Translate(transform.forward * _length, Space.World);

                float _width = LineWidth[(int)UnityEngine.Random.Range(0, LineWidth.Length)];
                Trail.startWidth = _width;
                Trail.endWidth = _width;
            }
        }

        /// <summary>
        /// Call it to get bullet data
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
            return base.Template;
        }
         
        /// <summary>
        /// Call it to set template(prefab)
        /// </summary>
        /// <param name="_template"></param>
        public void SetTemplate(Transform _template)
        {
            base.Template = _template;
        }
         
        /// <summary>
        /// Called when disabled. Clear variables here
        /// </summary>
        private void OnDisable()
        {
            curState = State.Idle;
            destroyed = false;
        }

        /// <summary>
        /// Called each  frame
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
                        //moving
                        transform.Translate(transform.forward * TimeScaleManager.Instance.GetDelta() * data.Speed , Space.World);

                        //if (ControllerByGameSpeed)
                        //    transform.Translate(-Vector3.forward * TimeScaleManager.Instance.GetDelta() * GameManager.Instance.Speed, Space.World);

                        raycast = !raycast;

                        if (raycast)
                        {
                            //looking for impact
                            RaycastHit _hit;
                            if (Physics.Raycast(raycastSocket.position, transform.forward, out _hit, TimeScaleManager.Instance.GetDelta() * data.Speed * 2f * 1.5f, raycastMask))
                            {
                                ActiveObject _ao = _hit.collider.GetComponent<ActiveObject>();
                                transform.position = _hit.point;
                                //lets move it a little bit - SOME KIND OF LIFEHACK
                                transform.Translate(transform.forward * 0.5f, Space.World);

                                if (_ao == null)
                                    _ao = _hit.collider.gameObject.GetComponentInParent<ActiveObject>();

                                if (_ao != null && _ao.transform != data.Owner && _ao.gameObject.tag != creatorTag)
                                {
                                    CheckForStatisticsDataHited();
                                    curState = State.Hited;
                                    destroyed = _ao.OnHit(this);
                                    //transform.position = _ao.GetHitSocket().position;

                                    Destroy();
                                    return;
                                }
                                else if (_ao == null)
                                { 
                                    /*//if(_hit.collider.gameObject.tag == "Terrain")
                                    //{
                                    //    //Log.Write("Trying geenrate hole...", LogColors.White);
                                    //    //GenerateHole(_hit.point, _hit.point + _hit.normal);
                                    //}*/ 
                                    curState = State.Hited;
                                    Destroy();
                                    return;
                                }
                            }
                        } 

                        ///check for lifetime
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
        /// Call it to add shooted bullet to statistics
        /// </summary>
        void CheckForStatisticsData()
        {
            if (creatorTag == "Player")
            { 
            }
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
                if (_ao == null)
                    _ao = _hits[i].collider.gameObject.GetComponentInParent<ActiveObject>();

                if(_ao != null && _ao.transform != data.Owner && _ao.gameObject.tag != creatorTag)
                {
                    float _curDist = Vector3.SqrMagnitude(_hits[i].point - transform.position);
                    if (_curDist < _minDist)
                    {
                        _newPos = _ao.GetHitSocket().position;
                        _closest = _ao;
                        _minDist = _curDist;
                    }
                }
            } 

            return _closest;
        }

        /// <summary>
        /// Call it to move object to the closest hit point
        /// </summary>
        /// <param name="_hits"></param>
        bool GoClosestPoint(RaycastHit[] _hits)
        { 
            Vector3 _newPos = Vector3.zero;
            float _minDist = Mathf.Infinity;
            for (int i = _hits.Length - 1; i >= 0; i--)
            {
                ActiveObject _ao = _hits[i].collider.GetComponent<ActiveObject>();
                if (_ao == null)
                    _ao = _hits[i].collider.gameObject.GetComponentInParent<ActiveObject>();

                if(_ao!=null && _ao.gameObject.tag == creatorTag)
                {
                    //ingnoring this
                }
                else
                {
                    float _curDist = Vector3.SqrMagnitude(_hits[i].point - transform.position);
                    if (_curDist < _minDist)
                    {
                        _newPos = _hits[i].point;
                    }
                }
            }

            if (_newPos != Vector3.zero)
            {
                transform.position = _newPos;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call it to destroy bullet
        /// </summary>
        void Destroy()
        { 
            //if object hasnt beed destroyed - we may play hit sound and fx
            if (!destroyed)
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

            Despawn();
        }

        /// <summary>
        /// Call it to despawn bullet
        /// </summary>
        void Despawn()
        {
            //despawning
            PoolManager.Instance.Despawn(transform, GetTemplate());
        }

        /// <summary>
        /// Enum of bullet states
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