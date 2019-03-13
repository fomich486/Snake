using UnityEngine;
using PoolManagerModule;

namespace Gameplay
{
    public abstract class Bonus : MonoBehaviour
    {
        protected abstract void BonusEffect();
        [SerializeField]
        protected int points;
        [SerializeField]
        protected string bonusSoundName;

        public void OnBonusEnter()
        {
            Settings.Instance.SetScoreInGame(points);
            Settings.Instance.PlaySound(bonusSoundName);
            BonusEffect();
            Die();
        }

        protected void Die()
        {
            PoolManager.Instance.Despawn(transform);
        }
    }
}
