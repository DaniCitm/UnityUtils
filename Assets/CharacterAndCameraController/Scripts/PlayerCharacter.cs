using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	public enum FacingOptions
	{
		None,
		MovementDirection,
		CameraDirection
	}

    public Transform cameraRoot;
    public float moveSpeed = 5f;
    public bool runEnabled = true;
    public float runSpeed = 10f;
    public float gravity = 9.8f; //13 works ok
    public FacingOptions facing = FacingOptions.MovementDirection;

    private CharacterController characterController;
    private Vector3 direction;
    private Vector3 cameraForwardProjected;
    private float speed;

    [Header("Jump")]
    public bool playerCanJump = true;
    public Transform topLimit; //if a little sphere in that position collides with something, jump stops
    public float jumpHeight = 3.8f;
    public AnimationCurve jumpRaiseCurve;
    public float jumpRaiseMinTime = 0.04f;
    public float jumpRaiseMaxTime = 0.35f;
    public float jumpForwardSpeedMultiplier = 1.65f;
    public float coyoteTime = 0.1f;
    //TO DO public Vector3 airborneMoveMultiplier = Vector3.one; //I plan to use it to multiply direction while on the aire to avoid "walk" on the air and be able to correct poor calculated jumps too much

    private float airTime = 0f;
    private float currentJumpTime = 0f;
    private bool jumping = false;
    private float jumpY0;
    //TO DO private Vector3 jumpDirection;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        speed = runEnabled && Input.GetButton("Run") ? runSpeed : moveSpeed;    //set movement speed

        CalculateDirection();
        if (facing != FacingOptions.None) Facing(facing);

        //We can be jumping (raising in the air or not) Movement works different with one or the other but it will be unified (TO DO).
        if (jumping) Jumping();
        else
        {
            //Moving on the ground or falling.
            characterController.Move(Time.deltaTime * (direction * speed - gravity * Vector3.up));
        }

        //Jump zone.
        if (!playerCanJump) return;

        //if player press jump (and can do it) it calls Jump, a method that reset jump state and set jumping to true.
        if (Input.GetButtonDown("Jump") && !jumping && airTime < coyoteTime)
        {
            Jump();
        }
        //if jump button released and minimum jump time is reached, jumping = false
        else if (!Input.GetButton("Jump") && currentJumpTime >= jumpRaiseMinTime)
        {
            jumping = false;
        }

        // if jump time is finished jumping = false
        if (jumping && currentJumpTime >= jumpRaiseMaxTime) jumping = false;
    }

    private void FixedUpdate()
    {
        //Jump zone
        if (!playerCanJump) return;

        //if collide a celing while jumping, stop jumping.
        if(jumping && Physics.CheckSphere(topLimit.position, 0.1f, ~LayerMask.GetMask("PlayerCharacter"))) //avoid PlayerCharacter layer.
        {
            jumping = false;
        }
        //if player lands to the ground, jumping finishes. falling is not jumping in this code, but imagine jump to a ledge and touch the ground of the ledge before falling. we need to connsider this.
        //using this instead of CharacterController.isGrounded because it's sooooo buggy.
        else if (Physics.CheckSphere(transform.position, 0.1f, ~LayerMask.GetMask("PlayerCharacter"))) //avoid PlayerCharacter layer.
        {
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }

    }

	private void Facing(FacingOptions facing)
	{
        if(facing == FacingOptions.CameraDirection)
        {
            transform.forward = cameraForwardProjected;
            return;
        }

        //facing == FacingOptions.MovementDirection
        if (direction == Vector3.zero) return;

        transform.forward = direction;
	}

	private void CalculateDirection()
	{
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction = Vector3.zero;

        direction += Vector3.ProjectOnPlane(cameraRoot.right, transform.up) * horizontal;
        cameraForwardProjected = Vector3.ProjectOnPlane(cameraRoot.forward, transform.up);
        direction += cameraForwardProjected * vertical;
        direction = direction.normalized;
    }

    //Reset jump state and set character to jumping = true;
    private void Jump()
    {
        jumping = true;
        currentJumpTime = 0f;
        jumpY0 = transform.position.y;
        //TO DO jumpDirection = transform.forward;
    }

    //While jumping (raising to the air) height is calculated by a curve and time. It's also multiplied by the height of our jump.
    //This method moves character, but height could be stored and use always the same Move function. TO DO.
    private void Jumping()
    {
        currentJumpTime += Time.deltaTime;
        var curveTime = currentJumpTime.Remap01(0, jumpRaiseMaxTime);
        float currentJumpHeight = jumpY0 + jumpHeight * jumpRaiseCurve.Evaluate(curveTime);

        //TO DO jumpDirection = (jumpDirection + Vector3.Scale(airborneMoveMultiplier,direction)).normalized;
        characterController.Move(direction * Time.deltaTime * speed * jumpForwardSpeedMultiplier + Vector3.up * (currentJumpHeight - transform.position.y));
    }
}
