using UnityEngine;
using System.Collections;

public class PlatformerCharacterAlternative : MonoBehaviour {

	public float moveSpeed;
	public float jumpHeight;
	public Transform groundCheck;
	public float groundCheckRadius;
	public LayerMask whatIsGround;

	private bool grounded;
	private Rigidbody2D rb2D;
	private bool doubleJumped;

	void Start () {
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Update () {
		if (grounded)
			doubleJumped = false;

		if (Input.GetKeyDown (KeyCode.Space) && grounded)
			Jump ();

		if (Input.GetKeyDown (KeyCode.Space) && !doubleJumped && !grounded) {
			Jump ();
			doubleJumped = true;
		}

		if (Input.GetKey (KeyCode.A))
			rb2D.velocity = new Vector2 (-moveSpeed, rb2D.velocity.y);

		if (Input.GetKey (KeyCode.D))
			rb2D.velocity = new Vector2 (moveSpeed, rb2D.velocity.y);
	}

	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, whatIsGround);
	}

	void Jump () {
		rb2D.velocity = new Vector2 (rb2D.velocity.x, jumpHeight);
	}
}
