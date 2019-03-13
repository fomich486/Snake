using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class UIScoreManager : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Text finalScoreText;

        public void SetScoreText(int _currentScore)
        {
            scoreText.text = string.Format("Score : {0}", _currentScore);
        }

        public void ShowFinalScore(int _currentScore)
        {
            finalScoreText.text = string.Format("Your final score {0}", _currentScore);
        }
    }
}
