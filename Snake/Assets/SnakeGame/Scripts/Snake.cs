using System.Collections.Generic;
using UnityEngine;
using PoolManagerModule;

namespace Gameplay
{
    public class Snake : MonoBehaviour
    {
        [SerializeField]
        [Range(2, 6)]
        private int startLength = 3;
        [SerializeField]
        private GameObject snakeBlock;
        private Vector2 lastPosHead;
        private Vector2 lastPosTail;
        private Vector2 nextPosition;
        private bool reverseMovement = false;
        private float nextUpdateTime = 0f;
        public List<Transform> SnakeList { get; private set; }

        private void OnEnable()
        {
            StartSnakeSetup();
        }

        private void OnDisable()
        {
            foreach (Transform _snakeBlock in SnakeList)
            {
                PoolManager.Instance.Despawn(_snakeBlock.transform, true);
            }
            SnakeList.Clear();
        }

        private void StartSnakeSetup()
        {
            SnakeList = new List<Transform>();
            Vector2 startPos = Vector2.zero;
            for (int i = 0; i < startLength; i++)
            {
                Transform block = PoolManager.Instance.Spawn(snakeBlock.transform, startPos, Quaternion.identity);
                SnakeList.Add(block);
                startPos += Vector2.down;
            }
        }

        private void Update()
        {
            SnakeUpdate();
        }

        private void SnakeUpdate()
        {
            if (Time.time > nextUpdateTime)
            {
                Move(Settings.Instance.InputActions.MoveDirection);
                Settings.Instance.PlaySound("Move");
                nextUpdateTime = Time.time + Settings.Instance.UpdateDelay;
            }
        }

        public void Move(Vector2 _translateDirection)
        {
            NextPositionCalcumation(_translateDirection);
            if (!Settings.Instance.BonusSpawner.CheckBonusList(nextPosition) && !GameOverCheck())
            {
                Translate();
            }
        }

        public void MoveReverse()
        {
            reverseMovement = !reverseMovement;
        }

        private void NextPositionCalcumation(Vector2 _translateDirection)
        {
            if (!reverseMovement)
            {
                lastPosHead = SnakeList[0].position;
                nextPosition = lastPosHead + _translateDirection;
            }
            else
            {
                lastPosTail = SnakeList[SnakeList.Count - 1].position;
                nextPosition = lastPosTail + _translateDirection;
            }
        }
        public void Translate()
        {
            if (!reverseMovement)
            {
                LevelLimitsMovement(nextPosition);
                SnakeList.Insert(0, SnakeList[SnakeList.Count - 1]);
                SnakeList.RemoveAt(SnakeList.Count - 1);
            }
            else
            {
                LevelLimitsMovement(nextPosition);
                SnakeList.Add(SnakeList[0]);
                SnakeList.RemoveAt(0);
            }
        }
        private void LevelLimitsMovement(Vector2 _nextPos)
        {
            if (Mathf.Abs(_nextPos.x) == Settings.Instance.X_Border + 1)
            {
                int dirKoef = -1 * (int)(Mathf.Abs(_nextPos.x) / _nextPos.x);
                _nextPos = new Vector2(dirKoef * Settings.Instance.X_Border, _nextPos.y);
            }
            else if (Mathf.Abs(_nextPos.y) == Settings.Instance.Y_Border + 1)
            {
                int dirKoef = -1 * (int)(Mathf.Abs(_nextPos.y) / _nextPos.y);
                _nextPos = new Vector2(_nextPos.x, dirKoef * Settings.Instance.Y_Border);
            }
            if (!reverseMovement)
                SnakeList[SnakeList.Count - 1].position = _nextPos;
            else
                SnakeList[0].position = _nextPos;
        }

        public void AddNewElement()
        {
            GameObject block = Instantiate(snakeBlock, nextPosition, Quaternion.identity);
            if (!reverseMovement)
                SnakeList.Insert(0, block.transform);
            else
                SnakeList.Add(block.transform);
        }

        public void RemoveLastElement()
        {
            if (!reverseMovement)
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
                if (blockTransform.position == (Vector3)nextPosition)
                {
                    Settings.Instance.GameOver();
                    return true;
                }
            }
            return false;
        }
    }
}