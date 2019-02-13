using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Bonus
{
    protected override void BonusEffect()
    {
        Settings.Instance.Snake.AddNewElement();
    }
}
