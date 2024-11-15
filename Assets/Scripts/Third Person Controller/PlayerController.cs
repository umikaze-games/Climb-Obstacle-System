using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] private float rotationSpeed = 500f;

	[Header("Ground Check Settings")]
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private Vector3 groundCheckOffset;
	[SerializeField] LayerMask groundLayer;

	public float RotationSpeed=>rotationSpeed;
	private CameraController cameraController;
	private Quaternion targetRotation;
	private Animator animator;
	private CharacterController characterController;
	private EnvironmentScanner environmentScanner;

	private bool isGrounded;
	private bool hasControl=true;
	public bool InAction { get; private set; }

	private Vector3 desireMoveDir;
	private Vector3 moveDir;
	private Vector3 velocity;
	public LedgeData LedgeData { get; set; }

	public bool IsOnLedge { get; set; }
	public bool IsHanging { get; set; }

	private float ySpeed;

	private void Awake()
	{
		cameraController = Camera.main.GetComponent<CameraController>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
		environmentScanner = GetComponent<EnvironmentScanner>();
	}

	private void Update()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

		var moveInput = (new Vector3(h, 0, v)).normalized;

		desireMoveDir = cameraController.PlanarRotation * moveInput;
		moveDir = desireMoveDir;

		if (!hasControl) return;

		if (IsHanging) return;

		velocity = Vector3.zero;

		GroundCheck();
		animator.SetBool("isGrounded", isGrounded);
		if (isGrounded)
		{
			ySpeed = -0.5f;
			velocity = desireMoveDir * moveSpeed;
			IsOnLedge = environmentScanner.ObstacleLedgeCheck(desireMoveDir, out LedgeData ledgeData);
			if (IsOnLedge)
			{
				LedgeData= ledgeData;
				LedgeMovement();
			}
			animator.SetFloat("moveAmount", velocity.magnitude/moveSpeed , 0.2f, Time.deltaTime);
		}
		else
		{
			ySpeed += Physics.gravity.y * Time.deltaTime;
			velocity = transform.forward * moveSpeed / 2;
		}

		velocity.y = ySpeed;

		characterController.Move(velocity * Time.deltaTime);

		if (moveAmount > 0 && moveDir.magnitude>0.2f)
		{
		
			targetRotation = Quaternion.LookRotation(moveDir);
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
			rotationSpeed * Time.deltaTime);


	}
	private void GroundCheck()
	{
		isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);

	}
	private void LedgeMovement()
	{
		float SingedAngle=Vector3.SignedAngle(LedgeData.surfaceHit.normal, desireMoveDir,Vector3.up);
		float angle = Mathf.Abs(SingedAngle);
	

		if (Vector3.Angle(desireMoveDir,transform.forward)>=80)
		{
			velocity = Vector3.zero;
			return;
		}

		if (angle < 60)
		{
			velocity = Vector3.zero;
			moveDir = Vector3.zero;

		}
		else if(angle<90)
		{
			var left=Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
			var dir = left * Mathf.Sign(SingedAngle);
			velocity = velocity.magnitude * dir;
			moveDir = dir;
		
		}
	
	}
	public void SetControl(bool hascontrol)
	{
		this.hasControl = hascontrol;
		characterController.enabled = hascontrol;
		if (!hascontrol)
		{
			animator.SetFloat("moveAmount", 0f);
			targetRotation = transform.rotation;
		}
	}

	public void EnableCharacterController(bool enabled)
	{ 
		characterController.enabled = enabled;
	}
	public void ResetTargetRotation()
	{
		targetRotation = transform.rotation;
	}

	public IEnumerator DoAction(string animName,MatchTargetParams matchParams=null,Quaternion targetRotation=new Quaternion(),
		bool rotate=false,float postDelay=0f,bool mirror=false)
	{
		InAction = true;

		animator.SetBool("mirrorAction",mirror);
		animator.CrossFadeInFixedTime(animName, 0.2f);

		yield return null;


		var animState = animator.GetCurrentAnimatorStateInfo(0);

		float timer = 0f;
		while (timer < animState.length)
		{
			timer += Time.deltaTime;
			if (rotate)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}

			if (matchParams!=null)
			{
				if (!animator.IsInTransition(0))
				{
					MatchTarget(matchParams);
				}
				yield return null;
			}
			if (animator.IsInTransition(0) && timer > 0.5f)
			{
				break;
			}

		}
		yield return new WaitForSeconds(postDelay);

		InAction = false;
	}

	private void MatchTarget(MatchTargetParams mp)
	{
		animator.MatchTarget(
			mp.pos,
			transform.rotation,
			mp.bodyPart,
			new MatchTargetWeightMask(mp.posWeight, 0),
			mp.startTime,
			mp.targetTime);
	}
	public bool HasControl { 
		get=> hasControl;
		set=> hasControl=value; }

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
	}

}

public class MatchTargetParams
{
	public Vector3 pos;
	public AvatarTarget bodyPart;
	public Vector3 posWeight;
	public float startTime;
	public float targetTime;

}