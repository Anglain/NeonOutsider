using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Added to the shield prefab
/// </summary>
public class Shield : MonoBehaviour
{
	private float timeSinceShieldTurnedOn= 0;
	private const float shieldDuration = 3f;
	
	void Start()
	{
		CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
		Assert.IsNotNull(collider);
		float radius = collider.radius;

		//construct bit mask
		int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
		int enemiesLayerMask = 1 << LayerMask.NameToLayer("Enemies");
		int layerMask =  playerLayerMask | enemiesLayerMask;

		// Debug.Log(Convert.ToString(layerMask, 2).PadLeft(32, '0'));
		
		
		Collider2D[] gameObjectsInside = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
		
		// if there isn't any object of needed layer inside this collider, just let it be static
		float smallestDist = float.MaxValue;
		foreach(Collider2D col in gameObjectsInside)
		{
			float dist = Vector3.Distance(col.transform.position, this.transform.position);
			if(dist < smallestDist)
			{
				smallestDist = dist;
			}
		}


	}

	void Update()
	{
		timeSinceShieldTurnedOn += Time.deltaTime;
		if(timeSinceShieldTurnedOn >= shieldDuration)
		{
			this.gameObject.SetActive(false);
			// if(wasRemembered == false)		// add to stack if this is the first usage of skill
			// {
			// }
			SkillUsage usage = new SkillUsage(this.gameObject, Skill.Shield);
			RewindController.Instance.RememberSkillUsage(usage);
		}
	}

	public void Restart()
	{
		timeSinceShieldTurnedOn = 0;
	}
}
