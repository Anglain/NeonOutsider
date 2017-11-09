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
		Debug.Log("Start");
		TurnOnForDuration(false);
	}

	#region IRewindable members
	public void Rewind()
	{
		// place shield in position where it was used last time
		this.gameObject.transform.position = turnedOnPosition;

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
		this.gameObject.SetActive(true);

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

		Debug.Log("object inside shield : " + closestGo);
		if(closestGo != null)
		{
			this.gameObject.transform.parent = closestGo.transform;
			// ignore collisions between shield and it's new parent object
			Collider2D shieldCol = GetComponent<Collider2D>();
			Collider2D parentCol = closestGo.GetComponent<Collider2D>();
			Physics2D.IgnoreCollision(shieldCol, parentCol);
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
			this.gameObject.transform.parent = null;

			RewindController.Instance.RememberSkillUsage(this);
			this.gameObject.SetActive(false);
		}
	}

}
