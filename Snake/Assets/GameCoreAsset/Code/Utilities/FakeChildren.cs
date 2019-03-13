using PoolManagerModule;
using SFXManagerModule; 
using UnityEngine;

/// <summary>
/// Use this class to imitate parenting of gameobject
/// </summary>
public class FakeChildren : MonoBehaviour
{
    #region VARIABLES
    /// <summary>
    /// Will find target by it's tag
    /// </summary>
    public bool AutoFindTarget = false;
    /// <summary>
    /// Target's transform, fake "Parent"
    /// </summary>
    public Transform Target;
    /// <summary>
    /// Object's transform, fake "child"
    /// </summary>
    public Transform Object;
    /// <summary>
    /// Flag for position synchronization
    /// </summary>
    public bool Pos = true;
    /// <summary>
    /// Flag for rotation synchronization
    /// </summary>
    public bool Rot = true;
    /// <summary>
    /// Difference of position
    /// </summary>
    private Vector3 PositionDifference = Vector3.zero;
    /// <summary>
    /// Difference of rotation
    /// </summary>
    private Vector3 RotationDifference = Vector3.zero;
    /// <summary>
    /// Flag if moving will be lerped
    /// </summary>
    public bool Lerp = true;
    /// <summary>
    /// Flag of random rotation after despawn
    /// </summary>
    public bool RandomRotation = false;
    /// <summary>
    /// Value of lerp coeficient
    /// </summary>
    public float LerpCoef = 0.4f;
    /// <summary>
    /// Name of target(if we have to find it by TagName)
    /// </summary>
    public string TargetName = "Player";
    /// <summary>
    /// Flag if we have to despawn object if there is no object to follow
    /// </summary>
    public bool HaveToDespawnAfter = true;
    /// <summary>
    /// Flag if we have to despawn in parent despawned too
    /// </summary>
    public bool HaveToDespawnLikeParent = false;
    /// <summary>
    /// Flag if object have to lookat some target
    /// </summary>
    public bool HaveToLookAt = false;
    /// <summary>
    /// Flag if we have to clear differnece betwen objects
    /// </summary>
    public bool ClearDifference = false;
    #endregion

    #region METHODS
    /// <summary>
    /// Called when despawned
    /// </summary>
    private void OnEnable()
    { 
        if (RandomRotation)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0f, 360f));
        }

        //for test scene with selected object to follow
        if (Target != null && Object != null)
        {
            PositionDifference = Target.position - Object.position;
            RotationDifference = Target.eulerAngles - Object.eulerAngles;
        }
        //for result scene with unselected object to follow
        else if (Object != null && Target == null && AutoFindTarget && !string.IsNullOrEmpty(TargetName) )
        { 
            Target = GameObject.FindGameObjectWithTag(TargetName).transform;
            if (!ClearDifference)
            {
                PositionDifference = Target.position - Object.position;
                RotationDifference = Target.eulerAngles - Object.eulerAngles;
            }
            else
            {
                PositionDifference = Vector3.zero;
                RotationDifference = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Call it ot set target that object has to follow
    /// </summary>
    /// <param name="_target"></param>
    public void SetFollowingTarget(Transform _target)
    { 
        Target = _target;

        //for test scene with selected object to follow
        if (Target != null && Object != null)
        {
            PositionDifference = Target.position - Object.position;
            RotationDifference = Target.eulerAngles - Object.eulerAngles;
        } 
    }

    /// <summary>
    /// Call it to set parameters and start processing script
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_object"></param>
    public void Init(Transform _target, Transform _object)
    {
        Target = _target;
        Object = _object;

        PositionDifference = Target.position - Object.position; 
        RotationDifference = Target.eulerAngles - Object.eulerAngles;
    }
	 
    /// <summary>
    /// Called each frame
    /// </summary>
	void Update ()
    {
        if(Object && Target)
        {
            if (HaveToDespawnLikeParent && !Target.gameObject.activeSelf)
                this.gameObject.GetComponent<SFXPlayer>().Disable();

            if (Pos)
            {
                if (Lerp && LerpCoef > 0)
                    Object.position = Vector3.Lerp(Object.position, Target.position - PositionDifference, LerpCoef * TimeScaleManager.Instance.GetDelta());
                else
                    Object.position = Target.position - PositionDifference; 
            }
            if(Rot)
                Object.eulerAngles = Target.eulerAngles - RotationDifference;
        }
        else
        {
            if (HaveToLookAt && Target != null)
            {
                transform.LookAt(Target.position);
            }
            if (HaveToDespawnAfter)
            {
                Log.Write("Despawning cause there is no object!");
                PoolManager.Instance.Despawn(transform);
            }
        }
	}
     
    #endregion
}
