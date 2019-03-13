using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for clearing rotation of parent
/// </summary>
public class FakeTransform : MonoBehaviour
{
    /// <summary>
    /// Flag if you need to save global rotation 
    /// </summary>
    public bool SaveGlobalRotation = false;
    /// <summary>
    /// Global rotation
    /// </summary>
    public Vector3 GlobalRotationAngles = Vector3.zero;
    /// <summary>
    /// Flag if we have to annulate rotation
    /// </summary>
    public bool ClearRotation;
    /// <summary>
    /// flag if we have to annulate X and Z rotation
    /// </summary>
    public bool ClearRotationNotY;
    /// <summary>
    /// Set it to true if you want to clear local rotation when object become enabled
    /// </summary>
    public bool ClearLocalRotationOnStart = false;
    /// <summary>
    /// Flag if we need to clear local position in the start of game
    /// </summary>
    public bool ClearLocalPositionOnStart = false;
    /// <summary>
    /// Cahsed transform of camera
    /// </summary>
    public Transform camera;
    /// <summary>
    /// Maximum scale of bullet catcher(depends on current angle)
    /// </summary>
    public float MaxScale = 5f;
    /// <summary>
    /// Flag if we have to scale this object, by currentAngle*multiplier
    /// </summary>
    public bool UpdateScale = false;
    /// <summary>
    /// Cashed parent transform
    /// </summary>
    private Transform parent;
    /// <summary>
    /// Flag if we have to inverse X local euler
    /// </summary>
    public bool InverceX = false;
    /// <summary>
    /// Count of frames that have to be passed to update scale
    /// </summary>
    private int UpdateEachFrame = 5;
    /// <summary>
    /// Local counter of passed frames
    /// </summary>
    private int currentUpdateCount = 0;
    /// <summary>
    /// Last value of set scale
    /// </summary>
    private float lastScaleValue = -999f;
    /// <summary>
    /// Vector3 of start scale (in OnEnable time)
    /// </summary>
    private Vector3 startScale = Vector3.zero;
    /// <summary>
    /// Flag if object have to copy Y angle as object below
    /// </summary>
    public bool CopyYAngle = false;
    /// <summary>
    /// Object from which Y angle has to be copied
    /// </summary>
    public Transform CopyYAngleObject;
    /// <summary>
    /// Flag if object has to apply position of water (pos by Y axiz)
    /// </summary>
    public bool ApplyWaterPosition = false;
    /// <summary>
    /// Position that object has to reach
    /// </summary>
    public float WaterPosition = -11.5f;

    /// <summary>
    /// Called once when spawning
    /// </summary>
    private void OnEnable()
    {
        if(camera == null)
            camera = Camera.main.transform;

        parent = transform.parent;
        startScale = transform.localScale;

        if (ClearLocalRotationOnStart)
            transform.localEulerAngles = Vector3.zero;

        if (ClearLocalPositionOnStart)
            transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Called when object turning off
    /// </summary>
    private void OnDisable()
    {
        transform.localScale = startScale;
    }

    /// <summary>
    /// Call it to scale bullet catcher(depends on current angle)
    /// </summary>
    void UpdateBulletCatcherScale()
    {
        currentUpdateCount++;
        if (currentUpdateCount != UpdateEachFrame)
            return;

        currentUpdateCount = 0;

        float _currentAngle = transform.parent.eulerAngles.y;
        if (_currentAngle < 0)
        {
            _currentAngle = 360f - _currentAngle;
        }
        float _toScale = 1f;
        if (_currentAngle < 270f && _currentAngle > 90f)
        {
            if (_currentAngle < 180f)
            {
                _toScale = (180f - _currentAngle); 
            }
            else
            {
                _toScale = _currentAngle - 180f; 
            }
        }
        else
        {
            if (_currentAngle < 90f)
            {
            }
            else
            {
                _toScale = (360f - _currentAngle);
            } 
        } 
        _toScale = 1f + _toScale / 90f * (MaxScale - 1f);
        _toScale = Mathf.Round(_toScale * 10f) / 10f;

        if(transform.localScale.x != _toScale && _toScale != lastScaleValue)
        {
            transform.localScale = new Vector3(startScale.x * _toScale, transform.localScale.y, transform.localScale.z);
            lastScaleValue = _toScale;
        }
    }

    /// <summary>
    /// Called each frame
    /// </summary>
    void Update ()
    {
        if (ApplyWaterPosition)
        {
            transform.position = new Vector3(transform.position.x, WaterPosition, transform.position.z);
        }

        if(CopyYAngle && CopyYAngleObject != null)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, CopyYAngleObject.localEulerAngles.y, transform.localEulerAngles.z);

        if (UpdateScale)
            UpdateBulletCatcherScale();

        //rotations performing
        if (ClearRotation)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (SaveGlobalRotation)
        {
            transform.eulerAngles = GlobalRotationAngles;
        }
        else if (ClearRotationNotY)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else if (ClearLocalRotationOnStart)
        {
            //do nothing
        }
        else
        {
            transform.LookAt(camera);
        }

        //rotation final performing
        if (InverceX)
            transform.eulerAngles = new Vector3(-transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); 
	}
}
