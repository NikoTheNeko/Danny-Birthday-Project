using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour{

    #region Public Variables
    [Header("Camera Properties")]
    [Tooltip("This is the transform of the camera so we can make it swivel")]
    public Transform CameraObject;
    [Tooltip("This is the initial starting angle of the camera")]
    public float BaseAngleHorizontal;
    [Tooltip("This is the initial starting angle of the camera")]
    public float BaseAngleVertical;

    [Header("Tilt Properties")]
    [Tooltip("This is the max tilt it would tilt horizontally in degrees")]
    public float MaxTiltHorizontal = 5f;
    [Tooltip("This is the max tilt it would tilt vertically in degrees")]
    public float MaxTiltVertical = 5f;
    [Tooltip("This is the speed for the LERP when we make the thing go wobble wobble")]
    public float TiltSpeed = 0.28f;


    #endregion

    #region Private Variables
    [Tooltip("This is the mouse position")]
    [SerializeField] private Vector3 MousePosition;


    #endregion

    void Awake() {
        //Sets the base angles to the corresponding variables
        BaseAngleVertical = CameraObject.eulerAngles.x;
        BaseAngleHorizontal = CameraObject.eulerAngles.y;
    }

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
        ClampMouseValues();
        TiltCamera();
    }
    
    #region Mouse Percentages 

    /**
        This clamps the mouse values to the size of the window
    **/
    private void ClampMouseValues(){
        //Grabs the mouse position every frame;
        MousePosition = Input.mousePosition;

        //If the moust position is NEGATIVE then no it isn't actually
        if (MousePosition.x < 0)
            MousePosition.x = 0;
        if (MousePosition.y < 0)
            MousePosition.y = 0;
        
        //If the mouse position is MORE THAN THE WIDTH OR HEIGHT not it isn't, actually.
        if(MousePosition.x > Screen.width)
            MousePosition.x = Screen.width;
        if(MousePosition.y > Screen.height)
            MousePosition.y = Screen.height;
  
    }

    /**
        This changes the camera tilt
    **/
    private void TiltCamera(){
        //Calculates the Total angle change by doubling it
        var TotalTiltHorizontal = MaxTiltHorizontal * 2;
        var TotalTiltVertical = MaxTiltVertical * 2; 

        //Gets the percentage of the Width and Height based on mouse cursor
        var MouseXPercentage = MousePosition.x / Screen.width;
        var MouseYPercentage = MousePosition.y / Screen.height;

        //Sets the angle to tilt the camera at
        var TargetTiltHorizontal = (-1 * MaxTiltHorizontal) + (MouseXPercentage * TotalTiltHorizontal) + BaseAngleHorizontal;
        var TargetTiltVertical = (1 * MaxTiltVertical) - (MouseYPercentage * TotalTiltVertical) + BaseAngleVertical;

        //Makes a new vector to do the funny LERP
        Vector3 NewTilt;
        NewTilt.x = Mathf.Lerp(CameraObject.eulerAngles.x, TargetTiltVertical, TiltSpeed);
        NewTilt.y = Mathf.Lerp(CameraObject.eulerAngles.y, TargetTiltHorizontal, TiltSpeed);
        NewTilt.z = 0;

        //Debug.Log("X angle " + TargetTiltX);
        //Debug.Log("Y angle " + TargetTiltY);

        //Sets the camera to the tilt angles
        CameraObject.eulerAngles = NewTilt;
    }
    #endregion

    /**

    **/
}
