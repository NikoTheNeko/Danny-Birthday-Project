using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayingChecker : MonoBehaviour{
    //Variable to toggle when the animation is being played or not
    [SerializeField]
    private bool AnimationPlaying = false;

    public bool IsPlayingAnimation(){
        return AnimationPlaying;
    }
    
    public void SetAnimationPlayingTrue(){
        AnimationPlaying = true;
    }
}
