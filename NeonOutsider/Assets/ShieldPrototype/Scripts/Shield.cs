using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that has all the shield ability logic
/// Is added to the player gameobject
/// When time is rewinded, the additional shield will be spawned through this class
/// </summary>
public class Shield : MonoBehaviour
{
	// it will be our timer that will indicate where the shield was last used (for rewinding)
	public GameObject rememberedShieldPosition;
	public GameObject playersShield;	
	public GameObject shieldPrefab;
	float shieldDuration = 3f;
	float timeSinceShieldTurnedOn = 0f;
	bool shieldActive = false;

	/// <summary>
	/// turns shield on upon button press,
	/// places a gameobject label at the start point,
	/// keeps the shield on for duration, turns it off afterwards
	/// </summary>
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{	// turn shield on
			playersShield.SetActive(true);
			shieldActive = true;
			// move timer icon to be used by rewind skill
			rememberedShieldPosition.transform.position = playersShield.transform.position;
			rememberedShieldPosition.SetActive(true);
		}
		if(shieldActive)
		{
			timeSinceShieldTurnedOn += Time.deltaTime;
			if(timeSinceShieldTurnedOn >= shieldDuration)
			{
				playersShield.SetActive(false);
				shieldActive = false;
				timeSinceShieldTurnedOn = 0;
			}
		}
	}

}
