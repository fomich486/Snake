using System.Collections;
using UnityEngine;
using UISystem;
using InputManagerModule;

namespace Gameplay
{
    public class Settings : Singleton<Settings>
    {
        public UIScoreManager UiScoreManager;
        public Snake Snake;
        public BonusSpawner BonusSpawner;
        public InputActions InputActions;
        [Header("Level limits")]
        public int X_Border = 20;
        public int Y_Border = 12;
        [SerializeField]
        private GameObject _gameField;
        private float _fieldLimitCoef = 1f;
        [Header("Update setup")]
        [SerializeField]
        [Range(0.01f, 1f)]
        public float UpdateDelay = 0.3f;
        [Header("Score")]
        private int score = 0;

        private void Start()
        {
            GameObject field = Instantiate(_gameField, Vector3.zero, Quaternion.identity);
            field.transform.localScale = new Vector3(2f * X_Border + _fieldLimitCoef, 2f * Y_Border + _fieldLimitCoef);
        }

        public void GameSpeedControl(float modifier, float duration)
        {
            StartCoroutine(ChangeGameSpeed(modifier, duration));
        }

        IEnumerator ChangeGameSpeed(float modifier, float duration)
        {
            UpdateDelay *= modifier;
            yield return new WaitForSeconds(duration);
            UpdateDelay /= modifier;
        }

        public void GameOver()
        {
            PoolManagerModule.PoolManager.Instance.DespawnAll();
            UISystem.UIManager.Instance.ShowPage("Gameover");
            ShowScoreGameover();
        }

        public void SetScoreInGame(int _pointsAdd)
        {
            score += _pointsAdd;
            UiScoreManager.SetScoreText(score);
        }

        public void ShowScoreGameover()
        {
            UiScoreManager.ShowFinalScore(score);
        }

        public void ResetScore()
        {
            score = 0;
            SetScoreInGame(0);
        }

        public void PlaySound(string _soundName)
        {
            var _clip = Resources.Load(_soundName) as AudioClip;
            SFXManagerModule.SFXManager.Instance.PlaySound(_clip);
        }
    }
}
