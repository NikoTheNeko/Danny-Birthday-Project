using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiPlateMover : MonoBehaviour{

    #region Public Variables
    public Rigidbody Plates;
    public Vector3 MovementVector = new Vector3(0,0,0);

    public float DestroyTime = 10f;
    #endregion

    #region Private Variables
    #endregion

    // Start is called before the first frame update
    void Start(){
        GameObject.Destroy(this.gameObject, DestroyTime);
    }

    // Update is called once per frame
    void Update(){
        Plates.velocity = MovementVector;        
    }
}
