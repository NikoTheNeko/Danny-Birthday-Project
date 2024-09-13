using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CameraController : MonoBehaviour{

    #region Public Variables
    [Header("Testing variables they will not actually work")]
    [Tooltip("This is how long the shake will last")]
    public float TESTDuration;
    [Tooltip("This is how intense the shake will be")]
    public float TESTShakeStrength;
    [Tooltip("This is how much the shake will affect it to calm down.")]
    [Range(0.0f, 1.0f)]
    public float TESTShakeDecay;
    [Tooltip("This is the lerp for the camera, 1 for the most intense shakes")]
    [Range(0.0f, 1.0f)]
    public float TESTLERPStrength;
    #endregion

    #region Private Variables
    #endregion

    void Update(){
        //if(Input.GetKeyDown(KeyCode.Space)){
            //ShakeCamera(TESTDuration, TESTShakeStrength, TESTShakeDecay, TESTLERPStrength);
       // }
    }


    public void ShakeCamera(float Duration, float ShakeStrength, float ShakeDecay, float LERPStrength){
        StartCoroutine(CameraShakeCoroutine(Duration, ShakeStrength, ShakeDecay, LERPStrength));
    }

    private IEnumerator CameraShakeCoroutine(float Duration, float ShakeStrength, float ShakeDecay, float LERPStrength){
        //Sets a timer to the duration
        float Timer = Duration;
        float ShakeMagnitude = ShakeStrength;
        
        //Shakes
        while(Timer > 0){
            //Generates a random value for the shake
            float ShakeX = (UnityEngine.Random.value - 0.5f) * ShakeMagnitude;
            float ShakeY = (UnityEngine.Random.value - 0.5f) * ShakeMagnitude;

            //Shakes the local position
            Vector3 ShakeNewPosition = new Vector3(ShakeX, ShakeY, 0);
            Vector3 CameraMovementPosition = Vector3.Lerp(transform.localPosition, ShakeNewPosition, LERPStrength);

            //Transform of the local position will now be the camera's new position
            transform.localPosition = CameraMovementPosition;

            //Decay the Shake
            ShakeMagnitude = Mathf.Lerp(ShakeMagnitude, 1.0f, ShakeDecay);

            //Count down timer
            Timer -= Time.deltaTime;

            yield return null;
        }
    }
}
