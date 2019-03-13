using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// easy to use dictionary with paths, allows you to get some gameobject from resources ASAP without writing whole path
/// </summary>
public class ResourcesLoadManager : Singleton<ResourcesLoadManager>
{
    #region VARIABLES
    /// <summary>
    /// Path to player's planes prefabs
    /// </summary>
    public string PlayerPlanesPath = "Prefabs/Player/Planes/";
    /// <summary>
    /// Path to weapons prefabs
    /// </summary>
    public string WeaponsPath = "Prefabs/Weapons/";
    /// <summary>
    /// Path to bullets prefabs
    /// </summary>
    public string BulletPath = "Prefabs/Weapons/Bullet/";
    /// <summary>
    /// Path to planes schemas
    /// </summary>
    public string PlaneSchemaPath = "Prefabs/UI/PlaneSketches/";
    #endregion

    #region METHODS
    /// <summary>
    /// Call this function to load player's plane model
    /// </summary>
    /// <param name="_planeName">Name of prefab</param>
    /// <returns></returns>
    public GameObject GetPlane(string _planeName)
    {
        GameObject _loaded = Resources.Load(PlayerPlanesPath + _planeName) as GameObject;
        if (_loaded == null)
            Log.Write("There is no plane by this path:" + PlayerPlanesPath + _planeName,LogColors.Red);

        return _loaded;
    }

    /// <summary>
    /// Call this function to load player's weapon
    /// </summary>
    /// <param name="_weaponName"></param>
    /// <returns></returns>
    public GameObject GetWeapon(string _weaponName)
    {
        GameObject _loaded = Resources.Load(WeaponsPath + _weaponName) as GameObject;
        if (_loaded == null)
            Log.Write("There is no weapon by this path:" + WeaponsPath + _weaponName,LogColors.Red);

        return _loaded;
    } 

    /// <summary>
    /// Call it to load bullet template
    /// </summary>
    /// <param name="_bulletName">Name of bullet</param>
    /// <returns>Bullet's prefab</returns>
    public GameObject GetBullet(string _bulletName)
    {
        GameObject _loaded = Resources.Load(BulletPath + _bulletName) as GameObject;
        if (_loaded == null)
            Log.Write("There is no bullet by this path:" + BulletPath + _bulletName,LogColors.Red);

        return _loaded;
    }

    /// <summary>
    /// Call it to load plane schema from prefabs
    /// </summary>
    /// <param name="_schemaName"></param>
    /// <returns></returns>
    public GameObject GetPlaneSchema(string _schemaName)
    {
        GameObject _schema = Resources.Load(PlaneSchemaPath + _schemaName) as GameObject;
        if (_schema == null)
            Log.Write("There is no schema by this path:" + PlaneSchemaPath + _schemaName, LogColors.Yellow);

        return _schema;
    }
    #endregion
}
