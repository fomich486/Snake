using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bonus : MonoBehaviour
{
    protected abstract void BonusEffect();
    [SerializeField]
    protected int _points;
    public void OnBonusEnter()
    {
        Settings.Instance.SetScore(_points);
        BonusEffect();
        Die();
    }
    protected void Die()
    {
        Destroy(gameObject);
    }
}
