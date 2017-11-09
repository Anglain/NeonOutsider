using UnityEngine;
public interface IRewindable
{
	/// <summary>
	/// duplicates the skill's, first-time usage
	/// and deletes the gameobject after the skill duration
	/// </summary>
	void Rewind();

	/// <summary>
	/// Get's the position where the skill will be restarted
	/// RC will put timer there
	/// </summary>
	/// <returns></returns>
	Vector3 UsagePosition();

	/// <summary>
	/// Deletes the gameobject of this skill's script
	/// </summary>
	void Dispose();
}
