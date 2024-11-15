using UnityEngine;

[CreateAssetMenu(fileName = "ParkourAction", menuName = "Parkour System/ParkourAction")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] private string animName;
	[SerializeField] private string obsatcleTag;

	[SerializeField] private float minHeight;
	[SerializeField] private float maxHeight;

    [SerializeField] private bool rotateToObstacle;
    [SerializeField] private float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] private bool enableTargetMatching=true;
    [SerializeField] protected AvatarTarget matchBodyPart;
	[SerializeField] private float matchStartTime;
	[SerializeField] private float matchTargetTime;
    [SerializeField] private Vector3 matchPosWeight=new Vector3(0,1,1);

    public bool Mirror { get; set; }

	public Quaternion TargetRotation { get;set; }
    public Vector3 MatchPos { get; set; }
	public Vector3 MatchPosWeight { get; private set; }
	public string AnimName { get; private set; }
	public bool RotateToObstacle { get; private set; }
	public bool EnableTargetMatching { get; private set; }
	public AvatarTarget MatchBodyPart { get; protected set; }
	public float MatchStartTime { get; private set; }
	public float MatchTargetTime { get; private set; }
	public float PostActionDelay { get; private set; }

	private void OnEnable()
	{
		MatchPosWeight=matchPosWeight;
		AnimName =animName;
        RotateToObstacle = rotateToObstacle;
        EnableTargetMatching = enableTargetMatching;
        MatchBodyPart = matchBodyPart;
        MatchStartTime = matchStartTime;
        MatchTargetTime = matchTargetTime;
        PostActionDelay = postActionDelay;
	}
	public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if (!string.IsNullOrEmpty(obsatcleTag)&&hitData.forwardHit.transform.tag!=obsatcleTag)
        {
            return false;
        }

        float height = hitData.heightHit.point.y - player.position.y;

		if (height < minHeight || height > maxHeight) return false;

        if (rotateToObstacle)
        {
            TargetRotation = Quaternion.LookRotation (- hitData.forwardHit.normal);    
        
        }
        if (enableTargetMatching)
        {
            MatchPos = hitData.heightHit.point;
        
        }

        return true;
    }
}
