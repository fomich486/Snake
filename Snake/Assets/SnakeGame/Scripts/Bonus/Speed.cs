using UnityEngine;

namespace Gameplay
{
    public class Speed : Bonus
    {
        [SerializeField]
        [Range(0.1f, 5f)]
        private float gameSpeedModifier;
        [SerializeField]
        [Range(0.1f, 5f)]
        private float duration;
        protected override void BonusEffect()
        {
            Settings.Instance.Snake.Translate();
            Settings.Instance.GameSpeedControl(gameSpeedModifier, duration);
        }
    }
}
