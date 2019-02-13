using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadFood : Bonus
{
    protected override void BonusEffect()
    {
        Settings.Instance.Snake.Translate();
        Settings.Instance.Snake.RemoveLastElement();
    }
}
