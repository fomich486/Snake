using PoolManagerModule;
using SFXManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    /// <summary>
    /// Class of weapons manager
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        #region VARIABLES
        /// <summary>
        /// List of primary Weapon Modules
        /// </summary>
        public List<WeaponModule> PrimaryWeapons = new List<WeaponModule>();
        /// <summary>
        /// Sockets for primary sockets 
        /// </summary>
        public List<Transform> PrimaryWeaponsSockets = new List<Transform>();
        /// <summary>
        /// Count of secondary base capacity
        /// </summary>
        public int GeneralSecondaryCount = 0;
        /// <summary>
        /// Count of secondary base capacity
        /// </summary>
        private Transform GeneralBulletTemp;
        /// <summary>
        /// Flag if we already set up base count
        /// </summary>
        private bool baseCountSet = false;
        /// <summary>
        /// List of ammo for secondary weapons
        /// </summary>
        public List<Transform> GeneralSecondaryAmmo = new List<Transform>();
        /// <summary>
        /// Sockets for secondary weapons
        /// </summary>
        public List<Transform> SecondaryWeaponsSockets = new List<Transform>();
        /// <summary>
        /// List of secondary WeaponModule
        /// </summary>
        public List<WeaponModule> SecondaryWeapons = new List<WeaponModule>();
        /// <summary>
        /// Flag of firing together(the same time) form all secondary weapons
        /// </summary>
        public bool FireTogetherSecondary = true;
        /// <summary>
        /// Flag of general ammo for secondary ammo
        /// </summary>
        public bool GeneralAmmoSecondary = true;
        /// <summary>
        /// Flag if we have to play shoot sound only once
        /// </summary>
        public bool PlaySoundPerGroup = false;
        /// <summary>
        /// Index of current secondary weapon
        /// </summary>
        private int currentSecondaryWeaponIndex = 0; 
        /// <summary>
        /// Weapon Module for additional weapon (bombthrower)
        /// </summary>
        public WeaponModule AdditionalWeapon;
        /// <summary>
        /// Socket for additional weapon
        /// </summary>
        public Transform AdditionalWeaponSocket;
        /// <summary>
        /// Sfx player for weapon manager (for all of them)
        /// </summary>
        private SFXPlayer sfxPlayer;
        #endregion

        #region METHODS

        /// <summary>
        /// Called in the start
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(WaitForInstall());
        }

        /// <summary>
        /// Called when disabled
        /// </summary>
        private void OnDisable()
        {
            if (sfxPlayer != null)
            {
                sfxPlayer.Disable();
            }
        }

        /// <summary>
        /// Coroutine that wait for installing. If you dont need this delay - remove it
        /// </summary>
        /// <returns></returns>
        public IEnumerator WaitForInstall()
        { 
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            InstallPlayer();
        }
        
        /// <summary>
        /// Call it to install SFXPlayer for weapons
        /// </summary>
        void InstallPlayer()
        { 
            //only if you need one player per all weapons
            if (PlaySoundPerGroup)
            {
                SFXSourceData _pl = new SFXSourceData();
                SFXPlayer _player = SFXManager.Instance.PlayFollowingSound(_pl,this.transform);

                if (PrimaryWeapons != null)
                {
                    for (int i = 0; i < PrimaryWeapons.Count; i++)
                    {
                        PrimaryWeapons[i].SfxPlayer = _player;
                    }
                }
                if (SecondaryWeapons != null)
                {
                    for (int i = 0; i < SecondaryWeapons.Count; i++)
                    {
                        SecondaryWeapons[i].SfxPlayer = _player;
                    }
                }
                if (AdditionalWeapon != null)
                {
                    AdditionalWeapon.SfxPlayer = _player;
                }

                sfxPlayer = _player;
            }
        }

        /// <summary>
        /// Call it to make primary fire
        /// </summary>
        public void StartPrimaryFire()
        { 
            foreach(WeaponModule _module in PrimaryWeapons)
            { 
                _module.StartFiring(); 
            }
        }

        /// <summary>
        /// Call it to start fire from weapon directly(by it's index)
        /// </summary>
        /// <param name="_index"></param>
        public void StartPrimaryFire(int _index)
        {
            if (PrimaryWeapons.Count > _index)
            {
                PrimaryWeapons[_index].StartFiring();
            }
        }

        /// <summary>
        /// Call it to stop primary fire
        /// </summary>
        public void EndPrimaryFire()
        { 
            foreach (WeaponModule _module in PrimaryWeapons)
            {
                _module.EndFiring();
            }
        }

        /// <summary>
        /// Call it to end primary fire directly(by index)
        /// </summary>
        /// <param name="_index"></param>
        public void EndPrimaryFire(int _index)
        {
            if (PrimaryWeapons.Count > _index)
            {
                PrimaryWeapons[_index].EndFiring();
            }
        }

        /// <summary>
        /// Call this function to shoot from secondary weapons
        /// </summary>
        public void StartSecondaryFire()
        { 
            if (SecondaryWeapons.Count <= 0)
                return;
                
            //if we have to fire from all weapons in one time
            if (FireTogetherSecondary)
            { 
                foreach (WeaponModule _module in SecondaryWeapons)
                {
                    //if we have general ammo for secondary weapons
                    if (GeneralAmmoSecondary)
                    { 
                        if (_module.CanFire() && GeneralSecondaryAmmo.Count > 0)
                        { 
                            if (_module.TriggerMode)
                            {
                                //Log.Write("Started firing from cannon");
                                _module.StartFiring();
                            }
                            else
                            {
                                _module.MakeShot(GeneralSecondaryAmmo[0]);
                                GeneralSecondaryAmmo.RemoveAt(0);
                            }
                        }
                    }
                    //else - start firing and let weapon make decision can it fire or not itself
                    else
                    {
                        _module.StartFiring();
                    }
                }
            }
            //if we have to use queue fire
            else
            {
                //if we have general ammo
                if (GeneralAmmoSecondary)
                {
                    if (SecondaryWeapons[currentSecondaryWeaponIndex].CanFire() && GeneralSecondaryAmmo.Count > 0)
                    {
                        //queue firing from secondary weapons
                        if (SecondaryWeapons[currentSecondaryWeaponIndex].TriggerMode)
                        {
                            foreach(WeaponModule _wm in SecondaryWeapons)
                            {
                                _wm.StartFiring();
                            }
                        }
                        else
                        { 
                            SecondaryWeapons[currentSecondaryWeaponIndex].StartFiring(GeneralSecondaryAmmo[0]);
                            GeneralSecondaryAmmo.RemoveAt(0);

                            currentSecondaryWeaponIndex++;

                            if (currentSecondaryWeaponIndex >= SecondaryWeapons.Count)
                                currentSecondaryWeaponIndex = 0;
                        }
                    }
                }
                //if each weapon has itself ammo
                else
                {
                    if (SecondaryWeapons[currentSecondaryWeaponIndex].CanFire())
                    {
                        SecondaryWeapons[currentSecondaryWeaponIndex].StartFiring();

                        currentSecondaryWeaponIndex++;

                        if (currentSecondaryWeaponIndex >= SecondaryWeapons.Count)
                            currentSecondaryWeaponIndex = 0;
                    }
                }
            }
        } 

        /// <summary>
        /// Call it to end secondary firing
        /// </summary>
        public void EndSecondaryFiring()
        {
            foreach (WeaponModule _mod in SecondaryWeapons)
            {
                _mod.EndFiring();
            }
        }

        /// <summary>
        /// Call it to start additional fire
        /// </summary>
        public void StartAdditionalFire()
        {
            if (AdditionalWeapon == null)
                return;

            AdditionalWeapon.StartFiring();
        }

        /// <summary>
        /// Call it to get current overheating coefficient
        /// </summary>
        /// <returns></returns>
        public float GetPrimaryOverheating()
        {
            if (PrimaryWeapons.Count > 0)
            {
                return PrimaryWeapons[0].GetOverheating();
            }
            return 0f;
        }

        /// <summary>
        /// Call it to add ammo to additional weapon
        /// </summary>
        /// <param name="_template"></param>
        /// <param name="_count"></param>
        public void AddAdditionalAmmo(Transform _template, int _count = 1)
        {
            if (AdditionalWeapon == null)
                return;

            if (!AdditionalWeapon.BaseSetUp)
            {
                AdditionalWeapon.BaseSetUp = true;
                AdditionalWeapon.BaseCount = _count;
                AdditionalWeapon.BulletBase = _template;
            }
            for(int i = 0; i < _count; i++)
            {
                AdditionalWeapon.AvailableBulletTypes.Add(_template);
            }
        }

        /// <summary>
        /// Call it to get all secondary ammo. If GeneralAmmoSecondary == false - it will return summ of all ammos in all Secondary Weapon Modules
        /// </summary>
        /// <returns></returns>
        public int GetSecondaryAmmoCount()
        {
            if (GeneralAmmoSecondary)
            {
                return GeneralSecondaryAmmo.Count;
            }
            else
            {
                int _summ = 0;
                foreach(WeaponModule _mod in SecondaryWeapons)
                {
                    _summ += _mod.AvailableBulletTypes.Count;
                }
                return _summ;
            }
        }

        /// <summary>
        /// Call it to restore ammo by some %
        /// </summary>
        /// <param name="_value"></param>
        public bool RestoreAmmo(float _value)
        {
            bool _added = false;
            //SECONDARY
            if (GeneralSecondaryCount > 0)
            {
                int _toAdd = (int) (GeneralSecondaryCount * _value);
                if(GeneralSecondaryAmmo.Count + _toAdd > GeneralSecondaryCount)
                {
                    _toAdd = GeneralSecondaryCount - GeneralSecondaryAmmo.Count;
                }
                if (_toAdd > 0)
                    _added = true;

                AddSecondaryAmmo(GeneralBulletTemp, _toAdd);
            }
            //ADDITIONAL
            if (AdditionalWeapon!= null && AdditionalWeapon.BaseCount > 0)
            {
                int _toAdd = (int)(AdditionalWeapon.BaseCount * _value);
                if (AdditionalWeapon.AvailableBulletTypes.Count + _toAdd > AdditionalWeapon.BaseCount)
                {
                    _toAdd = AdditionalWeapon.BaseCount - AdditionalWeapon.AvailableBulletTypes.Count;
                }
                if (_toAdd > 0)
                    _added = true;
                AddAdditionalAmmo(AdditionalWeapon.BulletBase, _toAdd);
            }
            return _added;
        }

        /// <summary>
        /// Call it to get additional ammo count
        /// </summary>
        /// <returns></returns>
        public int GetAdditionalAmmoCount()
        {
            if (AdditionalWeapon == null)
                return 0;

            return AdditionalWeapon.AvailableBulletTypes.Count;
        }

        /// <summary>
        /// Call it to add secondary ammo
        /// </summary>
        /// <param name="_template">Template to add(rocket template(prefab))</param>
        /// <param name="_count">Count of templates to add</param>
        public void AddSecondaryAmmo(Transform _template, int _count = 1)
        {
            if (_template != null && SecondaryWeapons.Count > 0 && SecondaryWeapons[0]!=null)
            {
                if (!PoolManager.Instance.Templates.Contains(_template))
                {
                    int _spawnCount = (int)(SecondaryWeapons[0].FireRate / 4);
                    if (_spawnCount < 10)
                        _spawnCount = 10;

                    PoolManager.Instance.AddToPool(_template, _spawnCount);
                }
            }
            else
            {
                Log.Write("Cant instantiate bullets in pool manager, cause there is no tempalate/secondary weapons/secondaryweapon[0] == null", LogColors.Red);
            }

            //if we have general ammo - just add it directly
            if (GeneralAmmoSecondary)
            {
                if (!baseCountSet)
                {
                    baseCountSet = true;
                    GeneralSecondaryCount = _count;
                    GeneralBulletTemp = _template;
                }
                Log.Write("Base count set to:" + _count);
                for (int i = 0; i < _count; i++)
                {
                    GeneralSecondaryAmmo.Add(_template);
                }
                return;
            }

            //else - add it to the most low capacity weapon
            for(int j=0;j<_count;j++)
            {
                int _indexLowestCapacity = -1;
                int _lowestCapacity = 999999999;
                for(int i=0;i<SecondaryWeapons.Count;i++)
                {
                    if (SecondaryWeapons[i].AvailableBulletTypes.Count< _lowestCapacity)
                    {
                        _lowestCapacity = SecondaryWeapons[i].AvailableBulletTypes.Count;
                        _indexLowestCapacity = i;
                    }
                } 

                if (_indexLowestCapacity >= 0)
                {
                    SecondaryWeapons[_indexLowestCapacity].AvailableBulletTypes.Add(_template);
                }
            }
        }
        #endregion
    }
}