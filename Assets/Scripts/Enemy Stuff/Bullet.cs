using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;
using TwitchChatConnect.Manager;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TwitchChatConnect.Example.MiniGame;

public class Bullet : MonoBehaviour{

    #region Public Variables
    [Header("Game Variables")]
        [Tooltip("This is the speed at which the enemy will run towards the player")]
            public float BaseSpeed = 1.0f;
        [Tooltip("Minimum Distance to deal damage to player")]
            public float MinDistance = 1.0f;
        [Tooltip("Bullet letter used to destroy bullet")]
            public string BulletString = "D";
    [Header("Game Objects")]
        [Tooltip("Game Manager")]
        public GameManager GameManager;
        [Tooltip("Player to target who to walk to")]
            public GameObject PlayerObject;
        [Tooltip("Rigidbody of the bullet")]
            public Rigidbody BulletRigidbody;
        [Tooltip("This is the Text on the bullet")]
            public TMP_Text BulletText;
    #endregion

    #region Private Variables
    #endregion

#region Unity Functions

    //Calls at the start
    void Awake(){
        BulletText.text = BulletString.ToUpper();
    }
    // Update is called once per frame
    void Update(){
    	MoveTowardsPlayer();
    }

    #endregion

    #region Set Up Functions


    #endregion
    
    /**

    **/
    public void FuckingDies(){
        GameManager.BulletsActive.Remove(this);
        Destroy(gameObject);
    }

    /**
		Rotates towarsd the player
	**/
	private void FacePlayer(){
		transform.LookAt(PlayerObject.transform);
        
	}

    /**
    	Simple move towards the player functions
    **/
    private void MoveTowardsPlayer(){
    	//Calculates the distance between enemy and player
    	float DistanceBetweenEnemyAndPlayer = Vector3.Distance(transform.position, PlayerObject.transform.position);
    	//Checks if the enemy is too close to the player and if it is, just do not execute
    	if(DistanceBetweenEnemyAndPlayer <= MinDistance){
            GameManager.DealPlayerDamage(1);
    		FuckingDies();
    	} else {
	    	//Faces the player so it can walk towards the player
	    	FacePlayer();
	    	//Gets the forward and multiplies the speed to walk towards the player
	    	Vector3 WalkToPlayerVelocity = transform.forward * BaseSpeed;
	    	//Velocity of the enemy moves towards the player
	 		BulletRigidbody.velocity = WalkToPlayerVelocity;
 		}
    }

    public void NewLetter(string Letter){
        BulletString = Letter;
        BulletText.text = Letter;
    }
    
}
