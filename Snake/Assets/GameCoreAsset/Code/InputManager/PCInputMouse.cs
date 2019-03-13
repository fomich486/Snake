using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputManagerModule
{
    /// <summary>
    /// Class for performing pc input
    /// </summary>
    public class PCInputMouse : BaseInput
    { 
        private float Sensitivity = 1.3f;

        private Vector2 CurrentDirection = Vector2.zero;
        /// <summary>
        /// Called once per frame
        /// </summary>
        public override void Tick()
        {
            ProcessDirection();
            ProcessButtons();
        }

        /// <summary>
        /// Call it to process direction
        /// </summary>
        void ProcessDirection()
        {
            CurrentDirection = (CurrentDirection + new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))* Sensitivity * TimeScaleManager.Instance.GetDelta());
            CurrentDirection = new Vector2(Mathf.Clamp(CurrentDirection.x, -1, 1), Mathf.Clamp(CurrentDirection.y, -1, 1));
            MovingAxisX.Invoke(CurrentDirection.x);
            MovingAxisY.Invoke(CurrentDirection.y); 
        }

        /// <summary>
        /// Call it to process buttons
        /// </summary>
        void ProcessButtons()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Button1.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Button1Up.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Button2.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                Button2Up.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                Button3.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwipeLeft.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SwipeRight.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwipeTop.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause.Invoke();
            }
        }
    }
}