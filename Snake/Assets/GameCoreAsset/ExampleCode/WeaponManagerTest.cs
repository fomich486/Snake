using InputManagerModule;
using UnityEngine;
using WeaponSystem;

public class WeaponManagerTest : MonoBehaviour
{
    public WeaponManager Manager;

    private void Start()
    {
        InputManager.Instance.Button1.AddListener(Manager.StartPrimaryFire);
        InputManager.Instance.Button1Up.AddListener(Manager.EndPrimaryFire);
        InputManager.Instance.Button2.AddListener(Manager.StartSecondaryFire);
        InputManager.Instance.Button2Up.AddListener(Manager.EndSecondaryFiring);
        InputManager.Instance.Button3.AddListener(Manager.StartAdditionalFire);
    }
}
