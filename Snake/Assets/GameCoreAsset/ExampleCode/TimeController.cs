using PoolManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public Slider RealTimeSlider;
    public Slider VirtualTimeSlider;

    public Text RealValue;
    public Text VirtualValue;

    public Transform ParticlePrefabReal;
    public Transform ParticlePrefabVirtual;

    public Transform RealSocket;
    public Transform VirtualSocket;

    private void Start()
    {
        PoolManager.Instance.Spawn(ParticlePrefabReal, RealSocket.position);
        PoolManager.Instance.Spawn(ParticlePrefabVirtual, VirtualSocket.position);
    }

    public void RealChanged()
    {
        Time.timeScale = RealTimeSlider.value;

        RealValue.text = Time.timeScale.ToString();
        VirtualValue.text = (TimeScaleManager.Instance.GetScale() * Time.timeScale).ToString();
    } 

    public  void VirtualChanged()
    {
        TimeScaleManager.Instance.SetScale(VirtualTimeSlider.value);
        VirtualValue.text = (TimeScaleManager.Instance.GetScale() * Time.timeScale).ToString();
    }
}
