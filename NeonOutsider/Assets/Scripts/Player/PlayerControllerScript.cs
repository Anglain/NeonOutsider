﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerControllerScript : MonoBehaviour {

#region Skill vars

	public KeyCode shieldKey;
	public GameObject shieldPrefab;
	public GameObject wallPrefab;
	public GameObject lazerPrefab;
	public RewindTimer rewindTimer;
	private bool rewindFillStarted;

#endregion
	Animator anim;

#region moving and jumping vars
	
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
#endregion

	// Use this for initialization
	void Start () {
		Assert.IsNotNull(shieldPrefab);
		Assert.IsNotNull(wallPrefab);
		// Assert.IsNotNull(lazerPrefab);
		Assert.IsNotNull(rewindTimer);

		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		jumpTimeCounter = jumpTime;
	}

	void Update(){
		//checking crouch & jump
		crouch = Input.GetAxisRaw("Crouch");
		JumpFunction();
		CrouchFunction();
		ResolveForSkillInput();
	}

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

		ResolveFlipping(move, Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	/// <summary>
	/// flips player when move direction is different from facing direction,
	/// or cursor goes into the part of screen that is behind player's back
	/// </summary>
	/// <param name="move"></param>
	private void ResolveFlipping(float move, Vector3 cursorPos){
		if (move > 0 && !facingRight)
			Flip();
		else if (move < 0 && facingRight)
			Flip();

		Vector3 dir = cursorPos - transform.position;
		bool cursortOnRight = dir.x > 0;
		if(cursortOnRight && !facingRight || !cursortOnRight && facingRight)
		{
			Flip();
		}
	}

	private void ResolveForSkillInput(){
        if(Input.GetMouseButtonDown(1))
		{
			GameObject wallInstance = Instantiate(wallPrefab, this.transform.position, Quaternion.identity);
			Wall wallScript = wallInstance.GetComponent<Wall>();
			wallScript.Place(this.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			Instantiate(shieldPrefab, this.transform.position, Quaternion.identity);
		}

		ResolveRewindInput();
    }
	
    private void ResolveRewindInput(){
		if(RewindController.Instance.HasSkills() == false)	// timer should be hidden, ignore all input 
		{
			return;
		}

		if(rewindFillStarted)
		{
			if(Input.GetKeyUp(shieldKey))	// not holding key any more
			{
				rewindTimer.FinishFill();
				rewindFillStarted = false;
			}
		}
		else if(Input.GetKeyDown(shieldKey))	// started holding key
		{
			rewindTimer.StartFill();
			rewindFillStarted = true;
		}		
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

	void CrouchFunction(){
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
	
	void Flip(){
		facingRight =! facingRight;
		Vector3 scale = transform.localScale;
		scale.x *=-1;
		transform.localScale=scale;
	}
}
