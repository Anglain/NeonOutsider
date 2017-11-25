using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Is attached to the timer gameobject
/// that appeares when the skill is used
/// controls the percentage of timer's fill
/// </summary>
public class RewindTimer : MonoBehaviour 
{
    public Image innerImage;
    public float fillDuration;     // time needed to fill whole circle

    void Awake()
    {
        Assert.IsNotNull(innerImage);
        Reset();
    }

    void OnEnable()
    {
        // Debug.Log("On enable");
        // Reset();
    }

   /// <summary>
   /// starts graphical effect of filling the timer using coroutine (other thread)
   /// If the timer fills to 100%, last skill will be rewinded
   /// can be stopped earlier, using FinishTimer()
   /// </summary>
    public void StartFill()
    {
        if(RewindController.Instance.HasSkills() == false)
        {
            Debug.LogError("started filling timer with no skills in stack!");
        }
        // Debug.Log("Started StartFill go is " + gameObject.activeSelf);
        StartCoroutine(FillStartCoroutine());
        // Debug.Log("Ended StartFill go is " + gameObject.activeSelf);
    }

    /// <summary>
    /// calls the duplicateLastSkill method before
    /// the fill coroutine is finished
    /// </summary>
    public void FinishFill()
    {
        // Don't have the slightest ide why tth FUCK it gives the strangest bug
        // when stopping coriutine by name
        // although there is only 1 running 
        StopAllCoroutines();
        // StopCoroutine(FillStartCoroutine());
        RewindController.Instance.DuplicateLastSkill();
    }

    /// <summary>
    /// makes the timer ready to use again
    /// </summary>
    public void Reset()
    {
        innerImage.fillAmount = 0f;
    }
    
    private IEnumerator FillStartCoroutine()
    {
        Debug.Log("fill routine");
        while(innerImage.fillAmount < 1.0f)
        {
            innerImage.fillAmount +=  (Time.deltaTime / fillDuration);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("coroutine did not stop!");
        RewindController.Instance.DuplicateLastSkill();
    }
}
