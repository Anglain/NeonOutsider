using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
	public float speed = 100f;
	public float jumpFactor = 1f;
	int framesToJumpAgain = 10;
	int framesPast = 0;
	bool jumping = false;
	Rigidbody2D rb;
	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void FixedUpdate()
	{
		// Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime
		// rb.MovePosition(rb.position);
		float moveHorizontal = Input.GetAxis("Horizontal");
		Vector2 moveVec = new Vector2(moveHorizontal * speed * Time.deltaTime, 0.0f);
		rb.AddForce(moveVec);



		float moveJump = Input.GetAxisRaw("Jump");
		if(moveJump != 0.0f && jumping == false)
		{
			jumping = true;
			// Vector3 jumpVector = new Vector3(0.0f, 1.0f , 0.0f);
			Vector3 jumpVector = Vector3.up;
			rb.AddForce(jumpVector * jumpFactor , ForceMode2D.Impulse);
		}
		if(jumping)
		{
			framesPast++;
			if(framesPast >= framesToJumpAgain)
			{
				jumping = false;
				framesPast = 0;
			}
		}

	}
}
