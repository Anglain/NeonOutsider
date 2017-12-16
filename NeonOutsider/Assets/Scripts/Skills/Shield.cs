using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Instances are added to the shield prefab,
/// manages the lifetime of shield object
/// </summary>
public class Shield : MonoBehaviour, IRewindable
{
	public float shieldDuration = 3f;
	public float stickRadiusPercentage = 0.6f; // from 0 to 1 - the percentage of prefabs radius
	private Vector3 turnedOnPosition;
	
	void Start()
	{
		TurnOnForDuration(false);
	}

	#region IRewindable members
	public void Rewind()
	{
		gameObject.SetActive(true);
		// place shield in position where it was used last time
		transform.position = turnedOnPosition;

		TurnOnForDuration(true);
	}

	public Vector3 UsagePosition()
	{
		return turnedOnPosition;
	}

	public void Dispose()
	{
		Destroy(this.gameObject);
	}
	#endregion

	/// <summary>
	/// turns shield on, then after the duration 
	/// if <paramref name="delete"/> = false, passes the skill to RewindController,
	/// else deletes the gameobject
	/// <param name="deleteOnEnd"></param>
	private void TurnOnForDuration(bool deleteOnEnd)
	{
		// if spawned near object - stick to it
		CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
		Assert.IsNotNull(collider);
		float radius = collider.radius * stickRadiusPercentage;

		//construct bit mask 
		int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
		int enemiesLayerMask = 1 << LayerMask.NameToLayer("Enemies");
		int layerMask =  playerLayerMask | enemiesLayerMask;

		// Debug.Log(Convert.ToString(layerMask, 2).PadLeft(32, '0'));
		
		Collider2D[] gameObjectsInside = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
		
		// if there isn't any object of needed layer inside this collider, just let it be static
		float smallestDist = float.MaxValue;
		GameObject closestGo = null;
		foreach(Collider2D col in gameObjectsInside)
		{
			float dist = Vector3.Distance(col.transform.position, this.transform.position);
			if(dist < smallestDist)
			{
				smallestDist = dist;
				closestGo = col.gameObject;
			}
		}

		if(closestGo != null)
		{
			transform.parent = closestGo.transform;
			transform.position = closestGo.transform.position;	// center it on parent's object
			// start ignoring collisions between shield and it's new parent object
			SetIsCollisionIgnored(closestGo, true);
		}

		turnedOnPosition = this.transform.position;
		StartCoroutine(ShieldCoroutine(deleteOnEnd));
	}

	// keeps shield active for duration
	IEnumerator ShieldCoroutine(bool deleteOnEnd)
	{
		float endTime = Time.realtimeSinceStartup + shieldDuration;
		while(Time.realtimeSinceStartup < endTime)
		{
			yield return new WaitForEndOfFrame();
		}

		if(deleteOnEnd)
		{
			Dispose();
		}
		else
		{
			// unlock from parent gameobject
			SetIsCollisionIgnored(transform.parent.gameObject, false);
			transform.parent = null;

			RewindController.Instance.RememberSkillUsage(this);
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// sets wether this prefab's collider ignores 
	/// collisions with all colliders of the go
	/// </summary>
	/// <param name="go"></param>
	/// <param name="isIgnored"></param>
	/// <returns></returns>
	bool SetIsCollisionIgnored(GameObject go, bool isIgnored)
	{
		Collider2D[] cols = go.GetComponentsInChildren<Collider2D>();
		
		if(cols == null)
			Debug.LogError("Trying to ignore collision betweeen wall and go with no colliders");


		Collider2D shieldCol = GetComponent<Collider2D>();
		foreach (Collider2D goCol in cols)
		{
			Physics2D.IgnoreCollision(shieldCol, goCol);
		}

		return false;
	}
}
