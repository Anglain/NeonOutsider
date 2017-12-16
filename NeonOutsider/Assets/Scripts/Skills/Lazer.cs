<<<<<<< HEAD:NeonOutsider/Assets/Scripts/LazerController.cs
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour {

	// Duration of one bullet
	private float DIFF = .5f;

	public LayerMask blockingLayer;
	public LayerMask reflectingLayer;

	public KeyCode frontShootKey;
	public KeyCode upShootKey;

	// For tests needs
	public KeyCode rewindKey;

	// The max distance of laser (if it reflects, the range reset)
	public float range = 50f; 
	public float speed = 50f;


	/// <summary>
	/// Class that srore the point and its direction
	/// </summary>
	class LineCoordinates {
		public Vector2 coordinates;
		public Vector2 direction;

		public LineCoordinates(Vector2 coords, Vector2 direct){
			coordinates = coords;
			direction = direct;
		}
	}

	// Prevent frome creating new lazer bullet
	bool isShooting = false;
	bool keyUp = false;

	private KeyCode pressed = KeyCode.None;

	void FixedUpdate () {
		if ((Input.GetKey (frontShootKey)) && !isShooting) { 
			isShooting = true;
			RightShoot ();
			StartCoroutine (Timer(DIFF - .05f));
		}
		if ((Input.GetKey (upShootKey))&& !isShooting ) {
			isShooting = true;
			UpShoot ();
			StartCoroutine (Timer(DIFF - .05f));
		}

		if ((Input.GetKey (rewindKey)) && previousShootCoords.coordinates != Vector2.zero) {
				Rewind ();
		}
	}

	void Update(){
		if ((Input.GetKey (frontShootKey)) && pressed == KeyCode.None) { 
			pressed = frontShootKey;
			StartCoroutine ( DurationCalculate());
		}
		if ((Input.GetKey (upShootKey)) && pressed == KeyCode.None) {
			pressed = upShootKey;
			StartCoroutine ( DurationCalculate());
		}

		if (Input.GetKeyUp (pressed)) {
			keyUp = true;
		}
	}

	// Delay for shooting
	private IEnumerator Timer(float time){
		yield return new WaitForSeconds(time);
		isShooting = false;
	}

	public void RightShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, transform.right);
		Shoot(new LineCoordinates(transform.position, transform.right));
	}	

	public void LeftShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, -transform.right);
		Shoot(new LineCoordinates(transform.position, transform.right));
	}	

	public void UpShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, transform.up);
		Shoot(new LineCoordinates(transform.position, transform.up));
	}

	//
	//	REWIND PART
	//
	LineCoordinates previousShootCoords = new LineCoordinates(Vector2.zero, Vector2.zero);

	Vector3 UsagePosition(){
		Vector3 usagePosition = previousShootCoords.coordinates;
		return usagePosition;
	}

	public void Rewind(){
		LineCoordinates tmp = previousShootCoords;
		previousShootCoords = new LineCoordinates (Vector2.zero, Vector2.zero);
		StartCoroutine(RewindShoot(duration, tmp));
	}

	private IEnumerator RewindShoot(float duration, LineCoordinates previousShootCoords){
		float thisDuration = duration; //to save localy
		float timer = 0f;
		while (timer <= duration){
			Shoot (previousShootCoords);
			yield return new WaitForSeconds(DIFF - .05f);
			timer += (DIFF - .05f) + Time.deltaTime;
			print (duration + " t: " + timer);
		}
		yield break;
	}

	float duration; // The last duration of lazer
	private IEnumerator DurationCalculate(){ // Calculate how long player pressed key (was shooting)
		duration = 0f;
		while(true) {
			//print (duration);
			//print (keyUp);
			if (keyUp) {
				pressed = KeyCode.None;
				print (duration);
				keyUp = false;
				yield break;
			}
			yield return null;
			duration += Time.deltaTime;
		}
	}

	void Dispose(){
		// Lazer do it by itself
	}
		
	//
	// SHOOTHING PART
	//

	void Shoot(LineCoordinates coords){
		LineCoordinates bulletCoordsStart = new LineCoordinates(coords.coordinates, coords.direction);
		GameObject lazerBullet = new GameObject();
		lazerBullet.AddComponent<LineRenderer> ();
		SetLazerSettings(lazerBullet.GetComponent<LineRenderer> ());
		Shoot (bulletCoordsStart, lazerBullet);
	}
		
	private void Shoot(LineCoordinates bulletCoordsStart, GameObject lazerBullet){
		// Gets collision point with reflecting layer
		RaycastHit2D hitReflect = Physics2D.Raycast (bulletCoordsStart.coordinates+bulletCoordsStart.direction * 0.1f, bulletCoordsStart.direction, range, reflectingLayer);
		if (hitReflect.point.Equals(Vector2.zero)) { // if there is no collision -> max range of lazer
			hitReflect.point = bulletCoordsStart.coordinates + bulletCoordsStart.direction * range;
		}  

		// Gets collision point with blocking layer
		RaycastHit2D hitBlock = Physics2D.Raycast (bulletCoordsStart.coordinates, bulletCoordsStart.direction, range, blockingLayer);
		if (hitBlock.point.Equals(Vector2.zero)) { // if there is no collision -> max range of lazer
			hitBlock.point = bulletCoordsStart.coordinates + bulletCoordsStart.direction * range;
		}

		// Gets the closest collision poin
		LineCoordinates bulletCoordsNext;
		if ((hitBlock.point - bulletCoordsStart.coordinates).magnitude 
			> (hitReflect.point - bulletCoordsStart.coordinates).magnitude) { // if it is reflecting layer
			bulletCoordsNext = new LineCoordinates (hitReflect.point, Vector2.Reflect (bulletCoordsStart.direction, hitReflect.normal));
		}  else { // if it is blocking layer -> we don't need to reflect lazer -> that's why Vector2.zero
			bulletCoordsNext = new LineCoordinates (hitBlock.point, Vector2.zero);
		}

		// Start shooting (moves lazerBullet) from bulletCoordsStart to bulletCoordsNext
		StartCoroutine (LazerMove(bulletCoordsStart, bulletCoordsNext, lazerBullet));
	}

	private IEnumerator LazerMove(LineCoordinates from, LineCoordinates to, GameObject lazerBullet){
		
		// Timers for mooving tail and head of the bullet
		float timer = 0f;
		float timerTail = 0f;
		float timerHead = 0f;

		// Time needed to collide with object 
		float time = (to.coordinates - from.coordinates).magnitude / speed;

		// Delta coordinates (move)
		Vector2 addDist;

		LineRenderer lr = lazerBullet.GetComponent<LineRenderer> ();

		// The head amd the tail of LineRenderer
		Vector2 head = from.coordinates, tail = from.coordinates;

		// Prevent from multiple reflection
		bool nextCoroutineStarted = false;

		while (true) {
			addDist = from.direction * speed * Time.deltaTime; // Calculate the delta of move
			timer += Time.deltaTime;
				

			if (timer >= DIFF) { // the difference between head and tail is 0.5f sec
				timerTail += Time.deltaTime;
				tail += addDist; // Moves the tail of the bullet
				if (timerTail >= time) {
					lr.enabled = false;

					if (to.direction == Vector2.zero) {
						gameObject.SetActive (false);
					}
					//Destroy (lazerBullet);

					yield break;
				}
			}
				
		
			timerHead += Time.deltaTime;
			if (timerHead <= time) {
				head += addDist; //Moves the head of the bullet
			}  else {
				head = to.coordinates;
				// If it is reflecting layer -> start new shoot from this point
				if (!nextCoroutineStarted && to.direction != Vector2.zero) {
					nextCoroutineStarted = true; 
					Shoot (to); // The next leg of lazer 
				}
			}

			// Set the tail and head of LineRenderer
			lr.SetPosition (0, tail);
			lr.SetPosition (1, head);

			yield return new WaitForFixedUpdate();
		}
	}


	//
	// DECORATION PART
	//
	public float width = .2f;
	public int sortingOrder = 3;
	public Material material;

	private void SetLazerSettings(LineRenderer lazer){
		lazer.endWidth = .2f;
		lazer.startWidth = .2f;
		lazer.startColor = Color.white;
		lazer.endColor = Color.white;
		lazer.sortingOrder = sortingOrder;
		lazer.material = material;
		lazer.useWorldSpace = true;
		lazer.enabled = true;

		if (material == null) {
			Debug.LogError ("No material for lazer! [LAZER_CONTROLLER.CS]");
		}
	}
}


=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour {

	// Duration of one bullet
	private float DIFF = .5f;

	public LayerMask blockingLayer;
	public LayerMask reflectingLayer;

	public KeyCode frontShootKey;
	public KeyCode upShootKey;

	// For tests needs
	public KeyCode rewindKey;

	// The max distance of laser (if it reflects, the range reset)
	public float range = 50f; 
	public float speed = 50f;

	/// <summary>
	/// Class that srore the point and its direction
	/// </summary>
	class LineCoordinates {
		public Vector2 coordinates;
		public Vector2 direction;

		public LineCoordinates(Vector2 coords, Vector2 direct){
			coordinates = coords;
			direction = direct;
		}
	}

	// Prevent frome creating new lazer bullet
	bool isShooting = false;
	bool keyUp = false;

	private KeyCode pressed = KeyCode.None;

	/// <summary>
	/// This method must be envoked manually after instantiating the prefab
    /// starts the lazer shooting mechanism in the front direction of player
	/// </summary>
	public void AttemptShootFront(){
		if ((Input.GetKey (frontShootKey)) && !isShooting) {
			isShooting = true;
			RightShoot ();
			StartCoroutine (Timer(DIFF - .05f));
		}
	}

	/// <summary>
	/// This method must be envoked manually after instantiating the prefab
    /// starts the lazer shooting mechanism in the upwards direction of player
	/// </summary>
	public void AttemptShootUp(){
		if ((Input.GetKey (upShootKey))&& !isShooting ) {
		
			isShooting = true;
			UpShoot ();
			StartCoroutine (Timer(DIFF - .05f));
		}
			
	}

	void FixedUpdate() {
		 
			
		
			

		if ((Input.GetKey (rewindKey)) && previousShootCoords.coordinates != Vector2.zero) {
				Rewind ();
		}
	}

	void Update(){
		if ((Input.GetKey (frontShootKey)) && pressed == KeyCode.None) { 
			pressed = frontShootKey;
			StartCoroutine ( DurationCalculate());
		}
		if ((Input.GetKey (upShootKey)) && pressed == KeyCode.None) {
			pressed = upShootKey;
			StartCoroutine ( DurationCalculate());
		}

		if (Input.GetKeyUp (pressed)) { 
			keyUp = true;
		}
	}

	// Delay for shooting
	private IEnumerator Timer(float time){
		yield return new WaitForSeconds(time);
		isShooting = false;
	}

	public void RightShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, transform.right);
		Shoot(new LineCoordinates(transform.position, transform.right));
	}	

	public void LeftShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, -transform.right);
		Shoot(new LineCoordinates(transform.position, transform.right));
	}	

	public void UpShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, transform.up);
		Shoot(new LineCoordinates(transform.position, transform.up));
	}

	#region IRewindable  members
	LineCoordinates previousShootCoords = new LineCoordinates(Vector2.zero, Vector2.zero);

	Vector3 UsagePosition(){
		Vector3 usagePosition = previousShootCoords.coordinates;
		return usagePosition;
	}

	public void Rewind(){
		LineCoordinates tmp = previousShootCoords;
		previousShootCoords = new LineCoordinates (Vector2.zero, Vector2.zero);
		StartCoroutine(RewindShoot(duration, tmp));
	}

	void Dispose(){
		// Lazer do it by itself - IT DOES NOT

	}

	private IEnumerator RewindShoot(float duration, LineCoordinates previousShootCoords){
		float thisDuration = duration; //to save localy
		float timer = 0f;

		while (timer <= duration){

			Shoot (previousShootCoords);
			yield return new WaitForSeconds(DIFF - .05f);

			timer += (DIFF - .05f) + Time.deltaTime;
			print (duration + " t: " + timer);
		}
		yield break;
	}
	#endregion

	float duration; // The last duration of lazer
	private IEnumerator DurationCalculate(){ // Calculate how long player pressed key (was shooting)
		duration = 0f;
		while(true) {
			//print (duration);
			//print (keyUp);
			if (keyUp) {
				pressed = KeyCode.None;
				print (duration);
				keyUp = false;
				yield break;
			}
			yield return null;
			duration += Time.deltaTime;
		}
	}
		
	//
	// SHOOTHING PART
	//
	void Shoot(LineCoordinates coords){
		LineCoordinates bulletCoordsStart = new LineCoordinates(coords.coordinates, coords.direction);
		GameObject lazerBullet = new GameObject();
		lazerBullet.AddComponent<LineRenderer> ();
		SetLazerSettings(lazerBullet.GetComponent<LineRenderer> ());
		Shoot (bulletCoordsStart, lazerBullet);
	}
		
	private void Shoot(LineCoordinates bulletCoordsStart, GameObject lazerBullet){
		// Gets collision point with reflecting layer
		RaycastHit2D hitReflect = Physics2D.Raycast (bulletCoordsStart.coordinates+bulletCoordsStart.direction * 0.1f, bulletCoordsStart.direction, range, reflectingLayer);
		if (hitReflect.point.Equals(Vector2.zero)) { // if there is no collision -> max range of lazer
			hitReflect.point = bulletCoordsStart.coordinates + bulletCoordsStart.direction * range;
		}  

		// Gets collision point with blocking layer
		RaycastHit2D hitBlock = Physics2D.Raycast (bulletCoordsStart.coordinates, bulletCoordsStart.direction, range, blockingLayer);
		if (hitBlock.point.Equals(Vector2.zero)) { // if there is no collision -> max range of lazer
			hitBlock.point = bulletCoordsStart.coordinates + bulletCoordsStart.direction * range;
		}

		// Gets the closest collision poin
		LineCoordinates bulletCoordsNext;
		if ((hitBlock.point - bulletCoordsStart.coordinates).magnitude 
			> (hitReflect.point - bulletCoordsStart.coordinates).magnitude) { // if it is reflecting layer
			bulletCoordsNext = new LineCoordinates (hitReflect.point, Vector2.Reflect (bulletCoordsStart.direction, hitReflect.normal));
		}  else { // if it is blocking layer -> we don't need to reflect lazer -> that's why Vector2.zero
			bulletCoordsNext = new LineCoordinates (hitBlock.point, Vector2.zero);
		}

		// Start shooting (moves lazerBullet) from bulletCoordsStart to bulletCoordsNext
		StartCoroutine (LazerMove(bulletCoordsStart, bulletCoordsNext, lazerBullet));
	}

	private IEnumerator LazerMove(LineCoordinates from, LineCoordinates to, GameObject lazerBullet){
		
		// Timers for mooving tail and head of the bullet
		float timer = 0f;
		float timerTail = 0f;
		float timerHead = 0f;

		// Time needed to collide with object 
		float time = (to.coordinates - from.coordinates).magnitude / speed;

		// Delta coordinates (move)
		Vector2 addDist;

		LineRenderer lr = lazerBullet.GetComponent<LineRenderer> ();

		// The head amd the tail of LineRenderer
		Vector2 head = from.coordinates, tail = from.coordinates;

		// Prevent from multiple reflection
		bool nextCoroutineStarted = false;

		while (true) {
			addDist = from.direction * speed * Time.deltaTime; // Calculate the delta of move
			timer += Time.deltaTime;
				

			if (timer >= DIFF) { // the difference between head and tail is 0.5f sec
				timerTail += Time.deltaTime;
				tail += addDist; // Moves the tail of the bullet
				if (timerTail >= time) {
					lr.enabled = false;

					if (to.direction == Vector2.zero) {
						gameObject.SetActive (false);
					}
					//Destroy (lazerBullet);

					yield break;
				}
			}
				
		
			timerHead += Time.deltaTime;
			if (timerHead <= time) {
				head += addDist; //Moves the head of the bullet
			}  else {
				head = to.coordinates;
				// If it is reflecting layer -> start new shoot from this point
				if (!nextCoroutineStarted && to.direction != Vector2.zero) {
					nextCoroutineStarted = true; 
					Shoot (to); // The next leg of lazer 
				}
			}

			// Set the tail and head of LineRenderer
			lr.SetPosition (0, tail);
			lr.SetPosition (1, head);

			yield return new WaitForFixedUpdate();
		}
	}

	//
	// DECORATION PART
	//
	public float width = .2f;
	public int sortingOrder = 3;
	public Material material;

	private void SetLazerSettings(LineRenderer lazer){
		lazer.endWidth = .2f;
		lazer.startWidth = .2f;
		lazer.startColor = Color.white;
		lazer.endColor = Color.white;
		lazer.sortingOrder = sortingOrder;
		lazer.material = material;
		lazer.useWorldSpace = true;
		lazer.enabled = true;

		if (material == null) {
			Debug.LogError ("No material for lazer! [LAZER_CONTROLLER.CS]");
		}
	}
}


>>>>>>> 2f72992ace2df80b94a09c9f479c2130c77a637e:NeonOutsider/Assets/Scripts/Skills/Lazer.cs
