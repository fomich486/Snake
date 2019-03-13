using DataSystem;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InputManagerModule
{
    /// <summary>
    /// Class for mobile input
    /// </summary>
    public class MobileInput : BaseInput
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
        /// Cashed transform of base
        /// </summary>
        public RectTransform StickBaseTransform;
        /// <summary>
        /// Base transform position (StickBaseTransform).anchoredPosition
        /// </summary>
        private Vector3 StickBaseTransformPositon;
        /// <summary>
        /// Cashed transform of imitator
        /// </summary>
        public RectTransform StickImitatorTransform;
        /// <summary>
        /// Local start position of stick
        /// </summary>
        private Vector2 stickBasePositon;
        /// <summary>
        /// Maximum distance to offset stick
        /// </summary>
        private float _maxOffsetRadius = .1f;
        /// <summary>
        /// Current vector of direction
        /// </summary>
        private Vector2 currentDirection = Vector2.zero;
        /// <summary>
        /// ID of stick-touch 
        /// </summary>
        private int touchID = -1;
        /// <summary>
        /// Image of stick base
        /// </summary>
        private Image stickBase;
        /// <summary>
        /// Button of stick base
        /// </summary>
        private Button stickBaseButton;
        /// <summary>
        /// Flag from saves if floating control turned on
        /// </summary>
        private bool floatingControl = false;
        /// <summary>
        /// Flag if this component was installed
        /// </summary>
        private bool installed = false;
        /// <summary>
        /// Flag if swipe started counting
        /// </summary>
        private bool countingSwipe = false;
        /// <summary>
        /// index of counting swipe
        /// </summary>
        private int countingSwipeIndex = 0;
        /// <summary>
        /// Start position of counting swipe
        /// </summary>
        private Vector2 countingSwipeStart = Vector3.zero;
        #endregion

        #region METHODS_INIT
        /// <summary>
        /// Called in the start of working by InputManager
        /// </summary>
        public override void Start()
        {
            InputManager.Instance.StartCoroutine(InstallLater());
        }

        /// <summary>
        /// Call it to install stick with delay of few frames
        /// </summary>
        /// <returns></returns>
        IEnumerator InstallLater()
        {
            yield return new WaitForEndOfFrame();
            Log.Write("Current DPI:" + Screen.dpi, LogColors.Blue);
            _maxOffsetRadius *= Screen.width * (Screen.dpi / 326f);
            yield return new WaitForEndOfFrame();
             
            InstantiateStick();
            installed = true;
        }

        /// <summary>
        /// Call it to reset stick positon
        /// </summary>
        public void ResetAll()
        {
            StickBaseTransform.anchoredPosition = StickBaseTransformPositon;
            //we have to set local position to zero
            StickImitatorTransform.localPosition = Vector3.zero; //StickImitatorTransformPositon;
            StickTransform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// Call it to instantiate stick, it's variables and start parameters.
        /// </summary>
        void InstantiateStick()
        {
            //cashing floating control state
            //floatingControl = DataManager.Instance.saveData.ControlPosition == 1 ? true : false;

            //setting callbacks to stick (central circle) 
            GameObject _stickGameObjet = null;// _gameUI.StickImg.gameObject;
            if (_stickGameObjet != null)
            {
                StickTransform = _stickGameObjet.GetComponent<RectTransform>();
                EventTrigger _stickEvents = _stickGameObjet.GetComponent<EventTrigger>();
                _stickEvents.triggers.Clear();

                StickTransform.GetComponent<Image>().raycastTarget = true;

                //ON POINTER DOWN
                EventTrigger.Entry _entryDown = new EventTrigger.Entry();
                _entryDown.eventID = EventTriggerType.PointerDown;
                _entryDown.callback.AddListener((data) => { OnStartedDrag(); });
                _stickEvents.triggers.Add(_entryDown);

                //ON POINTER UP
                EventTrigger.Entry _entryUp = new EventTrigger.Entry();
                _entryUp.eventID = EventTriggerType.PointerUp;
                _entryUp.callback.AddListener((data) => { OnEndedDrag(); });
                _stickEvents.triggers.Add(_entryUp);
            }
            else
            {
                Log.Write("Cant find MoveStick!", LogColors.Yellow);
            }

            //BUTTON 1
            GameObject _button1 = null;// _gameUI.Button1.gameObject;
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
                Log.Write("Cant find Button1", LogColors.Yellow);
            }


            //BUTTON 2
            GameObject _button2 = null;// _gameUI.Button2.gameObject;
            if (_button2 != null)
            {
                Button _button = _button2.GetComponent<Button>();
                if (Button2 != null)
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
            GameObject _button3 = null;// _gameUI.Button3.gameObject;
            if (_button3 != null)
            {
                Button _button = _button3.GetComponent<Button>();
                if (Button3 != null)
                {
                    _button.onClick.RemoveAllListeners();
                    _button.onClick.AddListener(() => Button3.Invoke());
                }
            }
            else
            {
                Log.Write("Cant find Button3", LogColors.Yellow);
            }

            //STICK BASE
            GameObject _stickBase = null;// _gameUI.StickBaseTransform.gameObject;
            if (_stickBase != null)
            {
                StickBaseTransform = null;// _gameUI.StickBaseTransform;
                StickTransform.localPosition = Vector3.zero;
                StickBaseTransformPositon = Vector3.zero;// _gameUI.StickBasePosition;
                Image _img = _stickBase.GetComponent<Image>();
                if (_img != null)
                {
                    stickBase = _img;
                    if (!floatingControl)
                    {
                        stickBase.raycastTarget = false;
                    }
                    else
                    {
                        stickBase.raycastTarget = true;
                    }
                }

                //STICK BASE BUTTON 
                Button _but = null;// _gameUI.StickBaseBackgroundTransform.GetComponent<Button>();
                if (_but != null)
                {
                    stickBaseButton = _but;
                    // NON FLOATING - CLEARING CALLBACKS
                    if (!floatingControl)
                    {
                        stickBaseButton.interactable = false;
                        stickBaseButton.GetComponent<Image>().raycastTarget = false;

                        EventTrigger _stickEvents = stickBaseButton.GetComponent<EventTrigger>();
                        _stickEvents.triggers.Clear();
                    }
                    // FLOATING - ADD CALLBACKS
                    else
                    {
                        stickBaseButton.interactable = true;
                        stickBaseButton.GetComponent<Image>().raycastTarget = true;

                        EventTrigger _stickEvents = stickBaseButton.GetComponent<EventTrigger>();
                        _stickEvents.triggers.Clear();

                        //ON POINTER DOWN
                        EventTrigger.Entry _entryDown = new EventTrigger.Entry();
                        _entryDown.eventID = EventTriggerType.PointerDown;
                        _entryDown.callback.AddListener((data) => StickClicked());
                        _stickEvents.triggers.Add(_entryDown);

                        //ON POINTER UP
                        EventTrigger.Entry _entryUp = new EventTrigger.Entry();
                        _entryUp.eventID = EventTriggerType.PointerUp;
                        _entryUp.callback.AddListener((data) => OnEndedDrag());
                        _stickEvents.triggers.Add(_entryUp);
                    }
                }
            }

            //imitator
            GameObject _imitatorGO = null;// _gameUI.StickImitatorTransform.gameObject;
            if (_imitatorGO != null)
            {
                _imitatorGO.transform.localPosition = Vector3.zero;
                StickImitatorTransform = _imitatorGO.GetComponent<RectTransform>();
            }
        }
        #endregion

        #region METHODS_PERFORM
        /// <summary>
        /// Called when stick's dragging started
        /// </summary>
        public void OnStartedDrag()
        {
            //setting move flag
            currentlyMoving = true;
            //if pc
#if UNITY_EDITOR
            moveStartPosition = Input.mousePosition;
            //if mobile
#else
            foreach (Touch _t in Input.touches)
            {
                PointerEventData _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                _eventDataCurrentPosition.position = new Vector2(_t.position.x, _t.position.y);
                List<RaycastResult> _res = new List<RaycastResult>();
                EventSystem.current.RaycastAll(_eventDataCurrentPosition, _res);
                for (int i = _res.Count - 1; i >= 0; i--)
                {
                    if (_res[i].gameObject == StickTransform.gameObject || _res[i].gameObject == stickBase.gameObject)
                    {
                        moveStartPosition = _t.position;
                        touchID = _t.fingerId;
                        break;
                    }
                }
            }
#endif
            if (floatingControl)
            {
                StickTransform.position = moveStartPosition;

                if (StickBaseTransform != null)
                {
                    StickBaseTransform.position = moveStartPosition;// = moveStartPosition;
                }

                if (StickImitatorTransform != null)
                    StickImitatorTransform.position = moveStartPosition; //moveStartPosition;
            } 
        }

        /// <summary>
        /// Called when stick's dragging ended
        /// </summary>
        public void OnEndedDrag()
        {
            currentlyMoving = false;
            ResetStickPosition();
            currentDirection = Vector2.zero; 
        }

        /// <summary>
        /// Call it to reset stick position(for example, in the end of move)
        /// </summary>
        void ResetStickPosition()
        {
            StickTransform.anchoredPosition = stickBasePositon;
            if (floatingControl)
            {
                StickBaseTransform.anchoredPosition = StickBaseTransformPositon;
                //StickImitatorTransform.position = StickImitatorTransform.position;
            }
        }

        /// <summary>
        /// Call it to get currently selected touch position
        /// </summary>
        /// <returns></returns>
        Vector3 GetTouchPositionWithID()
        {
            foreach (Touch _t in Input.touches)
            {
                if (_t.fingerId == touchID)
                {
                    return _t.position;
                }
            }
            return moveStartPosition;
        }

        /// <summary>
        /// Call it to check swipe start and end
        /// </summary>
        void CheckForSwipe()
        {

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
                    if (Mathf.Abs(_diffX) > 0.3f * Screen.width && Mathf.Abs(_diffX) > _diffY)
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
                        Log.Write("STARTED MOVING ON:" + countingSwipeStart, LogColors.Green, 25);
                    }
                }
                else if (_t.phase == TouchPhase.Ended && _t.fingerId == countingSwipeIndex)
                {
                    if (countingSwipe)
                    {
                        countingSwipe = false;
                        float _diffX = _t.position.x - countingSwipeStart.x;
                        float _diffY = _t.position.y - countingSwipeStart.y;
                        Log.Write("ENDED MOVING ON"+_t.position, LogColors.Green, 25);
                        if (Mathf.Abs(_diffX) > 0.1f * Screen.width && Mathf.Abs(_diffX) > _diffY)
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
        /// Called when clicked on super-base of stick
        /// </summary>
        void StickClicked()
        {
            if (currentlyMoving)
                return;

            OnStartedDrag();
        }

        /// <summary>
        /// Called each frame by InputManager
        /// </summary>
        public override void Tick()
        {
            if (!installed)
                return;

            CheckForSwipe();

            if (currentlyMoving)
            {
                float _strengthMultiplier = 1f;
                //getting current position
#if UNITY_EDITOR
                moveCurrentPosition = Input.mousePosition;

#else
                moveCurrentPosition = GetTouchPositionWithID();
#endif
                if (!floatingControl)
                {
                    //getting current offset (swipe vector3)
                    Vector2 _newPos = (moveCurrentPosition - moveStartPosition);
                    //saving current direction
                    currentDirection = _newPos.normalized;
                    //checking if we already passed too much distance
                    if (_newPos.sqrMagnitude > _maxOffsetRadius * _maxOffsetRadius)
                    {
                        _strengthMultiplier = 1f;
                        _newPos = _newPos.normalized * _maxOffsetRadius;
                    }
                    else
                    {
                        _strengthMultiplier = 1f * _newPos.magnitude / _maxOffsetRadius;
                    }
                    currentDirection *= _strengthMultiplier;

                    //adding stick base position
                    _newPos += stickBasePositon;
                    //moving stick to calculated position
                    StickTransform.anchoredPosition = _newPos;
                }
                else
                {
                    Vector2 _moveVector = moveCurrentPosition - moveStartPosition;
                    currentDirection = _moveVector.normalized;
                    Vector2 _newPos = moveCurrentPosition;
                    if (_moveVector.sqrMagnitude > _maxOffsetRadius * _maxOffsetRadius)
                    {
                        _strengthMultiplier = 1f;
                        _newPos = moveStartPosition + _moveVector.normalized * _maxOffsetRadius;
                    }
                    else
                    {
                        _strengthMultiplier = 1f * _moveVector.magnitude / _maxOffsetRadius;
                    }
                    currentDirection *= _strengthMultiplier;
                    StickTransform.position = _newPos;
                }
            }

            //in any case we have to send these deletaes - even with (0,0) data
            //calling move X delegate
            MovingAxisX.Invoke(currentDirection.x);

            //calling move Y delegate
            MovingAxisY.Invoke(currentDirection.y);
        }
        #endregion 
    }
}