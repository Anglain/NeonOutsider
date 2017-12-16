using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for better jumping physics
public class BetterJump : MonoBehaviour {

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 0.5f;

	Rigidbody2D rb;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
	}

	void Update(){
		//jumping up is longer than falling down
		if (rb.velocity.y<0){
			rb.velocity +=Vector2.up *Physics2D.gravity.y * (fallMultiplier-1) *Time.deltaTime;
		} else if (rb.velocity.y>0 && !Input.GetButton("Jump")){
			rb.velocity +=Vector2.up *Physics2D.gravity.y * (lowJumpMultiplier-1) *Time.deltaTime;
		}
	}
}
