using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderControl : MonoBehaviour {

	//colliders
	public BoxCollider2D stand;
	public BoxCollider2D crouch;
	public CircleCollider2D circle;

	PlayerControllerScript playerC;
	// Use this for initialization
	void Start () {
		playerC=GetComponent<PlayerControllerScript>();
		stand.enabled=true;
		crouch.enabled=false;
		circle.enabled=true;
	}
	
	// Update is called once per frame
	void Update () {
		//if player not on ground disable circle
		if (!playerC.groundCheck)
		{
			stand.enabled=true;
			crouch.enabled=false;
			circle.enabled=false;
		}
		//if player on ground enable circle
		else
		{
			//if crouching enabling smaller collider
			if (playerC.crouching)
			{
				stand.enabled=false;
				crouch.enabled=true;
				circle.enabled=true;
			}
			//if standing enabling bigger collider
			else
			{
				stand.enabled=true;
				crouch.enabled=false;
				circle.enabled=true;
			}
		}
	}
}
