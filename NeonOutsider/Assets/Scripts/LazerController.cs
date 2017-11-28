using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour {

	// Duration of one bullet
	private float DIFF = .5f;

	public LayerMask blockingLayer;
	public LayerMask reflectingLayer;

	// The max distance of laser (if it reflects, the range reset)
	public float range = 50f; 
	public float speed = 50f;
	public float lazerDuration = 3f;


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


	public void RightShoot(){
		// Save previous shoot
		Debug.Log("right shoot");
		previousShootCoords = new LineCoordinates(transform.position, transform.right);
		StartCoroutine(StartShooting(previousShootCoords, lazerDuration));
	}	

	public void LeftShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, -transform.right);
		StartCoroutine(StartShooting(previousShootCoords, lazerDuration));
	}	

	public void UpShoot(){
		// Save previous shoot
		previousShootCoords = new LineCoordinates(transform.position, transform.up);
		StartCoroutine(StartShooting(previousShootCoords, lazerDuration));
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

	//
	//
	//
	//
	//

	private bool whetherToShoot = false;
	private bool lastBullet = false;
	private IEnumerator StartShooting(LineCoordinates startCoords, float time){
		whetherToShoot = true;
		lastBullet = false;
		Debug.Log("start shoot");
		float timer = 0f;

		while (timer <= time && whetherToShoot) {
			Debug.Log (timer + " < " + time);
			Shoot(new LineCoordinates(startCoords.coordinates, startCoords.direction));
			yield return new WaitForSeconds (DIFF - .05f);
			timer += DIFF - .05f;
		}
		lastDuration = timer;
		lastBullet = true;
		yield break;
	}

	public void StopShooting(){
		whetherToShoot = false;
	}

	private IEnumerator SettingDiactive(float time){
		float timer = 0f;
		while (timer < time) {
			yield return null;
			timer += Time.deltaTime;
		}
		yield return null;
		gameObject.SetActive (false);

		//
		//
		//

		//REMEMBER HERE

		//
		//
		//


	}



	//
	//	REWIND PART
	//
	LineCoordinates previousShootCoords = new LineCoordinates(Vector2.zero, Vector2.zero);
	float lastDuration = 0f;

	Vector3 UsagePosition(){
		Vector3 usagePosition = previousShootCoords.coordinates;
		return usagePosition;
	}

	public void Rewind(){
		Debug.Log ("we r in! " + lastDuration);

		StartCoroutine(StartShooting (previousShootCoords, lastDuration));
	}

	public void Dispose(){
		Destroy (gameObject);
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
						
					Destroy (lazerBullet);

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


}

