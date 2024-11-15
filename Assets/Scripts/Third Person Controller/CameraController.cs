using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform followTarget;
	[SerializeField] private float rotationSpeed=2.0f;
	[SerializeField] private float distance=5.0f;
	[SerializeField] private float minVerticalAngle = -20.0f;
	[SerializeField] private float maxVerticalAngle = 45.0f;
	[SerializeField] private Vector2 framingOffset;

	private float rotationX;
	private float rotationY;
	private float invertXVal;
	private float invertYVal;

	[SerializeField] private bool invertX;
	[SerializeField] private bool invertY;
	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	private void Update()
	{
		invertXVal = (invertX) ? -1 : 1;
		invertYVal = (invertY) ? -1 : 1;
		rotationX += Input.GetAxis("CameraY")* invertYVal * rotationSpeed;
		rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

		rotationY += Input.GetAxis("CameraX") * invertXVal * rotationSpeed;

		var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
		var focusPosition = followTarget.position+new Vector3(framingOffset.x,framingOffset.y);

		transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
		transform.rotation = targetRotation;

	}
	public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
