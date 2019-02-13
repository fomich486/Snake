using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public UI UI;
    public Snake Snake;
    public BonusSpawner BonusSpawner;
    public PlayerController PlayerController;
    public static Settings Instance;
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
    private int _score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GameObject field = Instantiate(_gameField, Vector3.zero, Quaternion.identity);
        field.transform.localScale = new Vector3(2f * X_Border + _fieldLimitCoef, 2f * Y_Border + _fieldLimitCoef);
        SetScore(0);
    }

    public void GameSpeedControl(float modifier,float duration)
    {
        StartCoroutine(ChangeGameSpeed(modifier, duration));
    }

    IEnumerator ChangeGameSpeed(float modifier,float duration)
    {
        UpdateDelay *= modifier;
        yield return new WaitForSeconds(duration);
        UpdateDelay /= modifier;
    }
    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }
    public void SetScore(int pointsAdd)
    {
        _score += pointsAdd;
        UI.SetScoreText(_score);
        if (_score < 0)
            GameOver();
    }
}
