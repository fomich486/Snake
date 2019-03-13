using SFXManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManagerTest : MonoBehaviour
{
    public AudioClip LoopSound;
    public AudioClip SideSound;

	// Use this for initialization
	void Start ()
    {
        SFXSourceData _data = new SFXSourceData();
        _data.Clips.Add(LoopSound);
        _data.Loop = true;
        //you can call it by name
        SFXManager.Instance.PlayFollowingSound(_data, transform, -1, "player3d");
        //SFXManager.Instance.SwitchMusicTheme("idle");
        //or by id
        //SFXManager.Instance.PlayFollowingSound(_data, transform, 0);

        //or just get first
        //SFXManager.Instance.PlayFollowingSound(_data, transform);

        StartCoroutine(AnimateSideSounds());
    }

    IEnumerator AnimateSideSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(6f);
            //you can play global 2d sounds like blow
            SFXManager.Instance.PlaySound(SideSound);
        }
    } 
}
