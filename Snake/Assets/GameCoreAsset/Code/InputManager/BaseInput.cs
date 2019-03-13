using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InputManagerModule
{
    /// <summary>
    /// Base class for input
    /// </summary>
    [Serializable]
    public class BaseInput
    {
        #region CALLBACKS
        private UnityFloatEvent movingAxisX;
        /// <summary>
        /// Called each frame with horizontal float data
        /// </summary>
        public UnityFloatEvent MovingAxisX
        {
            get
            {
                if (movingAxisX == null)
                    movingAxisX = new UnityFloatEvent();

                return movingAxisX;
            }
            set
            {
                movingAxisX = value;
            }
        }

        private UnityFloatEvent movingAxisY;
        /// <summary>
        /// Called each frame with vetical float data
        /// </summary>
        public UnityFloatEvent MovingAxisY
        {
            get
            {
                if (movingAxisY == null)
                    movingAxisY = new UnityFloatEvent();

                return movingAxisY;
            }
            set
            {
                movingAxisY = value;
            }
        }

        private UnityEvent button1;
        /// <summary>
        /// Called each time, when Button1 clicked
        /// </summary>
        public UnityEvent Button1
        {
            get
            {
                if (button1 == null)
                    button1 = new UnityEvent();

                return button1;
            }
            set
            {
                button1 = value;
            }
        }

        private UnityEvent button1Up;
        /// <summary>
        /// Called each time, when Button1 unclicked
        /// </summary>
        public UnityEvent Button1Up
        {
            get
            {
                if (button1Up == null)
                    button1Up = new UnityEvent();

                return button1Up;
            }
            set
            {
                button1Up = value;
            }
        }

        private UnityEvent button2;
        /// <summary>
        /// Called each time, when Button2 clicked
        /// </summary>
        public UnityEvent Button2
        {
            get
            {
                if (button2 == null)
                    button2 = new UnityEvent();

                return button2;
            }
            set
            {
                button2 = value;
            }
        }

        private UnityEvent button2Up;
        /// <summary>
        /// Called each time, when Button2 clicked
        /// </summary>
        public UnityEvent Button2Up
        {
            get
            {
                if (button2Up == null)
                    button2Up = new UnityEvent();

                return button2Up;
            }
            set
            {
                button2Up = value;
            }
        }

        private UnityEvent button3;
        /// <summary>
        /// Called each time, when Button3 clicked
        /// </summary>
        public UnityEvent Button3
        {
            get
            {
                if (button3 == null)
                    button3 = new UnityEvent();

                return button3;
            }
            set
            {
                button3 = value;
            }
        }

        private UnityEvent swipeLeft;
        /// <summary>
        /// Called each time when user swipes left
        /// </summary>
        public UnityEvent SwipeLeft
        {
            get
            {
                if (swipeLeft == null)
                    swipeLeft = new UnityEvent();

                return swipeLeft;
            }
            set
            {
                swipeLeft = value;
            }
        }

        private UnityEvent swipeRight;
        /// <summary>
        /// Called each time when user swipes right
        /// </summary>
        public UnityEvent SwipeRight
        {
            get
            {
                if (swipeRight == null)
                    swipeRight = new UnityEvent();

                return swipeRight;
            }
            set
            {
                swipeRight = value;
            }
        }

        private UnityEvent swipeTop;
        /// <summary>
        /// Called when user swipes up
        /// </summary>
        public UnityEvent SwipeTop
        {
            get
            {
                if (swipeTop == null)
                    swipeTop = new UnityEvent();

                return swipeTop;
            }
            set
            {
                swipeTop = value;
            }
        }

        private UnityEvent pause;
        /// <summary>
        /// Called when user swipes up
        /// </summary>
        public UnityEvent Pause
        {
            get
            {
                if (pause == null)
                    pause = new UnityEvent();

                return pause;
            }
            set
            {
                pause = value;
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Called in the start of lifecycle
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Called each frame
        /// </summary>
        public virtual void Tick()
        {

        }
        #endregion
    }
}