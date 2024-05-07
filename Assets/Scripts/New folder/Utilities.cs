using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities : MonoBehaviourSingleton<Utilities>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator CheckAnimationCompleted(Animator anim, string CurrentAnim, Action Oncomplete)
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnim) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        { yield return null; }
        if (Oncomplete != null)
            Oncomplete();
    }
    public IEnumerator WaitforFrames(int frames = 1,Action callBack = null) 
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        if (callBack != null)
        {
            callBack.Invoke();
        }
    }
    public IEnumerator WaitforSeconds(int seconds = 0, Action callBack = null)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (callBack != null)
        {
            callBack.Invoke();
        }

    }
}
