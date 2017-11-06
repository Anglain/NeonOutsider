using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed = 3f;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Debug.Lo
		transform.position += Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		
	}
}
