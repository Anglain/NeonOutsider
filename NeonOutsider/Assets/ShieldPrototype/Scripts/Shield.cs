using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Added to the shield prefab
/// </summary>
public class Shield : MonoBehaviour
{
	public bool wasRemembered;
	
	private float timeSinceShieldTurnedOn= 0;
	private const float shieldDuration = 3f;
	
	void Start()
	{
		 
	}

	void Update()
	{
		timeSinceShieldTurnedOn += Time.deltaTime;
		if(timeSinceShieldTurnedOn >= shieldDuration)
		{
			this.gameObject.SetActive(false);
			if(wasRemembered == false)		// add to stack if this is the first usage of skill
			{
				SkillUsage usage = new SkillUsage(this.gameObject, Skill.Shield);
				
			}
		}
	}
}
