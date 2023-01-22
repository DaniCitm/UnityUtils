using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController2D : MonoBehaviour
{
	private Rigidbody2D myRigidbody2D;

	#region Movement
	public float movementSpeed = 2;
	private float movement;
	private bool facingRight = true;
	#endregion

	#region Jump
	public float jumpForce = 2;
	public LayerMask groundLayers;
	public float maxJumpTime = 0.2f;
	private bool jumping;
	private bool grounded;
	private float startjumpTime;
	#endregion

	private void Awake()
	{
		myRigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		movement = Input.GetAxis("Horizontal");
		Facing();
		Movement();
		CheckGrounded();
		Jump();
	}

	private void Facing()
	{
		if(movement > 0.1f && !facingRight)
		{
			transform.eulerAngles = Vector3.zero;
			facingRight = true;
		}
		else if(movement < -0.1f && facingRight)
		{
			transform.eulerAngles = new Vector3(0, 180, 0);
			facingRight = false;
		}
	}

	private void Movement()
	{
		myRigidbody2D.AddForce(new Vector2(movement * movementSpeed, 0), ForceMode2D.Impulse);
	}

	private void Jump()
	{
		if (grounded) jumping = false;

		if(jumping && Input.GetButton("Jump") && Time.time - startjumpTime < maxJumpTime)
		{
			myRigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
		}

		//Start Jump
		if (!jumping && Input.GetButtonDown("Jump") && grounded)
		{
			myRigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			jumping = true;
			startjumpTime = Time.time;
		}
	}

	private void CheckGrounded()
	{
		grounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayers);
	}
}
