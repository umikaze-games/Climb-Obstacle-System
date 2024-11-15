using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
	[SerializeField] private List<ParkourAction> parkourActions;
	[SerializeField] private ParkourAction jumpDownAction;
	[SerializeField] private float autoDropHeightLimit=1f;

	private EnvironmentScanner environmentScanner;
	private Animator animator;
	private	PlayerController playerController;
	private void Awake()
	{
		environmentScanner =GetComponent<EnvironmentScanner>();
		animator = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();	
	}
	private void Update()
	{
		var hitData = environmentScanner.ObstacleCheck();
		if (Input.GetButtonDown("Jump")&& !playerController.InAction && !playerController.IsHanging)
		{
			
			if (hitData.forwardHitFound)
			{
				foreach (var action in parkourActions)
				{
					if (action.CheckIfPossible(hitData, transform))
					{
						
						StartCoroutine(DoParkourAction(action));
						break;
					}
		
				}

			}

		}
		if (playerController.IsOnLedge&&!playerController.InAction && !hitData.forwardHitFound)
		{
			bool shouldJump = true;
			if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButtonDown("Jump"))
			{
				Debug.Log("space");
				shouldJump = false;
			}
			if (shouldJump&&playerController.LedgeData.angle <= 50.0f)
			{
				Debug.Log("nospace");
				playerController.IsOnLedge = false;
				StartCoroutine(DoParkourAction(jumpDownAction));
			}
	
		}

	}
	private IEnumerator DoParkourAction(ParkourAction action)
	{
		playerController.SetControl(false);
		MatchTargetParams matchParam = null;

		if (action.EnableTargetMatching)
		{
			matchParam = new MatchTargetParams()
			{
				pos = action.MatchPos,
				bodyPart = action.MatchBodyPart,
				posWeight = action.MatchPosWeight,
				startTime = action.MatchStartTime,
				targetTime = action.MatchTargetTime,

			};
		}

		yield return playerController.DoAction(action.AnimName, matchParam, action.TargetRotation, action.RotateToObstacle, action.PostActionDelay, action.Mirror);
		playerController.SetControl(true);
	}

	private void MatchTarget(ParkourAction action)
	{
		animator.MatchTarget(
			action.MatchPos,
			transform.rotation,
			action.MatchBodyPart,
			new MatchTargetWeightMask(action.MatchPosWeight, 0),
			action.MatchStartTime,
			action.MatchTargetTime);
	}

}

