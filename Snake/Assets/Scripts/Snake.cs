using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    [Range(2, 6)]
    private int _startLength = 3;
    [SerializeField]
    private GameObject _snakeBlock;
    private Vector2 _lastPosHead;
    private Vector2 _lastPosTail;
    private Vector2 _nextPosition;
    private bool _reverseMovement = false;

    public List<Transform> SnakeList { get; private set; }

    public void Move(Vector2 translateDirection)
    {
        NextPositionCalcumation(translateDirection);
        if (!Settings.Instance.BonusSpawner.CheckBonusList(_nextPosition) && !GameOverCheck())
        {
            Translate();
        }
    }

    public void MoveReverse()
    {
        _reverseMovement = !_reverseMovement;
    }

    private void Awake()
    {
        StartSnakeSetup();
    }
    private void StartSnakeSetup()
    {
        SnakeList = new List<Transform>();
        Vector2 startPos = Vector2.zero;
        for (int i = 0; i < _startLength; i++)
        {
            GameObject block = Instantiate(_snakeBlock, startPos, Quaternion.identity);
            SnakeList.Add(block.transform);
            startPos += Vector2.down;
        }
    }

    private void NextPositionCalcumation(Vector2 translateDirection)
    {
        if (!_reverseMovement)
        {
            _lastPosHead = SnakeList[0].position;
            _nextPosition = _lastPosHead + translateDirection;
        }
        else
        {
            _lastPosTail = SnakeList[SnakeList.Count - 1].position;
            _nextPosition = _lastPosTail + translateDirection;
        }
    }
    public void Translate()
    {
        if (!_reverseMovement)
        {
            LevelLimitsMovement(_nextPosition);
            SnakeList.Insert(0, SnakeList[SnakeList.Count - 1]);
            SnakeList.RemoveAt(SnakeList.Count - 1);
        }
        else
        {
            LevelLimitsMovement(_nextPosition);
            SnakeList.Add(SnakeList[0]);
            SnakeList.RemoveAt(0);
        }
    }
    private void LevelLimitsMovement(Vector2 nextPos)
    {
        if (Mathf.Abs(nextPos.x) == Settings.Instance.X_Border + 1)
        {
            int dirKoef = -1 * (int)(Mathf.Abs(nextPos.x) / nextPos.x);
            nextPos = new Vector2(dirKoef * Settings.Instance.X_Border, nextPos.y);
        }
        else if (Mathf.Abs(nextPos.y) == Settings.Instance.Y_Border + 1)
        {
            int dirKoef = -1 * (int)(Mathf.Abs(nextPos.y) / nextPos.y);
            nextPos = new Vector2(nextPos.x, dirKoef * Settings.Instance.Y_Border);
        }
        if (!_reverseMovement)
            SnakeList[SnakeList.Count - 1].position = nextPos;
        else
            SnakeList[0].position = nextPos;
    }

    public void AddNewElement()
    {
        GameObject block = Instantiate(_snakeBlock ,_nextPosition, Quaternion.identity);
        if (!_reverseMovement)
            SnakeList.Insert(0, block.transform);
        else
            SnakeList.Add(block.transform);
    }

    public void RemoveLastElement()
    {
        if (!_reverseMovement)
        {
            Destroy(SnakeList[SnakeList.Count - 1].gameObject);
            SnakeList.RemoveAt(SnakeList.Count - 1);
        }
        else
        {
            Destroy(SnakeList[0].gameObject);
            SnakeList.RemoveAt(0);
        }
    }
    public bool GameOverCheck()
    {
        foreach (Transform blockTransform in SnakeList)
        {
            if (blockTransform.position == (Vector3)_nextPosition)
            {
                Settings.Instance.GameOver();
                return true;
            }
        }
        return false;
    }
}