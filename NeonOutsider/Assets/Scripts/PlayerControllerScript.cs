using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

	public float maxSpeed = 15f;
	bool facingRight = true;

	public float jumpTime=0.1f;
	private float jumpTimeCounter;
	public float jumpForce=10f;

	Animator anim;

	bool grounded = false;
	public Transform groundCheck;
	float groundRadius=0.2f;
	public LayerMask whatIsGround;


	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rb=GetComponent<Rigidbody2D>();
		jumpTimeCounter = jumpTime;
	}

	void Update(){

		if (Input.GetButtonDown("Jump")){
			if (grounded){
				anim.SetBool("Ground", false);
				rb.velocity=new Vector2(rb.velocity.x, jumpForce/2);
			}
		}


		if (Input.GetButton("Jump")){
			if(jumpTimeCounter>0){
				
				rb.velocity = new Vector2(rb.velocity.x, jumpForce);
				jumpTimeCounter -= Time.deltaTime;
			}
		}

		if (Input.GetButtonUp("Jump")){
			jumpTimeCounter=0;
		}

		if (grounded){
			jumpTimeCounter=jumpTime;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		grounded=Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		anim.SetBool("Ground", grounded);

		anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);

		float move = Input.GetAxis("Horizontal");

		anim.SetFloat("Speed", Mathf.Abs(move));

		rb.velocity=new Vector2(move * maxSpeed, rb.velocity.y);

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
