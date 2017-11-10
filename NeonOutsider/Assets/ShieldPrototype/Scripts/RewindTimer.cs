using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Is attached to the timer gameobject
/// that appeares when the skill is used
/// controls the percentage of timer's fill
/// </summary>
public class RewindTimer : MonoBehaviour 
{
    public Image innerImage;
    public float fillSpeed;     // percents per second



    public void StartFill()
    {

    }

    public void FinishFill()
    {

    }

    /// <summary>
    /// makes the timer ready to use again
    /// </summary>
    public void Reset()
    {

    }

    /// <summary>
    /// starts graphical effect of filling the timer
    /// If the timer fills to 100%, last skill will be rewinded
    /// can be stopped earlier, using FinishTimer()
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimerFillStartCoroutine()
    {


        yield return null;
    }
	
}
