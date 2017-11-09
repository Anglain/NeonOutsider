using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// keeps track of recently used abilities,
/// supplies methods for player to be able to use the skill again
/// </summary>
public class RewindController : Singleton<RewindController>
{
	private int maxUsageEntries = 3;
	private LinkedList<SkillUsage> skillMemory;

	public void DuplicateLastSkill()
	{
		SkillUsage lastSkill = skillMemory.First.Value;
		skillMemory.RemoveFirst();

		switch(lastSkill.skillType)
		{
			case Skill.Shield:
			case Skill.Wall:
			{

				break;
			}

			case Skill.Gun:
			{
				break;
			}
		}
	}
	
	public void RememberSkillUsage(SkillUsage usage)
	{
		int skillsRemembered = skillMemory.Count;
		skillMemory.AddFirst(usage);

		if(skillsRemembered >= maxUsageEntries)
		{
			skillMemory.RemoveLast();
		}
	}
}

/// <summary>
/// Keeps all data about skill usage 
/// </summary>
public struct SkillUsage
{
	// public readonly Vector3 position;
	public readonly GameObject gameObject;
	public readonly Skill skillType;

	/// <summary>
	/// pass a prefab of wall or shield when they are deactivated
	/// or null otherwise
	/// </summary>
	/// <param name="gameObject"></param>
	/// <param name="skillType"></param>
	public SkillUsage(GameObject gameObject, Skill skillType)
	{
		this.gameObject = gameObject;
		this.skillType = skillType;
	}
}

public enum Skill
{
	Shield, Wall, Gun
}
