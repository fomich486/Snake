using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reverse : Bonus
{
    protected override void BonusEffect()
    {
        Settings.Instance.Snake.Translate();
        Settings.Instance.PlayerController.ReverseChangeDirection();
        Settings.Instance.Snake.MoveReverse();
    }
}
