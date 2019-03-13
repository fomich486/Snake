using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InputManagerModule
{
    /// <summary>
    /// Class for mobile input with gyro
    /// </summary>
    public class MobileGyroInput : BaseInput
    {
        #region VARIABLES
        /// <summary>
        /// Flag if we are moving stick now
        /// </summary>
        private bool currentlyMoving = false;
        /// <summary>
        /// Position of the start of stick-touch
        /// </summary>
        private Vector2 moveStartPosition = Vector2.zero;
        /// <summary>
        /// Current stick-touch position
        /// </summary>
        private Vector2 moveCurrentPosition = Vector2.zero;
        /// <summary>
        /// Cashed transform of stick
        /// </summary>
        public RectTransform StickTransform;
        /// <summary>
        /// Local start position of stick
        /// </summary>
        private Vector2 stickBasePositon;
        /// <summary>
        /// Maximum distance to offset stick
        /// </summary>
        private float _maxOffsetRadius = 60f;
        /// <summary>
        /// Current vector of direction
        /// </summary>
        private Vector2 currentDirection = Vector2.zero;
        /// <summary>
        /// ID of stick-touch 
        /// </summary>
        private int touchID = -1; 

        /// <summary>
        /// Min angle to perform turn
        /// </summary>
        private float minAngle = 0.03f;
        /// <summary>
        /// Max angle to perform turn
        /// </summary>
        private float maxAngle = 0.35f; 

        /// <summary>
        /// STart rotation when game inits
        /// </summary>
        private Vector3 StartRotation = Vector3.zero; 
        /// <summary>
        /// Debug text (WILL BE DEPRECATED)
        /// </summary>
        public Text debugText1;
         
        /// <summary>
        /// Multiplier of rotation (left-right album)
        /// </summary>
        private int rotationMultiplier = 1;

        /// <summary>
        /// Flag if input component was installed
        /// </summary>
        private bool installed = false;

        /// <summary>
        /// Flag if we currently calculate swipe
        /// </summary>
        private bool countingSwipe = false;
        /// <summary>
        /// Index of finger to calculate swipe
        /// </summary>
        private int countingSwipeIndex = 0;
        /// <summary>
        /// Start swipe point
        /// </summary>
        private Vector2 countingSwipeStart = Vector3.zero;
        #endregion

        #region METHODS_INIT
        /// <summary>
        /// Called in the start of working by InputManager
        /// </summary>
        public override void Start()
        {
            UIManager.Instance.StartCoroutine(InstallLater());
        }

        /// <summary>
        /// Coroutine that isntall variables after few frames
        /// </summary>
        /// <returns></returns>
        IEnumerator InstallLater()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            InstantiateVariables();
            installed = true;
        }

        /// <summary>
        /// Call it to instantiate stick, it's variables and start parameters.
        /// </summary>
        void InstantiateVariables()
        {  
            //cashing gyro acceleration 
            StartRotation = Input.acceleration; 

            //BUTTON 1
            GameObject _button1 = null;// _gameUI.Button1.gameObject;// GameObject.Find("Button1");
            if (_button1 != null)
            {
                Button _button = _button1.GetComponent<Button>();
                if (_button != null)
                {
                    EventTrigger _stickEvents = _button.GetComponent<EventTrigger>();
                    _stickEvents.triggers.Clear();

                    //ON POINTER DOWN
                    EventTrigger.Entry _entryDown = new EventTrigger.Entry();
                    _entryDown.eventID = EventTriggerType.PointerDown;
                    _entryDown.callback.AddListener((data) => Button1.Invoke());
                    _stickEvents.triggers.Add(_entryDown);

                    //ON POINTER UP
                    EventTrigger.Entry _entryUp = new EventTrigger.Entry();
                    _entryUp.eventID = EventTriggerType.PointerUp;
                    _entryUp.callback.AddListener((data) => Button1Up.Invoke());
                    _stickEvents.triggers.Add(_entryUp);
                }
            }
            else
            {
                Log.Write("Cant find Button1", LogColors.Red);
            }

            //BUTTON 2
            GameObject _button2 = null;// _gameUI.Button2.gameObject;// GameObject.Find("Button2");
            if (_button2 != null)
            {
                Button _button = _button2.GetComponent<Button>();
                if (_button != null)
                {
                    EventTrigger _stickEvents = _button2.GetComponent<EventTrigger>();
                    _stickEvents.triggers.Clear();

                    //ON POINTER DOWN
                    EventTrigger.Entry _entryDown = new EventTrigger.Entry();
                    _entryDown.eventID = EventTriggerType.PointerDown;
                    _entryDown.callback.AddListener((data) => Button2.Invoke());
                    _stickEvents.triggers.Add(_entryDown);

                    //ON POINTER UP
                    EventTrigger.Entry _entryUp = new EventTrigger.Entry();
                    _entryUp.eventID = EventTriggerType.PointerUp;
                    _entryUp.callback.AddListener((data) => Button2Up.Invoke());
                    _stickEvents.triggers.Add(_entryUp); 
                }
            }
            else
            {
                Log.Write("Cant find Button2", LogColors.Yellow);
            }

            //BUTTON 3
            GameObject _button3 = null;//  _gameUI.Button3.gameObject;// GameObject.Find("Button3");
            if (_button3 != null)
            {
                Button _button = _button3.GetComponent<Button>();
                if (_button != null)
                {
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => Button3.Invoke());
                }
            }
            else
            {
                Log.Write("Cant find Button3", LogColors.Yellow);
            }

            Log.Write("Generated mobile");
        }
        #endregion

        #region METHODS_PERFORM

        /// <summary>
        /// Call it to check for swipe start-end
        /// </summary>
        void CheckForSwipe()
        { 
            //PC
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    countingSwipe = true;
                    countingSwipeIndex = 0;
                    countingSwipeStart = Input.mousePosition;
                }

            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (countingSwipe)
                {
                    countingSwipe = false;
                    float _diffX = Input.mousePosition.x - countingSwipeStart.x;
                    float _diffY = Input.mousePosition.y - countingSwipeStart.y;
                    if (Mathf.Abs(_diffX) > 0.3f * Screen.width)
                    {
                        if (_diffX > 0)
                        {
                            SwipeRight.Invoke();
                        }
                        else
                        {
                            SwipeLeft.Invoke();
                        }
                    }
                    else if (_diffY > 0.3f * Screen.height)
                    {
                        SwipeTop.Invoke();
                    }
                }
            }
            //MOBILE
#else
            foreach (Touch _t in Input.touches)
            {
                if (_t.phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(_t.fingerId))
                    {
                        countingSwipe = true;
                        countingSwipeIndex = _t.fingerId;
                        countingSwipeStart = _t.position;
                    }
                }
                else if (_t.phase == TouchPhase.Ended && _t.fingerId == countingSwipeIndex)
                {
                    if (countingSwipe)
                    {
                        countingSwipe = false;
                        float _diffX = Input.mousePosition.x - countingSwipeStart.x;
                        float _diffY = Input.mousePosition.y - countingSwipeStart.y;
                        if (Mathf.Abs(_diffX) > 0.1f * Screen.width)
                        {
                            if (_diffX > 0)
                            {
                                SwipeRight.Invoke();
                            }
                            else
                            {
                                SwipeLeft.Invoke();
                            }
                        }
                        else if (_diffY > 0.15f * Screen.height)
                        {
                            SwipeTop.Invoke();
                        }
                    }
                }
            } 
#endif
        }
         
        /// <summary>
        /// Called each frame by InputManager
        /// </summary>
        public override void Tick()
        {
            if (!installed)
                return;

            CheckForSwipe();

            rotationMultiplier = Input.deviceOrientation == DeviceOrientation.LandscapeLeft ? 1 : -1;

            //in any case we have to send these deletaes - even with (0,0) data
            //calling move X delegate
            MovingAxisX.Invoke(currentDirection.x * rotationMultiplier);

            //calling move Y delegate
            MovingAxisY.Invoke(currentDirection.y * rotationMultiplier);

            //processing axises by gyro
            ProcessGyroscope();
        }

        /// <summary>
        /// Call it to process gyroscope input
        /// </summary>
        void ProcessGyroscope()
        {
            Vector3 _currentEuler = Input.acceleration;// gyro.attitude.eulerAngles;

            if(debugText1!=null)
                debugText1.text = (_currentEuler - StartRotation).ToString();

            float _currentXValue = 0f;
            float _currentXAngles = GetDifference(_currentEuler.x - StartRotation.x);
            
            if(Mathf.Abs(_currentXAngles) > minAngle)
                _currentXValue = Mathf.Clamp(_currentXAngles / maxAngle, -maxAngle, maxAngle);

            float _currentYValue = 0f;
            float _currentZAngles = GetDifference(_currentEuler.y - StartRotation.y);
            if(Mathf.Abs(_currentZAngles) > minAngle)
                _currentYValue = Mathf.Clamp(_currentZAngles / maxAngle, -maxAngle, maxAngle);

            currentDirection = new Vector2(-_currentXValue/maxAngle, -_currentYValue/maxAngle);
             
            //in any case we have to send these deletaes - even with (0,0) data
            //calling move X delegate
            MovingAxisX.Invoke(currentDirection.x);

            //calling move Y delegate
            MovingAxisY.Invoke(currentDirection.y);
        } 

        /// <summary>
        /// Call it to get difference -180..180 space
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        float GetDifference(float _x)
        {
            //acceleration
            if (_x > 180)
            {
                return 360f - _x;
            }
            //braking
            return -_x; 
        }
        #endregion
    }
}