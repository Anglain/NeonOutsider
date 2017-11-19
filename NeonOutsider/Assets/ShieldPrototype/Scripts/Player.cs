using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour 
{
	public GameObject shieldPrefab;
	public GameObject wallPrefab;
	public RewindTimer rewindTimer;

	private bool rewindFillStarted;

	#region  movement vars
	public float speed = 100f;
	public float jumpFactor = 1f;
	int framesToJumpAgain = 10;
	int framesPast = 0;
	bool jumping = false;
	Rigidbody2D rb;
	#endregion

	// Use this for initialization
	
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(1))
		{
			GameObject wallInstance = Instantiate(wallPrefab, this.transform.position, Quaternion.identity);
			Wall wallScript = wallInstance.GetComponent<Wall>();
			wallScript.Place(this.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

			
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			GameObject shieldInstance = Instantiate(shieldPrefab, this.transform.position, Quaternion.identity);
			// shieldInstance.SetActive(true);	// must set it active from there or the Start will no be called
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			if(rewindFillStarted == false)
			{
				rewindTimer.StartFill();
				rewindFillStarted = true;
			}
			else
			{
				if(Input.GetKeyDown(KeyCode.R) == false)	// if user stopped pressing key, rewind
				{
					rewindTimer.FinishFill();
					rewindFillStarted = false;
				}
			}
		}

		if(rewindFillStarted == false )	// start filling timer
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				rewindTimer.StartFill();
				rewindFillStarted = true;
			}
		}
		else if (Input.GetKey(KeyCode.R) == false)	// if user stopped pressing key, rewind
		{
			rewindTimer.FinishFill();
			rewindFillStarted = false;
		}
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
