using UnityEngine;

public class CharController : MonoBehaviour
{
	public enum FacingOptions
	{
		None,
		MovementDirection,
		CameraDirection
	}

    public Transform cameraRoot;
    public float speed = 2f;
    public FacingOptions facing = FacingOptions.MovementDirection;

    private CharacterController characterController;
    private Vector3 direction;
    private Vector3 cameraForwardProjected;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateDirection();
        if (facing == FacingOptions.MovementDirection) FaceMovementDirection();
        else if (facing == FacingOptions.CameraDirection) FaceCameraDirection();
        Move();
    }

	private void FaceCameraDirection()
	{
        transform.forward = cameraForwardProjected;
	}

	private void FaceMovementDirection()
	{
        if (direction == Vector3.zero) return;

        transform.forward = direction;
	}

	private void CalculateDirection()
	{
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction = Vector3.zero;

        direction += Vector3.ProjectOnPlane(cameraRoot.right, transform.up).normalized * horizontal;
        cameraForwardProjected = Vector3.ProjectOnPlane(cameraRoot.forward, transform.up).normalized;
        direction += cameraForwardProjected * vertical;
    }

    private void Move()
	{
        characterController.SimpleMove(direction * speed);
    }
}
