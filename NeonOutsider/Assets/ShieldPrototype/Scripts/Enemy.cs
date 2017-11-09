using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{

	// Use this for initialization
	public bool moves = true;
	public float speed = 1;
	public float walkTime = 5;
	float timeSinceDirChanged = 0f;
	public bool doubleWalk = false;
	bool goingLeft = true;
	
	// Update is called once per frame
	void Update () 
	{

		if(moves == false)
			return;
			
		transform.position += goingLeft? -Vector3.left * speed * Time.deltaTime : Vector3.left * speed * Time.deltaTime;
		timeSinceDirChanged += Time.deltaTime;
		if(doubleWalk == true && timeSinceDirChanged >= walkTime / 2)	// change direction
		{
			goingLeft = !goingLeft;
			doubleWalk = false;
			timeSinceDirChanged = 0;
		}
		else if (timeSinceDirChanged >= walkTime)
		{
			goingLeft = !goingLeft;
			timeSinceDirChanged = 0;
		}
	}
}
