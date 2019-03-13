using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputManagerModule
{
    /// <summary>
    /// Class for performing pc input
    /// </summary>
    public class PCInputKeyBoard : BaseInput
    { 
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
            //VERTICAL AXIS
            if (Input.GetKey(KeyCode.UpArrow))
            { 
                MovingAxisY.Invoke(1);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                MovingAxisY.Invoke(-1);
            }
            else
            {
                MovingAxisY.Invoke(0);
            }

            //HORIZONTAL AXIS
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MovingAxisX.Invoke(-1);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                MovingAxisX.Invoke(1);
            }
            else
            {
                MovingAxisX.Invoke(0);
            }
        }

        /// <summary>
        /// Call it to process buttons
        /// </summary>
        void ProcessButtons()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Button1.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                Button1Up.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Button2.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Button2Up.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.LeftAlt))
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