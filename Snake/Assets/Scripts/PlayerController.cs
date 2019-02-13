using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _nextUpdateTime = 0f;
    private Vector2 _moveDirection;

    private void Start()
    {
        _moveDirection = Vector2.up;
    }

    private void Update()
    {
        PlayerInput();
        SnakeUpdate();
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _moveDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _moveDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _moveDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _moveDirection = Vector2.right;
        }
    }

    private void SnakeUpdate()
    {
        if (Time.time > _nextUpdateTime)
        {
            Settings.Instance.Snake.Move(_moveDirection);
            _nextUpdateTime = Time.time + Settings.Instance.UpdateDelay;
        }
    }

    public void ReverseChangeDirection()
    {
        _moveDirection *= -1;
    }
}
