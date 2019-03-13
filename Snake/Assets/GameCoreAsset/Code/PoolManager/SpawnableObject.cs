using DataSystem; 
using PoolManagerModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour, ISpawnableObject
{
    #region VARIABLES
    /// <summary>
    /// Cashed template of 
    /// </summary>
    public Transform Template; 
    /// <summary>
    /// Delta time to despawn object
    /// </summary>
    public float DespawnAfter = 0f;
    /// <summary>
    /// Delta time to despawn object
    /// </summary>
    public float DespawnAfterDistance = 0f;
    /// <summary>
    /// Flag if particle system playing speed have to be controlled via virtual timescale
    /// </summary>
    public bool ControlParticleSystemSpeed = false;  
    /// <summary>
    /// list of particle systems
    /// </summary>
    [SerializeField]
    private ParticleSystem[] ps;
    /// <summary>
    /// Multiers of game speed
    /// </summary>
    public float[] Multipliers;
    /// <summary>
    /// cor for updating ps playback speed
    /// </summary>
    private WaitForSeconds UpdatePSDelay = new WaitForSeconds(0.2f);
    /// <summary>
    /// current playback speed(in particle systems)
    /// </summary>
    private float currentPlaybackSpeed = 1f;
    /// <summary>
    /// Flag if this object insta
    /// </summary>
    public bool InstalledInPool = false;  

    /// <summary>
    /// private lifetime timer for spawnable object
    /// </summary>
    private float lifeTimeCur = 0f;   

    /// <summary>
    /// Flag if we need to update particles speed permanently (for some reason in some direction)
    /// </summary>
    public bool UpdateParticlesByGameSpeed = false;

    /// <summary>
    /// Flag if we need to scale object
    /// </summary>
    public bool NeedScale = false; 
    /// <summary>
    /// Min distance to start object scaling
    /// </summary>
    public float DistanceToStartScale = 304f;
    /// <summary>
    /// Max distance to end object scaling
    /// </summary>
    public float DistanceToEndScale = 0f;
    /// <summary>
    /// Min scale size
    /// </summary>
    public float MaxScale = 1f;
    /// <summary>
    /// max scale size
    /// </summary>
    public float MinScaleValue = 0.35f;

    /// <summary>
    /// List of objects to show only in high quality
    /// </summary>
    public GameObject[] ShowInHighQualityOnly;
    /// <summary>
    /// ParticleSystems to stop emition under water
    /// </summary>
    public ParticleSystem[] PSToHide;
    /// <summary>
    /// Flag if we have to stop emit 
    /// </summary>
    public bool HideUnderWater = false;
    /// <summary>
    /// Position of Y axiz for water
    /// </summary>
    public float WaterPosition = -11.5f;
    #endregion
     
    #region METHODS
    /// <summary>
    /// Called when object become spawned
    /// </summary>
    protected void OnEnable()
    {
        //we havent do anything if it's not installed in pool
        if (!InstalledInPool)
            return;

        //if we have despawn timer - clear private timer to basic despawn time
        if (DespawnAfter > 0)
        {
            if (PoolManager.Instance == null)
                Log.Write("There is no instance of pool manager!", LogColors.Red);
            lifeTimeCur = DespawnAfter; 
        }

        //if we need to control particle system by virtual time - start this coroutine
        if (ControlParticleSystemSpeed)
        { 
            StartCoroutine(CheckForPlaybackSpeed());
        } 

        //and if we can hide particles under water(or, actually, under some Y axiz value) - enable emission (in case, it was disabled in prev life cycle)
        if (HideUnderWater)
        {
            foreach (ParticleSystem _ps in PSToHide)
            {
                _ps.enableEmission = true;
            }
        }
    }

    /// <summary>
    /// Call it to start checking for particle systems playback speed
    /// </summary>
    public void StartCheckingForParticleSpeed()
    {
        if (ControlParticleSystemSpeed) 
        {
            if(ps != null && ps.Length > 0)
            {
                //do nothing
            }
            else
            {
                ps = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            }
            StartCoroutine(CheckForPlaybackSpeed());
        }
    }  

    /// <summary>
    /// Call it to update scale of object
    /// </summary>
    void UpdateScale()
    {
        float _curZ = transform.position.z;
        if(_curZ > DistanceToStartScale && transform.localScale.x != MaxScale)
        {
            transform.localScale = Vector3.one * MaxScale;
        }
        else
        {
            float _scaleSize = Mathf.Lerp(MinScaleValue, MaxScale, (1f * _curZ / DistanceToStartScale));
            if(transform.localScale.x != _scaleSize)
            {
                transform.localScale = Vector3.one * _scaleSize;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!InstalledInPool)
            return;

        //check somehow qulity setting of your game and turn off unnecessary files

        //if (DataManager.Instance != null && !DataManager.Instance.saveData.BeautifulWater && ShowInHighQualityOnly != null && ShowInHighQualityOnly.Length>0)
        //{
        //    foreach (GameObject _go in ShowInHighQualityOnly)
        //    {
        //        if(_go!=null && _go.activeSelf)
        //        {
        //            _go.SetActive(false);
        //        }
        //    }
        //}

        if (HideUnderWater)
        {
            CheckForHideUnderWater();
        }
    }

    /// <summary>
    /// Call it to check if current position of particles under some Y axiz value, if true - disable it's emittion
    /// </summary>
    void CheckForHideUnderWater()
    {
        foreach(ParticleSystem _ps in PSToHide)
        {
            if(_ps.isEmitting && _ps.transform.position.y <= WaterPosition)
            {
                _ps.enableEmission = false;
            }
        }
    }

    /// <summary>
    /// Called each frame
    /// </summary>
    protected void Update()
    { 
        //some methods you can use even withoud installation in pool manager (bad practise, but fast developing generated this situation)
        if (UpdateParticlesByGameSpeed)
        {
            UpdateParticlesPosition();
        }

        if (NeedScale)
        {
            UpdateScale();
        }

        if (!InstalledInPool)
            return;
          
        if (DespawnAfterDistance != 0)
        {
            if (transform.position.z == DespawnAfterDistance)
            {
                if (PoolManager.Instance == null)
                    Log.Write("There is no instance of pool manager!", LogColors.Red);
                else
                {
                    PoolManager.Instance.Despawn(transform, GetTemplate());
                    return;
                }
            }
        }

        if (DespawnAfter > 0)
        {
            lifeTimeCur -= TimeScaleManager.Instance.GetDelta();
            if (lifeTimeCur <= 0)
            {
                PoolManager.Instance.Despawn(transform, GetTemplate());
                return;
            }
        }

        //if (ControllerByGameSpeed)
        //{
        //    transform.Translate(-Vector3.forward * GameManager.Instance.Speed * TimeScaleManager.Instance.GetDelta(), Space.World);
        //}
    }

    /// <summary>
    /// Call it to update particles position by game speed
    /// </summary>
    void UpdateParticlesPosition()
    {
        int _index = 0;
        foreach(ParticleSystem _ps in ps)
        { 
            ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[_ps.particleCount];
            int _count = _ps.GetParticles(_particles);
            for (int i = 0; i < _particles.Length; i++)
            {
                float _mult = 1;
                if (_index < Multipliers.Length)
                    _mult = Multipliers[_index];

                //update here position by speed of game

                //float yVel = GameManager.Instance.Speed * _mult;
                //_particles[i].position += (_particles[i].velocity + new Vector3(0, 0, -yVel)) * TimeScaleManager.Instance.GetDelta();
            }
            _ps.SetParticles(_particles, _count);
            _index++;
        }
    }

    /// <summary>
    /// Call it to start checking of playback speed chaning
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckForPlaybackSpeed()
    { 
        while (gameObject.activeSelf && InstalledInPool)
        { 
            if (TimeScaleManager.Instance.GetScale() != currentPlaybackSpeed)
            {
                currentPlaybackSpeed = TimeScaleManager.Instance.GetScale();
                UpdatePlaybackSpeed(); 
            }
            yield return UpdatePSDelay;
        }
    }

    /// <summary>
    /// Call it to update playback speed in particle systems
    /// </summary>
    void UpdatePlaybackSpeed()
    {
        foreach(ParticleSystem _ps in ps)
        {
            if (_ps != null)
            {
                if (currentPlaybackSpeed == 0)
                {
                    if (!_ps.isPaused)
                        _ps.Pause();
                }
                else
                {
                    if (_ps.isPaused)
                        _ps.Play();
                }

                if (_ps.playbackSpeed != currentPlaybackSpeed)
                    _ps.playbackSpeed = currentPlaybackSpeed;
            }
        }
    }

    /// <summary>
    /// Call it to get template
    /// </summary>
    /// <returns></returns>
    public Transform GetTemplate()
    {
        return Template;
    } 

    /// <summary>
    /// Call it to set template
    /// </summary>
    /// <param name="_template"></param>
    public void SetTemplate(Transform _template)
    {
        Template = _template;
    }

    /// <summary>
    /// Call it to set object as installed
    /// </summary>
    public void SetInstalled()
    {
        InstalledInPool = true;
    }
    #endregion
}
