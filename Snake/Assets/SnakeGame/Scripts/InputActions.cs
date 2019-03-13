using UnityEngine;
using InputManagerModule;

namespace InputManagerModule
{
    public class InputActions : MonoBehaviour
    {
        private Vector2 moveDirection = Vector2.up;
        public Vector2 MoveDirection { get { return moveDirection; } private set { moveDirection = value; } }
        private void Start()
        {
            InputManager.Instance.MovingAxisX.AddListener(ChangeX);
            InputManager.Instance.MovingAxisY.AddListener(ChangeY);
        }

        private void ChangeX(float _x)
        {
            if (_x != 0)
                MoveDirection = new Vector2(_x, 0f).normalized;
        }

        private void ChangeY(float _y)
        {
            if (_y != 0)
                MoveDirection = new Vector2(0f, _y).normalized;
        }

        public void ReverseChangeDirection()
        {
            moveDirection *= -1;
        }
    }
}
