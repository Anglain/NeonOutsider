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
    public float fillDuration;     // time needed to fill whole circle



    void Start()
    {
        innerImage.fillAmount = 0;
    }

   /// <summary>
   /// starts graphical effect of filling the timer using coroutine (other thread)
   /// If the timer fills to 100%, last skill will be rewinded
   /// can be stopped earlier, using FinishTimer()
   /// </summary>
    public void StartFill()
    {
        StartCoroutine(FillStartCoroutine());
    }

    /// <summary>
    /// calls the duplicateLastSkill method before
    /// the fill coroutine is finished
    /// </summary>
    public void FinishFill()
    {
        StopCoroutine(FillStartCoroutine());
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
        Debug.Log(innerImage.fillAmount);
        while(innerImage.fillAmount < 1.0f)
        {
            innerImage.fillAmount +=  (Time.deltaTime / fillDuration);
            yield return new WaitForEndOfFrame();
        }
        RewindController.Instance.DuplicateLastSkill();
    }
}
