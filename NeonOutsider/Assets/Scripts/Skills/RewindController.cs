using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
		Assert.IsNotNull(timer);
		skillMemory = new LinkedList<IRewindable>();
	}

	public bool HasSkills(){ return skillMemory.Count != 0; }
	
	public void DuplicateLastSkill()
	{
		// Debug.Log("duplicating skill");
		if(HasSkills() == false)
		{
			Debug.LogError("Called DuplicateLastSkill() with no skills in stack");
			return;
		}

		IRewindable lastSkill = skillMemory.First.Value;
		skillMemory.RemoveFirst();

		lastSkill.Rewind();
		
		timer.Reset();
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
		skillMemory.AddFirst(skill);

		// place timer in needed location
		timer.transform.position = skill.UsagePosition();
		// if there where no usages of skill in stack, the timer is hidden
		timer.gameObject.SetActive(true);

		if(skillMemory.Count > MAX_USAGE_ENTRIES)	// forget last
		{	// clean up the gameobject
			IRewindable lastSkill = skillMemory.Last.Value;
			lastSkill.Dispose();
			skillMemory.RemoveLast();
		}
	}
}