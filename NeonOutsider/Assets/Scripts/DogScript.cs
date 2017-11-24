using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogScript : MonoBehaviour {
    public LayerMask enemyMask;
    public float speed = 1;
    Rigidbody2D dogBody;
    Transform dogTransform;
    float dogWidth;


	// Use this for initialization
	void Start () {
        dogTransform = this.transform;
        dogBody = this.GetComponent<Rigidbody2D>();
        dogWidth = this.GetComponent<SpriteRenderer>().bounds.extents.x; // width of the sprite itself
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //check of there is a ground under
        //find the enemy position, go to the left of the enemy, *width
        Vector2 lineCastPosition = dogTransform.position - dogTransform.right * dogWidth;

        //Debug.DrawLine(lineCastPosition, lineCastPosition + Vector2.down*2, Color.red);
        
        //return true if colliding with the ground 
        bool isGrounded = Physics2D.Linecast(lineCastPosition, lineCastPosition + Vector2.down*2, enemyMask);


        //if there is no ground - turn around
        if (!isGrounded)
        {
            Vector3 currentRotation = dogTransform.eulerAngles;
            currentRotation.y += 180;
            dogTransform.eulerAngles = currentRotation;
        }

        //always moves forward
        Vector2 dogVelocity = dogBody.velocity;
        dogVelocity.x = -dogTransform.right.x*speed;
        //dogVelocity.x = speed;
        dogBody.velocity = dogVelocity;


    }
}
