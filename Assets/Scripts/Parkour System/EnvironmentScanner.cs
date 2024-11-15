using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
	[SerializeField] private Vector3 forwardRayOffset=new Vector3(0,0.25f,0);

	[SerializeField] private LayerMask obstacleLayer;
	[SerializeField] private LayerMask climbLedgeLayer;

	[SerializeField] private float forwardRayLength = 0.8f;
	[SerializeField] private float climbLedgeRayLength = 1.5f;
	[SerializeField] private float heightRayLength = 5.0f;
	[SerializeField] private float ledgeRayLength = 10.0f;
	[SerializeField] private float ledgeHeightThreshold = 0.75f;

	public ObstacleHitData ObstacleCheck()
	{
		var hitData = new ObstacleHitData();
		var forwardOrign = transform.position + forwardRayOffset;
		hitData.forwardHitFound = Physics.Raycast(
			forwardOrign, 
			transform.forward,
			out hitData.forwardHit,
			forwardRayLength,
			obstacleLayer
			);

		Debug.DrawRay(forwardOrign,transform.forward*forwardRayLength,(hitData.forwardHitFound) ? Color.red:Color.white);

		if(hitData.forwardHitFound)
		{
			var heighOrign= hitData.forwardHit.point+Vector3.up* heightRayLength;
			hitData.heightHitFound=Physics.Raycast(
				heighOrign,
				Vector3.down,
				out hitData.heightHit,
				heightRayLength,
				obstacleLayer
				);
			Debug.DrawRay(heighOrign, Vector3.down*heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);

		}

		return hitData;
	}

	public bool ClimbLedgeCheck(Vector3 dir, out RaycastHit ledgeHit)
	{
		ledgeHit =new RaycastHit();
		if (dir == Vector3.zero) return false;
		var origin = transform.position + Vector3.up*1.5f;
		var offset = new Vector3(0,0.18f,0);
		for (int i = 0; i < 10; i++)
		{
			if (Physics.Raycast(origin + offset * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
			{ 
				ledgeHit=hit;
				return true;
			
			}
		}
		return false;

	}

	public bool DropLedgeCheck(out RaycastHit ledgeHit)
	{ 
		ledgeHit=new RaycastHit();
		var origin = transform.position + Vector3.down * 0.1f + transform.forward * 2f;
		if (Physics.Raycast(origin, -transform.forward, out RaycastHit hit, 3, climbLedgeLayer))
		{
			ledgeHit = hit;
			return true; 
		}
		
		return false ;
	}

	public bool ObstacleLedgeCheck(Vector3 moveDir,out LedgeData ledgeData)
	{
		ledgeData=new LedgeData();

		if (moveDir==Vector3.zero)
		{
			return false;
		}
		float originOffset = 0.5f;
		var origin= transform.position+moveDir*originOffset+Vector3.up;

		if (PhysicsUtil.ThreeRaycasts(origin, Vector3.down,0.25f,transform,
			out List<RaycastHit> hits, ledgeRayLength, obstacleLayer,true))	
		{

			var validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

			if (validHits.Count > 0)
			{
				var surfaceRayOrigin = validHits[0].point;
				surfaceRayOrigin.y = transform.position.y - 0.1f;
				if (Physics.Raycast(surfaceRayOrigin, transform.position-surfaceRayOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
				{
					Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
					float height = transform.position.y - validHits[0].point.y;
					if (height > ledgeHeightThreshold)
					{
						ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
						ledgeData.height = height;
						ledgeData.surfaceHit = surfaceHit;
						return true;
					}
				}
			}
		}
		return false;
	}

}

public struct ObstacleHitData
{
	public bool forwardHitFound;
	public bool heightHitFound;
	public RaycastHit forwardHit;
	public RaycastHit heightHit;

}
public struct LedgeData
{
	public float height;
	public float angle;
	public RaycastHit surfaceHit;

}