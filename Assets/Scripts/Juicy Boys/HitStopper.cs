using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopper : MonoBehaviour{

    #region Public Variables
    [Header("For testing purposes")]
    public float TESTDuration = 1.0f;
    public float TESTEaseback = 0.5f;
    #endregion

    #region Private Variables
    private bool StillInTimeStop = false;
    #endregion

    void Update(){
        //if(Input.GetKeyDown(KeyCode.Space))
            //HoldForHitStop(TESTDuration, TESTEaseback);

    }

    public void HitStop(float Duration, float EaseBack){
        //If it's still in a timestop just ignore it
        if(StillInTimeStop)
            return;
        Time.timeScale = 0.0f;
        StartCoroutine(HoldForHitStop(Duration, EaseBack));
    }

    //CoRoutine
    private IEnumerator HoldForHitStop(float Duration, float EaseBack){
        StillInTimeStop = true;
        //Holds executings for the timescale change
        yield return new WaitForSecondsRealtime(Duration);
        /**
        while(Time.timeScale < 0.9f){
            //Reverts it back to 1
            if(Time.timeScale < 0.90f){
                //Eases it back to 1
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, EaseBack);
            } else if(Time.timeScale > 0.95f && Time.timeScale < 1.0f){
                //If it's within margin just set it to 1.0f
                Time.timeScale = 1.0f;
            } else {
                StillInTimeStop = false;
            }
        }
        **/
        Time.timeScale = 1.0f;
        StillInTimeStop = false;
        Debug.Log("TimeScale is currently at " + Time.timeScale);



    }

}
