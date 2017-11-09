using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// keeps track of recently used abilities,
/// supplies methods for player to be able to use the skill again
/// </summary>
public class RewindController : Singleton<RewindController>
{
	public RewindTimer timer;
	private const int MAX_USAGE_ENTRIES = 3;
	private LinkedList<IRewindable> skillMemory;

	protected override void Awake()
	{
		base.Awake();
		skillMemory = new LinkedList<IRewindable>();
	}
	
	public void DuplicateLastSkill()
	{
		IRewindable lastSkill = skillMemory.First.Value;
		skillMemory.RemoveFirst();

		lastSkill.Rewind();

		Debug.Log("Timer pos : " + timer.transform.position);

		
		if(skillMemory.Count != 0)	//move timer to older skill location
		{
			IRewindable olderSkill = skillMemory.First.Value;
			timer.transform.position = olderSkill.UsagePosition();
		}
		else	// hide timer
		{
			timer.gameObject.SetActive(false);
		}
	}
	
	public void RememberSkillUsage(IRewindable skill)
	{
		int skillsRemembered = skillMemory.Count;
		skillMemory.AddFirst(skill);

		// place timer in needed location
		timer.gameObject.transform.position = skill.UsagePosition();
		// if there where no usages of skill in stack, the timer is hidden
		timer.gameObject.SetActive(true);

		if(skillsRemembered >= MAX_USAGE_ENTRIES)
		{
			// clean up the gameobject
			IRewindable lastSkill = skillMemory.Last.Value;
			lastSkill.Dispose();
			skillMemory.RemoveLast();
		}
	}
}