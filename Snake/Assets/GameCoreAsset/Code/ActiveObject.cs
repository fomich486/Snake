using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using PoolManagerModule;
using WeaponSystem;
using SFXManagerModule;

public class ActiveObject : SpawnableObject
{
    #region VARIABLES 
    /// <summary>
    /// List of FXs that available to be spawned on object despawning
    /// </summary>
    [SerializeField]
    private Transform[] destroyFXs;
    /// <summary>
    /// List of VFXs that available to be played on object despawning
    /// </summary>
    [SerializeField]
    private AudioClip[] destroyVFXs;
    /// <summary>
    /// Current hp count of object
    /// </summary>
    [SerializeField]
    private float Hp = 10f;
    /// <summary>
    /// Socket where you can play hit FX
    /// </summary>
    [SerializeField]
    private Transform hitSocket;
    /// <summary>
    /// Flag if object is on the ground
    /// </summary>
    public bool Grounded = true;

    public string ObjectKey = "";
    public bool AlreadyProcessed = false;

    public Renderer[] RendsToHide;
    public Renderer RendTemplate;
    #endregion

    #region METHODS

    private void OnEnable()
    {
        base.OnEnable();
        AlreadyProcessed = false;
    }

    protected virtual void FixedUpdate()
    {
        base.FixedUpdate();

        //if(RendsToHide!=null && RendsToHide.Length > 0 && RendTemplate!=null)
        //{
        //    foreach (Renderer _mr in RendsToHide)
        //    {
        //        if (_mr.enabled && !RendTemplate.isVisible)
        //            _mr.enabled = false;
        //        else if (!_mr.enabled && RendTemplate.isVisible)
        //            _mr.enabled = true;
        //    }
        //}
    }

    /// <summary>
    /// Call it to make hit to object
    /// </summary>
    /// <param name="_bullet"></param>
    /// <returns></returns>
    public virtual bool OnHit(IBullet _bullet)
    {
        if (_bullet == null)
            return false;

        BulletData _data = _bullet.GetData();
        if(_data == null)
        {
            if (_bullet.GetTransform() == null)
                Log.Write("Bullet doent contain Transform. Fix it!",LogColors.Yellow);
            else
                Log.Write("There is no BulletData in bullet!" + _bullet.GetTransform().name, LogColors.Yellow);

            return false;
        }

        //reducing hp
        Hp -= _data.BulletDamage;
        //check for death
        if (Hp <= 0)
        {
            Destroy();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Call it to make damage by some value
    /// </summary>
    /// <param name="_damage"></param>
    public virtual void OnDamaged(float _damage)
    {
        //reducing hp
        Hp -= _damage;
        //check for death
        if (Hp <= 0)
        {
            Destroy(); 
        }
    }

    /// <summary>
    /// Call it to get hit socket
    /// </summary>
    /// <returns></returns>
    public Transform GetHitSocket()
    {
        return hitSocket;
    }

    /// <summary>
    /// Call it to spawn explosion and play explosion sound
    /// </summary>
    public void SpawnExplosion()
    { 
        //Playing death sound
        if (destroyVFXs.Length > 0)
        {
            int _randomedIndex = Random.Range(0, destroyVFXs.Length);
            if (destroyVFXs[_randomedIndex] != null)
                SFXManager.Instance.PlaySound(destroyVFXs);
        }

        //Playing death FX
        if (destroyFXs.Length > 0)
        {
            int _ramdomedIndex = Random.Range(0, destroyFXs.Length);
            if (destroyFXs[_ramdomedIndex] != null)
            { 
                Transform _fx = PoolManager.Instance.Spawn(destroyFXs[_ramdomedIndex], hitSocket.position, Quaternion.identity);
                if (_fx != null)
                    _fx.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
            }
        }
    }

    /// <summary>
    /// Call it to destroy ActiveObject
    /// </summary>
    public virtual void Destroy()
    { 
        SpawnExplosion();
        Despawn();
    }

    /// <summary>
    /// Call it to despawn object
    /// </summary>
    public virtual void Despawn()
    {  
        if (GetTemplate() == null)
            PoolManager.Instance.Despawn(transform);
        else
            PoolManager.Instance.Despawn(transform, GetTemplate());
    }
    #endregion
}
