using DataSystem;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.Events;

namespace InputManagerModule
{
    /// <summary>
    /// Global input manager class. Used to AddListener to main input events without thinking about platform choose
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        #region CALLBACKS 
        /// <summary>
        /// Called each frame with horizontal float data
        /// </summary> 
        public UnityFloatEvent MovingAxisX
        {
            get
            {
                if (currentInput.MovingAxisX == null)
                    currentInput.MovingAxisX = new UnityFloatEvent();

                return currentInput.MovingAxisX;
            }
            set
            {
                currentInput.MovingAxisX = value;
            }
        }
        /// <summary>
        /// Called each frame with vetical float data
        /// </summary>
        public UnityFloatEvent MovingAxisY
        {
            get
            {
                if (currentInput.MovingAxisY == null)
                    currentInput.MovingAxisY = new UnityFloatEvent();

                return currentInput.MovingAxisY;
            }
            set
            {
                currentInput.MovingAxisY = value;
            }
        }
        /// <summary>
        /// Called each time, when Button1 clicked
        /// </summary>
        public UnityEvent Button1
        {
            get
            {
                return currentInput.Button1;
            }
            set
            {
                currentInput.Button1 = value;
            }
        }
        /// <summary>
        /// Called each time, when Button1 clicked
        /// </summary>
        public UnityEvent Button1Up
        {
            get
            {
                return currentInput.Button1Up;
            }
            set
            {
                currentInput.Button1Up = value;
            }
        }
        /// <summary>
        /// Called each time, when Button2 clicked
        /// </summary>
        public UnityEvent Button2
        {
            get
            {
                return currentInput.Button2;
            }
            set
            {
                currentInput.Button2 = value;
            }
        }
        public UnityEvent Button2Up
        {
            get
            {
                return currentInput.Button2Up;
            }
            set
            {
                currentInput.Button2Up = value;
            }
        }
        /// <summary>
        /// Called each time, when Button3 clicked
        /// </summary>
        public UnityEvent Button3
        {
            get
            {
                return currentInput.Button3;
            }
            set
            {
                currentInput.Button3 = value;
            }
        }
        /// <summary>
        /// Called each time when user makes left swipe
        /// </summary>
        public UnityEvent SwipeLeft
        {
            get
            {
                return currentInput.SwipeLeft;
            }
            set
            {
                currentInput.SwipeLeft = value;
            }
        }
        /// <summary>
        /// Called each time when user makes right swipe
        /// </summary>
        public UnityEvent SwipeRight
        {
            get
            {
                return currentInput.SwipeRight;
            }
            set
            {
                currentInput.SwipeRight = value;
            }
        }
        /// <summary>
        /// Called when user swipes up
        /// </summary>
        public UnityEvent SwipeTop
        {
            get
            { 
                return currentInput.SwipeTop;
            }
            set
            {
                currentInput.SwipeTop = value;
            }
        }
        /// <summary>
        /// Called when user pressed P
        /// </summary>
        public UnityEvent PauseCall
        {
            get
            {
                return currentInput.Pause;
            }
            set
            {
                currentInput.Pause = value;
            }
        }
        #endregion

        #region VARIABLES 
        /// <summary>
        /// If input have to be by mouse. It's not recommented, cause in editor it's hard to do something
        /// </summary>
        public bool MouseInput = false;
        /// <summary>
        /// Input instance, that generates in the start of lifecycle
        /// </summary>
        private BaseInput currentInput;
        /// <summary>
        /// Enum of current input. You can use this info for your logic in game
        /// </summary>
        public InputType CurrentInp;
        #endregion

        #region METHODS
        /// <summary>
        /// Called in the start of work
        /// </summary>
        private void Awake()
        {
            base.Awake();
            BaseInstall();
        } 

        /// <summary>
        /// Call it to instantiate control
        /// </summary>
        void BaseInstall()
        {
            //choosing device 

#if UNITY_EDITOR || UNITY_5 || UNITY_EDITOR_WIN || UNITY_STANDALONE
            switch (CurrentInp)
            {
                case InputType.PCKeyBoard:
                    currentInput = new PCInputKeyBoard();
                    break;
                case InputType.PCMouse:
                    currentInput = new PCInputMouse();
                    break;
                default:
                    currentInput = new PCInputKeyBoard();
                    break;
            }
            Log.Write("Creation " + currentInput);
#else
            if (/*GyroscopeInput && */SystemInfo.supportsGyroscope)
            {
                Log.Write("Creation gyro input");
                currentInput = new MobileGyroInput();
                CurrentInp = InputType.MobileGyro;
            }
            else
            {
                //Log.Write("Creation mobile input. GyroscopeInput value:"+GyroscopeInput);
                currentInput = new MobileInput();
                CurrentInp = InputType.MobileStick;
            }
#endif
            //calling start in input instance to install everything we need
            currentInput.Start();
        }

        /// <summary>
        /// Call it to reinig controls
        /// </summary>
        public void Reinit()
        {
            //reset mobile options

            MobileInput _mi = currentInput as MobileInput;
            if (_mi != null)
            {
                _mi.ResetAll();
            }

            BaseInstall();
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            //calling tick in our input instance
            if (currentInput != null)
                currentInput.Tick();
        }

        /// <summary>
        /// Call it to execute some coroutine
        /// </summary>
        /// <param name="_cor"></param>
        public void Execute(IEnumerator _cor)
        {
            StartCoroutine(_cor);
        }
#endregion
    }
    
    /// <summary>
    /// Enum of available input types. put here any you want
    /// </summary>
    public enum InputType
    {
        MobileStick,
        MobileGyro,
        PCKeyBoard,
        PCMouse,
    }
}
