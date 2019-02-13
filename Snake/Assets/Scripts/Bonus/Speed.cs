using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : Bonus
{
    [SerializeField]
    [Range(0.1f, 5f)]
    private float _gameSpeedModifier;
    [SerializeField]
    [Range(0.1f, 5f)]
    private float _duration;
    protected override void BonusEffect()
    {
        Settings.Instance.Snake.Translate();
        Settings.Instance.GameSpeedControl(_gameSpeedModifier, _duration);
    }
}
