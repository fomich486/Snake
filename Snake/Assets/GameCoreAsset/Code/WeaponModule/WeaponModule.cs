using PoolManagerModule;
using SFXManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class of weapon module.
    /// </summary>
    public class WeaponModule : MonoBehaviour
    {
        #region VARIABLES
        /// <summary>
        /// Flag for trigger mode(if true - weapon module will make auto fire when fire started until it will be end)
        /// </summary>
        public bool TriggerMode = false;
        /// <summary>
        /// Flag for firing(true when begin touch, false after end of firing)
        /// </summary>
        public bool fireStarted = false;
        /// <summary>
        /// Count of shoots per minute
        /// </summary>
        public int FireRate = 400;
        /// <summary>
        /// Delta between shots. Calculating itself
        /// </summary>
        private float fireDelta = 1f;
        /// <summary>
        /// Last time of shot
        /// </summary>
        public float lastFireTime = 0f;
        /// <summary>
        /// Template of bullets for automatic firing (not recommended for secondary and additional logic)
        /// </summary>
        public Transform BulletTemplate;
        /// <summary>
        /// Templates of bombs/rockets with queue
        /// </summary>
        public List<Transform> AvailableBulletTypes = new List<Transform>();
        /// <summary>
        /// Base count of available bullet types
        /// </summary>
        public int BaseCount = 0;
        /// <summary>
        /// Transform of bullet's example (for restoring, for example, ammo )
        /// </summary>
        public Transform BulletBase;
        /// <summary>
        /// Flag if base count already set up
        /// </summary>
        public bool BaseSetUp = false;
        /// <summary>
        /// Muzzleflashes of firing
        /// </summary>
        public Transform[] AvailableMuzzleflashes;
        /// <summary>
        /// Shot audio clips of firing
        /// </summary>
        public AudioClip[] AvailableShotClips;
        /// <summary>
        /// AudioClip for loop shooting
        /// </summary>
        public AudioClip LoopShotSound;
        /// <summary>
        /// Flag for loop sound
        /// </summary>
        private bool loopSound;
        /// <summary>
        /// Flag if currenlty playing loop sound of firing
        /// </summary>
        private bool playingLoopNow = false;
        /// <summary>
        /// local isnt
        /// </summary>
        [SerializeField]
        private int currentCapacity = 50;
        /// <summary>
        /// Current bullet count for automatic firing
        /// </summary>
        public int CurrentCapacity
        {
            get
            {
                return currentCapacity;
            }
            set
            {
                currentCapacity = value;
                if(currentCapacity > 0 && waitingCapacityCharge)
                {
                    waitingCapacityCharge = false;
                }
            }
        }
        /// <summary>
        /// Maximum count for automatic firing. -1 will mean, that count is infinity
        /// </summary>
        public int MaxCapacity = 50;
        /// <summary>
        /// Flag if we waiting for capacity charge
        /// </summary>
        private bool waitingCapacityCharge = false;
        /// <summary>
        /// Count of nonstop firing to gain overheating. 0 will mean, that there is no overheating
        /// </summary>
        public float OverheatTime = 12f;
        /// <summary>
        /// Multiplier for overheat recharge
        /// </summary>
        public float OverheatRechargeCoef = 3f;//time for overheat recharing = OverHeatTime * OverheatRechargeCoef
        /// <summary>
        /// Current time of overheating ( 6f/OverheatTime(12f) = 0.5f = 50% of overheating )
        /// </summary>
        private float overheatCurrent = 0f;
        /// <summary>
        /// count of overheating for each shot
        /// </summary>
        public float overheatPerShot = 0f;
        /// <summary>
        /// Delta time when you can start reduce overheat
        /// </summary>
        private float overheatDelta = 0.3f;
        /// <summary>
        /// Becomes true when you reach overheat to 100%. You need to wait until it will recharge to 0%
        /// </summary>
        private bool waitingOverheatRecharge = false;
        /// <summary>
        /// Socket for firing(creation bullets)
        /// </summary>
        public Transform FireSocket;
        /// <summary>
        /// sound player
        /// </summary>
        public SFXPlayer SfxPlayer;
        /// <summary>
        /// Flag if we have to play SFX for shot
        /// </summary>
        private bool playSound = true;
        /// <summary>
        /// Current AI logic for weapon
        /// </summary>
        public string CurrentAILogic = "none"; 
        /// <summary>
        /// Flag if this is player's gun (for analytics)
        /// </summary>
        private bool playerGun = false; 
        /// <summary>
        /// Type of weapon
        /// </summary>
        public WeaponType WeapType = WeaponType.Primary;

        /// <summary>
        /// Deviation angle on horizontal axiz
        /// </summary>
        public float MaxDeviationHorizontal = 0f;
        /// <summary>
        /// Deviation angle on vertical axiz
        /// </summary>
        public float MaxDeviationVertical = 0f;

        /// <summary>
        /// Damage parameter (if -1  = no overright)
        /// </summary>
        public int Damage = -1;

        /// <summary>
        /// Sprite of button of this weapon
        /// </summary>
        public Sprite WeaponButtonSprite;

        /// <summary>
        /// Cashed weapon manager
        /// </summary>
        private WeaponManager weapManager; 
        #endregion

        #region METHODS 
        /// <summary>
        /// Called in the start 
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(InitWeapon());
        }

        /// <summary>
        /// Call it to init weapon
        /// </summary>
        IEnumerator InitWeapon()
        {
            //wait one frame before processing parenting and other actions
            yield return new WaitForEndOfFrame();
             
            //if we have bullet - check pool manager for enough count of them
            if (BulletTemplate != null)
            {
                if (!PoolManager.Instance.Templates.Contains(BulletTemplate))
                {
                    int _spawnCount = (int)(FireRate / 4);
                    if (_spawnCount < 10)
                        _spawnCount = 10;

                    PoolManager.Instance.AddToPool(BulletTemplate, _spawnCount);
                }
            }

            //the same thing with bullet types
            if (AvailableBulletTypes.Count > 0)
            {
                if (!PoolManager.Instance.Templates.Contains(AvailableBulletTypes[0]))
                {
                    int _spawnCount = (int)(FireRate / 4);
                    if (_spawnCount < 10)
                        _spawnCount = 10;

                    PoolManager.Instance.AddToPool(AvailableBulletTypes[0], _spawnCount);
                }
            }

            //get weap manager
            weapManager = gameObject.GetComponentInParent<WeaponManager>();

            //cashing state of loop sound 
            if (LoopShotSound != null)
            {
                loopSound = true;
            }
            else
            {
                loopSound = false;
            }

            //if we have shot clips
            if (AvailableShotClips.Length > 0)
            {
                //getting sfx player
                if (SfxPlayer == null)
                {
                    if(loopSound)
                    {
                        SfxPlayer = SFXManager.Instance.PlayFollowingSound(LoopShotSound, transform);
                        if (SfxPlayer != null)
                        {
                            SfxPlayer.StopPlaying();
                            SfxPlayer.RandomVolumePitch = false;
                        }
                    }
                    else if (weapManager != null && weapManager.PlaySoundPerGroup)
                    {
                        //we dont need this sound player
                    }
                    else
                    {
                        SfxPlayer = SFXManager.Instance.PlayFollowingSound(AvailableShotClips[0], transform);
                        if (SfxPlayer != null)
                        {
                            SfxPlayer.StopPlaying();
                            SfxPlayer.RandomVolumePitch = true;
                        }
                    }
                }
            }

            //calculate delta time between shots
            fireDelta = 60f / FireRate;
            //shots per one second
            overheatPerShot = (FireRate / 60f);
            overheatPerShot = 1f / overheatPerShot; 
        }

        /// <summary>
        /// Call it to set that this is player's gun
        /// </summary>
        public void MakeGunPlayers()
        {
            playerGun = true;
        }

        /// <summary>
        /// Called when despawning
        /// </summary>
        private void OnDisable()
        {
            if (SfxPlayer != null)
            { 
                SfxPlayer.Disable();
            }
        }

        /// <summary>
        /// Call it to start firing
        /// </summary>
        public void StartFiring(bool _playSound = true)
        {
            playSound = _playSound;
            if (!TriggerMode)
            {
                TryShot();
            }
            else
            { 
                fireStarted = true;
            }
        }

        /// <summary>
        /// Call it to get know if you can shoot
        /// </summary>
        /// <returns></returns>
        public bool CanFire()
        {
            if (!waitingOverheatRecharge && TimeScaleManager.Instance.GetTime() > lastFireTime + fireDelta)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call it to stop firing
        /// </summary>
        public void EndFiring()
        {
            fireStarted = false;
        }

        /// <summary>
        /// Call it to get overheating coefficient
        /// </summary>
        /// <returns></returns>
        public float GetOverheating()
        {
            if (OverheatTime == 0)
                return 0;

            return overheatCurrent / OverheatTime;
        }

        /// <summary>
        /// Call it to start playing loop sound
        /// </summary>
        void StartLoopSound()
        { 
            if (SfxPlayer != null && loopSound && !playingLoopNow)
            {
                playingLoopNow = true;
                SFXSourceData _data = new SFXSourceData(); 
                _data.Clips.Add(LoopShotSound);
                _data.Loop = true;
                _data.RandomVolumeAndPitch = false;
                SfxPlayer.StartPlaying(_data);
            }
        }

        /// <summary>
        /// Call it to end playing loop sound
        /// </summary>
        void EndLoopSound()
        {
            if (SfxPlayer != null && loopSound && playingLoopNow)
            {
                playingLoopNow = false;
                SfxPlayer.StopPlaying();
            }
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            //shooting group
            if (fireStarted && TriggerMode)
            {
                if (!waitingOverheatRecharge)
                    StartLoopSound();
                else
                    EndLoopSound();
                TryShot();
            }
            else
            {
                EndLoopSound();
            }

            //overheating group
            if (TriggerMode && OverheatTime > 0 )
            {
                //if trying to fire - add no overheating
                if(fireStarted && !waitingOverheatRecharge && (CurrentCapacity > 0 || MaxCapacity <=1))
                {
                    //overheatCurrent += overheatPerShot;// TimeScaleManager.Instance.GetDelta();
                    //if we reached maximum - wait for full recharge
                    if(overheatCurrent >= OverheatTime)
                    { 
                        waitingOverheatRecharge = true;
                        overheatCurrent = OverheatTime;
                    }
                }
                //if we dont fire - reduce overheating
                else if(!fireStarted)
                {
                    if(lastFireTime + overheatDelta <= TimeScaleManager.Instance.GetTime())
                        overheatCurrent -= TimeScaleManager.Instance.GetDelta() * OverheatRechargeCoef * OverheatTime;

                    //if already 0% - remove waiting
                    if (overheatCurrent < 0)
                    { 
                        overheatCurrent = 0;
                        waitingOverheatRecharge = false;
                    }
                }
                //if trying to fire and overheating
                else if(waitingOverheatRecharge)
                {
                    if (lastFireTime + overheatDelta <= TimeScaleManager.Instance.GetTime())
                        overheatCurrent -= TimeScaleManager.Instance.GetDelta() * OverheatRechargeCoef * OverheatTime;
                    //if already 0% - remove waiting
                    if (overheatCurrent < 0)
                    { 
                        overheatCurrent = 0;
                        waitingOverheatRecharge = false;
                    }
                }
                //if we havent bulles and overheat current > 0
                else if(CurrentCapacity <= 0 && overheatCurrent > 0)
                {
                    if (lastFireTime + overheatDelta <= TimeScaleManager.Instance.GetTime())
                        overheatCurrent -= TimeScaleManager.Instance.GetDelta() * OverheatRechargeCoef * OverheatTime;
                    //if already 0% - remove waiting
                    if (overheatCurrent < 0)
                    { 
                        overheatCurrent = 0;
                        waitingOverheatRecharge = false;
                    }
                }
            }
        }

        /// <summary>
        /// Call it to try make shot
        /// </summary>
        void TryShot()
        { 
            if(lastFireTime + fireDelta <= TimeScaleManager.Instance.GetTime() && !waitingOverheatRecharge)
            { 
                if (BulletTemplate && CurrentCapacity <= 0 && MaxCapacity > 0)
                    waitingCapacityCharge = true;
                else
                    MakeShot(); 
            }
        } 

        /// <summary>
        /// Call it to make immidiate shot
        /// </summary>
        /// <param name="_template"></param>
        public void MakeShot(Transform _template)
        {
            if (_template == null)
                Log.Write("There is no template to spawn..", LogColors.Red);
             
            //saving last fire time
            lastFireTime = TimeScaleManager.Instance.GetTime();

            overheatCurrent += overheatPerShot;

            ProcessAnalytics();

            Transform _spawnedBullet = PoolManager.Instance.Spawn(_template, FireSocket.position, FireSocket.rotation);  

            SpawnMuzzleflash();
            PlayShotSound();

            if (_spawnedBullet == null)
            {
                Log.Write("We got null when tried to spawn bullet!",LogColors.Yellow);
                return;
            }
            else
            {
                IBullet _bullet = _spawnedBullet.GetComponent<IBullet>();

                BulletData _data = _bullet.GetData();
                if (Damage > 0)
                    _data.BulletDamage = Damage;

                _data.Owner = transform;
                _bullet.SetData(_data);
                _bullet.Launch();
            }
        }

        /// <summary>
        /// Call it to spawn muzzleflash
        /// </summary>
        void SpawnMuzzleflash()
        {
            if (AvailableMuzzleflashes.Length > 0)
            {
                Transform _template = AvailableMuzzleflashes[(int)Random.Range(0, AvailableMuzzleflashes.Length)];
                Transform _spawnedMuzzleflash = PoolManager.Instance.Spawn(_template, FireSocket.position, FireSocket.rotation);
                if (_spawnedMuzzleflash != null)
                {
                    FakeChildren _child = _spawnedMuzzleflash.GetComponent<FakeChildren>();
                    if (_child != null)
                    {
                        _child.SetFollowingTarget(FireSocket);
                    }
                }
                else
                {
                    Log.Write("There is no muzzleflash from pool manager. Template name:" + _template.name);
                }
            }
        }

        /// <summary>
        /// Call it to play shot sound
        /// </summary>
        void PlayShotSound()
        {
            if (AvailableShotClips.Length > 0 && !loopSound)
            {
                if (playSound)
                {
                    if (SfxPlayer != null)
                        SfxPlayer.PlaySound(AvailableShotClips[(int)Random.Range(0, AvailableShotClips.Length)]);
                    else
                        SFXManager.Instance.PlaySound(AvailableShotClips[(int)Random.Range(0, AvailableShotClips.Length)], 0.3f);
                }
            }
        }

        /// <summary>
        /// Call it to process analytics (add fired bullet type)
        /// </summary>
        public void ProcessAnalytics()
        {
            if (playerGun)
            {
                switch (WeapType)
                {
                    //case WeaponType.Primary:
                    //    GameManager.Instance.TotalFired++;
                    //    break;
                    //case WeaponType.Secondary:
                    //    GameManager.Instance.TotalFiredRockets++;
                    //    break;
                    //case WeaponType.Additional:
                    //    GameManager.Instance.TotalFiredBombs++;
                    //    break;
                }
            }
        }

        /// <summary>
        /// Call it to generate and launch bullet
        /// </summary>
        void MakeShot()
        {
            //saving last fire time
            lastFireTime = TimeScaleManager.Instance.GetTime();

            overheatCurrent += overheatPerShot;

            //if we have template - fire
            if (BulletTemplate)
            {
                ProcessAnalytics();
                 
                Transform _spawnedBullet = PoolManager.Instance.Spawn(BulletTemplate, FireSocket.position, FireSocket.rotation);
                if (_spawnedBullet != null)
                {
                    //randoming deviation
                    Vector2 _randomed = Random.insideUnitCircle;
                    if (Mathf.Abs(_randomed.x) >= MaxDeviationHorizontal * 0.95f)
                        _randomed.x = _randomed.x * 3f;

                    Vector3 _lookAt = _spawnedBullet.right * _randomed.x * (MaxDeviationHorizontal / 45f) * 0.5f + _spawnedBullet.up * _randomed.y * (MaxDeviationVertical / 45f) * 0.5f + _spawnedBullet.forward;
                    _spawnedBullet.LookAt(_spawnedBullet.position + _lookAt);
                     
                    SpawnMuzzleflash();
                    PlayShotSound();
                    currentCapacity--;
                    if (_spawnedBullet == null)
                    {
                        Log.Write("We got null when tried to spawn bullet!", LogColors.Yellow);
                        return;
                    }
                    else
                    {
                        IBullet _bullet = _spawnedBullet.GetComponent<IBullet>();

                        BulletData _data = _bullet.GetData();
                        if (Damage > 0)
                            _data.BulletDamage = Damage;

                        _data.Owner = this.transform;
                        _bullet.SetData(_data);
                        _bullet.Launch();
                    }
                }
                else
                {
                    Log.Write("There is no spawned bullet by name:" + BulletTemplate.name);
                }
            }
            //if we have some bullets to shot (bomb or rocket)
            else if ((AvailableBulletTypes!=null && AvailableBulletTypes.Count > 0) || weapManager.GeneralSecondaryAmmo.Count > 0)
            {
                if (AvailableBulletTypes.Count > 0)
                {
                    int _index = Random.Range(0, AvailableBulletTypes.Count);
                    if (AvailableBulletTypes[0])
                    {
                        ProcessAnalytics();
                         
                        Transform _spawnedBullet = PoolManager.Instance.Spawn(AvailableBulletTypes[0], FireSocket.position, FireSocket.rotation);

                        if (_spawnedBullet == null)
                        {
                            Log.Write("We got null when tried to spawn bullet!", LogColors.Yellow);
                            return;
                        }
                        else
                        {
                            PlayShotSound();
                            IBullet _bullet = _spawnedBullet.GetComponent<IBullet>();

                            BulletData _data = _bullet.GetData();
                            if (Damage > 0)
                                _data.BulletDamage = Damage;
                            _data.Owner = this.transform;
                            _bullet.SetData(_data);
                            _bullet.Launch();
                        }
                    }
                    AvailableBulletTypes.RemoveAt(_index);
                }
                else
                {
                    int _index = Random.Range(0, weapManager.GeneralSecondaryAmmo.Count);
                    if (weapManager.GeneralSecondaryAmmo[0])
                    {
                        ProcessAnalytics();
                         
                        Transform _spawnedBullet = PoolManager.Instance.Spawn(weapManager.GeneralSecondaryAmmo[0], FireSocket.position, FireSocket.rotation);

                        if(TriggerMode)
                            weapManager.GeneralSecondaryAmmo.RemoveAt(0);

                        if (_spawnedBullet == null)
                        {
                            Log.Write("We got null when tried to spawn bullet!" + weapManager.GeneralSecondaryAmmo[0], LogColors.Yellow);
                            return;
                        }
                        else
                        {
                            PlayShotSound();
                            IBullet _bullet = _spawnedBullet.GetComponent<IBullet>();

                            BulletData _data = _bullet.GetData();
                            if (Damage > 0)
                                _data.BulletDamage = Damage;
                            _data.Owner = this.transform;
                            _bullet.SetData(_data);
                            _bullet.Launch();
                        }
                    } 
                }
            }
        }
        #endregion
    }
     
    /// <summary>
    /// Enum of weapon types
    /// </summary>
    public enum WeaponType
    {
        Primary,
        Secondary,
        Additional,
    }
}