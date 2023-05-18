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
    public float gravity = -9.8f; //-13 works ok
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
    public float coyoteTime = 0.1f;
    public float airborneControlMultiplierForward = 1.8f; //jump direction air control multiplier, usually more than side (below).
    public float airborneControlMultiplierSide = 1f; //jump side direction air control multiplier

    private float airTime = 0f;
    private float currentJumpTime = 0f;
    private bool jumping = false;
    private float jumpY0;
    private Vector3 jumpDirection;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDirection();
        if (facing != FacingOptions.None) Facing(facing);

        //Movement zone
        {
            Vector3 movement = Vector3.zero;
            speed = runEnabled && Input.GetButton("Run") ? runSpeed : moveSpeed;    //set movement speed

            //Air control. maneuverability of the player on the air.
            if (jumping || airTime > coyoteTime)
            {
                //making absolute the result of dot product we make that vectors aligned with jumpDirection = 1 and the perpendicular = 0. Then remap that 0-1 range to something else (1-1.8 by default) and use it on the vector lerp to get the new direction
                float multiplier = Mathf.Abs(Vector3.Dot(jumpDirection, direction)).Remap(0f,1f, airborneControlMultiplierSide, airborneControlMultiplierForward);
                direction = Vector3.Lerp(jumpDirection, multiplier * direction, 0.75f); //  3/4 new direction, 1/4 original one.
            }

            movement = Time.deltaTime * direction * speed;
            movement.y = jumping ? GetJumpCurrentHeight() : Time.deltaTime * gravity;

            characterController.Move(movement);
        }

        //Jump zone.
        {
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
        jumpDirection = transform.forward;
    }

    //Jump raise is calculated using a curve. The curve must go from time 0 to 1 and value should start a little bit over 0 (like 0.1, that's to make the character quickly raise and make jump feel super responsive) and finish on 1 too.
    // Returns how much the character needs to move up this frame
    private float GetJumpCurrentHeight()
    {
        currentJumpTime += Time.deltaTime;
        var curveTime = currentJumpTime.Remap01(0, jumpRaiseMaxTime);
        float currentJumpHeight = jumpY0 + jumpHeight * jumpRaiseCurve.Evaluate(curveTime);

        return currentJumpHeight - transform.position.y;
    }
}
