using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    public void SetScoreText(int currentScore)
    {
        _scoreText.text =string.Format("Score : {0}",currentScore);
    }
}
