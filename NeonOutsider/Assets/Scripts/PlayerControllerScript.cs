using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

	Animator anim;

	//variables for moving
	public float maxSpeed = 15f;
	//will help flipping sprite
	bool facingRight = true;

	//variables for jumping
	public Transform groundCheck;
	bool grounded = false;
	float groundRadius=0.2f;
	public LayerMask whatIsGround;

	public float jumpTime=0.1f;
	private float jumpTimeCounter;
	public float jumpForce=10f;

	//variables for crouching
	public Transform ceilingCheck;
	bool ceiled=false;
	private float crouch;
	public bool crouching;

	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rb=GetComponent<Rigidbody2D>();
		jumpTimeCounter = jumpTime;
	}

	void Update(){
		//checking crouch & jump
		crouch= Input.GetAxisRaw("Crouch");
		JumpFunction();
		CrouchFunction();
	}

	void JumpFunction(){
		//starting jump
		if (Input.GetButtonDown("Jump")){
			if (grounded){
				anim.SetBool("Ground", false);
				rb.velocity=new Vector2(rb.velocity.x, jumpForce/2);
			}
		}

		//long jump while time counter >0
		if (Input.GetButton("Jump")){
			if(jumpTimeCounter>0){
				rb.velocity = new Vector2(rb.velocity.x, jumpForce);
				jumpTimeCounter -= Time.deltaTime;
			}
		}

		//ending jump
		if (Input.GetButtonUp("Jump")){
			jumpTimeCounter=0;
		}

		//reseting timer
		if (grounded){
			jumpTimeCounter=jumpTime;
		}
	}

	void CrouchFunction()
	{
		//if player on ground && (crouching or can't stand up) then crouch=true
		if ((crouch!=0 || ceiled) && grounded)
		{
			crouching=true;
		}
		else
		{
			crouching=false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//checking if grounded and ceiled
		grounded=Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		ceiled=Physics2D.OverlapCircle(ceilingCheck.position, groundRadius, whatIsGround);

		//changing animator settings
		anim.SetBool("Ground", grounded);
		anim.SetBool("Crouch", crouching);
		anim.SetFloat("vSpeed", rb.velocity.y);

		//changing animator settings of speed
		float move = Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(move));

		rb.velocity=new Vector2(move * maxSpeed, rb.velocity.y);

		//flipping sprite
		if (move>0 && facingRight)
			Flip();
		else if (move<0 && facingRight)
			Flip();
	}


	void Flip(){
		facingRight=!facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *=-1;
		transform.localScale=theScale;
	}
}
