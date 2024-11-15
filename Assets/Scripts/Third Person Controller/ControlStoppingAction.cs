using UnityEngine;
using UnityEngine.Animations;

public class ControlStoppingAction : StateMachineBehaviour
{
	PlayerController player;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.GetComponent<PlayerController>();
		}
		player.HasControl=false;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player.HasControl = true;
	}
}
