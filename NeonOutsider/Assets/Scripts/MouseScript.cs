using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour {
    public LayerMask enemyMask;
    public float speed = 1;
    Rigidbody2D mouseBody;
    Transform mouseTransform;
    float mouseWidth;

    // Use this for initialization
    void Start () {
        mouseTransform = this.transform;
        mouseBody = this.GetComponent<Rigidbody2D>();
        mouseWidth = this.GetComponent<SpriteRenderer>().bounds.extents.x; // width of the sprite itself
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //check of there is a ground under
        //find the enemy position, go to the left of the enemy, *width
        Vector2 lineCastPosition = mouseTransform.position - mouseTransform.right * mouseWidth;

        Debug.DrawLine(lineCastPosition, lineCastPosition + Vector2.down*2, Color.red);

        //return true if colliding with the ground 
        bool isGrounded = Physics2D.Linecast(lineCastPosition, lineCastPosition + Vector2.down * 2, enemyMask);


        //if there is no ground - turn around
        if (!isGrounded)
        {
            Vector3 currentRotation = mouseTransform.eulerAngles;
            currentRotation.y += 180;
            mouseTransform.eulerAngles = currentRotation;
        }

        //always moves forward
        Vector2 mouseVelocity = mouseBody.velocity;
        mouseVelocity.x = -mouseTransform.right.x * speed;
        //dogVelocity.x = speed;
        mouseBody.velocity = mouseVelocity;


    }
}
