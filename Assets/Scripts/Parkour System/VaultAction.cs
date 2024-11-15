using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "VaultAction", menuName = "Parkour System/VaultAction")]
public class VaultAction : ParkourAction
{
	public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
	{
		if (!base.CheckIfPossible(hitData,player))
		{
			return false;
		}

		var hitpoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        if (hitpoint.z<0 && hitpoint.x<0 || hitpoint.z>0 && hitpoint.x>0)
        {
			Mirror = true;
			MatchBodyPart = AvatarTarget.RightHand;
        }
		else
		{
			Mirror = false;
			MatchBodyPart = AvatarTarget.LeftHand;
			Debug.Log($"match:{matchBodyPart},Match:{MatchBodyPart}");
		}


		return true;
    }

}
