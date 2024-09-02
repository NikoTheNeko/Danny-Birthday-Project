using System.Collections;
using System.Collections.Generic;
using TwitchChatConnect.Example.MiniGame;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/**
Have you heard of the critically acclaimed MMORPG Final Fantasy XIV?
With an expanded free trial which you can play through the entirety of
A Realm Reborn and the award-winning Heavensward,
and thrilling Stormblood expansions
up to level 70 for free with no restrictions on playtime.
**/

public class Enemy : MonoBehaviour{

    #region Public Variables

        #region Enemy Parts
        [Header("Enemy Parts")]
            [Tooltip("Array of enemy's parts, used to keep track of the enemy parts LMAO")]
            public EnemyPart[] PartsArray;

            public string[] PartsNames;
            [Tooltip("This is the dictionary for the enemy parts, used to keep track of voting")]
            [SerializeField]public Dictionary<string, EnemyPart> PartsDictionary = new Dictionary<string, EnemyPart>();
        #endregion
        
        #region Enemy Game Variables
        [Header("Enemy Game Variables")] 
            [Tooltip("Health of the enemy")]
            public int Health = 3;

            [Tooltip("Enemy Number, this is the number the players will access through the ! commands (ie. !1Head <-- damn no head???)")]
			public string EnemyNumber;
            
			[Tooltip("This is the speed at which the enemy will run towards the player")]
			public float EnemyBaseSpeed = 1.0f;
			
			[Tooltip("The min distance it'll be from the player")]
			[Range(15.0f, 100.0f)]
			public float MinDistance = 1.0f;
			
			[Tooltip("This is the direction the enemy will walk into.")]
			public Vector3 DirectionVector;

            [Tooltip("This is the end point location where the enemy will walk to")]
            public Transform EndPointLocation;

            [Tooltip("Minimum time to shoot")]
            public float ShootTimeMin = 3.0f;
            [Tooltip("Maximum time to shoot")]
            public float ShootTimeMax = 6.0f;

            [Tooltip("Minimum time to melee")]
            [Range(3.0f, 30.0f)]
            public float MeleeTimeMin = 2.0f;
            [Tooltip("Maxiumum time to melee")]
            [Range(3.0f, 30.0f)]
            public float MeleeTimeMax = 4.0f;
            [Tooltip("Array of letters for the bullets, primarily made this way for easter eggs")]
            public string[] BulletLetterArray = {"A"};
        #endregion

        #region Game Objects
        [Header("Other Game Objects")]
            [Tooltip("GameManager for the game")]
            public GameManager GameManager;
            
            [Tooltip("Player to target who to walk to")]
            public GameObject PlayerObject;
            
            [Tooltip("Rigidbody for the enemy")]
            public Rigidbody EnemyRigidbody;

        [Header("Prefabs")]
            [Tooltip("Bullet Prefab")]
            public GameObject BulletPrefab;
        #endregion

    #endregion

    #region Private Variables
    	//Speed of the enemy
        [Tooltip("CURRENT Speed of the enemy, calcualted through buffs and stuff")]
    	[SerializeField]private float CurrentSpeed;
    	//Allows the enemy to move
    	private bool AbleToMove = true;
        //Allows the enemy to shooter
        private bool CanShoot = false;
        //Amount of guns the enemy has
        private int GunsHeld = 0;
        //Allows the enemy to melee
        private bool CanMelee = false;
        //Is in the distance to melee
        private bool InMeleeRange = false;
        //Amount of Melee Held
        private int MeleeHeld = 0;
    
        private float ShootingTimer = 69420.0f;
        private float MeleeTimer = 69420.0f;
    	
    #endregion

    #region Unity Functions
    void Awake(){
        PartsNames = new string[PartsArray.Length];
    }
    // Start is called before the first frame update
    void Start(){
        //Faces enemy towards the player
        //FacePlayer();
        //Normalizes the direction vector
        DirectionVector.Normalize();
        //Sets current speed to the correct one with modifiers
        CurrentSpeed = EnemyBaseSpeed;
        //Calculates the buffs
        UpdateBuffs();
    }

    // Update is called once per frame
    void Update(){
    	//If it is able to move
    	if(AbleToMove)
	        MoveTowardsPoint();
        if(CanShoot)
            ShootCharacter();
        if(InMeleeRange && CanMelee)
            MeleeCharacter();
    }

    #endregion

    #region Part Checker

    /**
        This function goes through all of the enemy parts 
        and then like buffs the enemy when needed.
        List of "buffs" and functions
        Speed - Increases the speed of an enemy by a percentage, be written in percentage formate (ie .50 = 50%)
        Shoot - Shoots the player, this will spawn a bullet to shoot the player. Just allows the shoot player function to be enabled
        Melee - Melees the player, this will cause the enemy to melee when they're close
        Brain - This will instantly kill the enemy lol ignore this as it's more of a label than a thing to deal with
    **/
    public void UpdateBuffs(){
        //Resets the buffs
        CurrentSpeed = EnemyBaseSpeed;
        CanShoot = false;
        GunsHeld = 0;
        CanMelee = false;
        MeleeHeld = 0;

        //Goes through the part list
        foreach(EnemyPart Part in PartsArray){
            //If the part is nto active, skip.
            if(!Part.PartActive)   
                //Skip if the part if not active.
                continue;

            //Checks the buff in a switch statement
            switch(Part.FunctionType){
                //Increases speed
                case PartFunction.Speed:
                    float IncreaseSpeedFloat = EnemyBaseSpeed * Part.FunctionModifier;
                    CurrentSpeed += IncreaseSpeedFloat;
                break;

                //Allows Shooting
                case PartFunction.Shoot:
                    CanShoot = true;
                    GunsHeld += 1;
                break;

                //Allows Melee
                case PartFunction.Melee:
                    CanMelee = true;
                    MeleeHeld += 1;
                break;

                //This doesn't do anything it's just a label, also you're big brained neko
                //Your brain is as big ass your ass 😩😩😩💦💦💦
                case PartFunction.Brain:
                //Nothing used as a label. Remember to set damage to like 100 or something to instantly kill the enemy!
                break;
            }
        }

    }

    #endregion

    #region Set Up Functions
    
    /**
        Converts the names of the parts to the array
    **/
    public void ArrayToDictionary(){
        //Sets the label starter for the enemy number and command
        string LabelStarter = "!" + EnemyNumber;
        //Loops through the array
        for(int i = 0; i < PartsArray.Length; i++){
            EnemyPart TempPart = PartsArray[i];
            string EnemyLabelName = LabelStarter + TempPart.PartName;
            PartsDictionary.Add(EnemyLabelName, TempPart);
        }
    }

    /**
        Sets up the labels for the enemy
    **/
    public void NameLabels(){
        string LabelStarter = "!" + EnemyNumber;
        
        //Loops through the array
        for(int i = 0; i < PartsArray.Length; i++){
            //Gets the part as a temp
            EnemyPart TempPart = PartsArray[i];
            //Creates the label for the part (![number][partname])
            string EnemyLabelName = LabelStarter + TempPart.PartName;
            //Votes required is calculated by the amount of players active and rounds down. (truncated through the int cast)
            TempPart.VotesRequiredRounded = (int)(TempPart.PercentageOfVotesRequired * GameManager.PlayersActive);
            //If Votes Required is 0 then set it to 1
            //Edge Case check, if it's 0 then like lol it'll break oopy
            if(TempPart.VotesRequiredRounded <= 0)
            	TempPart.VotesRequiredRounded = 1;
            //Adjusts the text on the label
            TempPart.VotingText.text = EnemyLabelName + "\n" + 0 + "/" + TempPart.VotesRequiredRounded;
            //Puts the labels in an array
            PartsNames[i] = EnemyLabelName;
        }
    }
    


    /**
        Updates the parts labels
    **/
    public void UpdateLabel(string TargetName, int VoteCountUpdater){
        //If the part contains the name then update the label
        if(PartsDictionary.ContainsKey(TargetName)){
            //Debug.Log("Adjusting label for " + TargetName);
            //Gets the part as a temp
            EnemyPart TempPart = PartsDictionary[TargetName];
            //TempPart.VotesRequiredRounded = (int)(TempPart.PercentageOfVotesRequired * GameManager.PlayersActive);
            //Adjusts the text on the label
            TempPart.VotingText.text = TargetName;
            
            //If the part is nto active
            if(!TempPart.PartActive){
            	TempPart.VotingText.text += "\n" + "Broken!";
            } else {
	            TempPart.VotingText.text += "\n" + VoteCountUpdater + "/" + TempPart.VotesRequiredRounded;
            }
            
            
        }
    }
    
    /**
        This sets the votes of the parts to the correct number
    **/
    public void UpdateRequiredVotes(){
   		//Loops through the array
        for(int i = 0; i < PartsArray.Length; i++){
            //Gets the part as a temp
            EnemyPart TempPart = PartsArray[i];
            //Votes required is calculated by the amount of players active and rounds down. (truncated through the int cast)
            TempPart.VotesRequiredRounded = (int)(TempPart.PercentageOfVotesRequired * GameManager.PlayersActive);
            
            //If Votes Required is 0 then set it to 1 
            //Edge Case check, if it's 0 then like lol it'll break oopy
            if(TempPart.VotesRequiredRounded <= 0)
            	TempPart.VotesRequiredRounded = 1;
            //Adjusts the text on the label
            TempPart.VotingText.text = TempPart.PartName;
            
            //If the part is nto active
            if(!TempPart.PartActive){
            	TempPart.VotingText.text += "\n" + "Destroyed!";
            } else {
	            TempPart.VotingText.text += "\n" + 0 + "/" + TempPart.VotesRequiredRounded;
            }
            
        }
    }

    #endregion
    
    #region EnemyBehavior
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
    		AbleToMove = false;
    	} else {
	    	//Faces the player so it can walk towards the player
	    	//FacePlayer();
	    	//Gets the forward and multiplies the speed to walk towards the player
	    	Vector3 WalkToPlayerVelocity = DirectionVector * CurrentSpeed;
	    	//Velocity of the enemy moves towards the player
	 		EnemyRigidbody.velocity = WalkToPlayerVelocity;
 		}
    }

    /**
        This moves the enemy towards the point desginated
    **/
    private void MoveTowardsPoint(){
        //Moves the object towards the point designated
        Vector3 MovementVector;
        MovementVector = Vector3.MoveTowards(EnemyRigidbody.position, EndPointLocation.position, CurrentSpeed * Time.deltaTime);
        EnemyRigidbody.position = MovementVector;

        //If it can melee the character
        if(CanMelee){
            //Calculates the distance between enemy and player
            float DistanceBetweenEnemyAndPlayer = Vector3.Distance(transform.position, PlayerObject.transform.position);
            //Debug.Log("Distance: " + DistanceBetweenEnemyAndPlayer);
            //Checks if the enemy is too close to the player and if it is, just do not execute
            if(DistanceBetweenEnemyAndPlayer <= MinDistance){
                //Debug.Log("In Melee Range!");
                InMeleeRange = true;
            }
        }
    }
    
    //Sets the game object inactive
    private void FuckingDies(){
    	this.gameObject.SetActive(false);
    }
    
    #endregion

	#region Part Behaviors
	
    /**
        This destroys the enemey's part, then updates it accordingly
        Make sure to call fucking dies if the health becomes below 0
        that dude is fuckin dead LMAO
    **/
	public void DestroyPart(EnemyPart PartToDestroy){
        StartCoroutine(FinishTimestop());
		//Deals damage
		Health -= PartToDestroy.Damage;
		//If the health is lower than 0 then like die ig
		//tbh if i was an enemy i'd just like live
		if(Health <= 0){
			FuckingDies();
		}
		//Sets the partactive to false to stop the effect
		PartToDestroy.PartActive = false;
		//Sets the WHOLE ASS GAME OBJECT to false so it's just fuckin GONE!!!
		//Note this is a two step process (One to deactive effects then one to deactivate it visually)
		PartToDestroy.gameObject.SetActive(false);

        //Updates the buffs to match the changes
        UpdateBuffs();
	}

    /**
        Melees characters if they are in a certain distance
        Before they're in melee range, this will check for the distance between
        them and the player then swap to melee range
    **/
    public void MeleeCharacter(){
        Debug.Log("I'M TRYING TO BEAT ASS");
        //Initial Timer Generation
        //If you can Melee though actually put in the Meleey time yipee!!!
        if(MeleeTimer == 69420.0f){
            Debug.Log("Starting Melee Timer");
            //Generates the Melee time for the timer
            MeleeTimer = UnityEngine.Random.Range(MeleeTimeMin, MeleeTimeMax);
        }

        //If the Melee timer is MORE than 0, tick down.
        if(MeleeTimer > 0)
            MeleeTimer -= Time.deltaTime;
            Debug.Log("Melee Timer at: " + MeleeTimer);

        //If the Melee timer is less than zero
        // - whack a bitch
        // - reset the timer
        if(MeleeTimer <= 0){
            Debug.Log("They are beating his ass");
            GameManager.DealPlayerDamage(2);
            MeleeTimer = UnityEngine.Random.Range(MeleeTimeMin, MeleeTimeMax);
        }
    }

    /**
        This will shoot players. If they are in melee range they will STOP
        This loser will spawn as many bullets as it has guns (GunsHeld = number of bullets spanwed)
        bullets need to be made still hehe
    **/
    public void ShootCharacter(){
        //Initial Timer Generation
        //If you can shoot though actually put in the shooty time yipee!!!
        if(ShootingTimer == 69420.0f){
            //Generates the shoot time for the timer
            ShootingTimer = UnityEngine.Random.Range(ShootTimeMin, ShootTimeMax);
        }

        //If the shoot timer is MORE than 0, tick down.
        if(ShootingTimer > 0)
            ShootingTimer -= Time.deltaTime;

        //If the shoot timer is less than zero
        // - Spawn a bullet
        // - Assign it a letter
        // - Add it to the list in the game manager
        // - piss n shit or something
        if(ShootingTimer <= 0){
            //Creates a new bullet
            //Bullet NewShootyBoy = new Bullet();
            //Randomizes a letter for it
            //NewShootyBoy.BulletString = "A";
            Quaternion Rotato = new Quaternion(0,0,0,0);
	        Vector3 BulletSpawnLocation = transform.position;
            GameObject BulletSpawned = Object.Instantiate(BulletPrefab, BulletSpawnLocation, Rotato);

            //Sets the bullet components
            //Generates a letter for the array.
            int BulletLetterIndex = (int)math.round(UnityEngine.Random.Range(0, BulletLetterArray.Length));
            Debug.Log("Bullet Letter Index: " + BulletLetterIndex);
            BulletSpawned.GetComponent<Bullet>().NewLetter(BulletLetterArray[BulletLetterIndex]);
            BulletSpawned.GetComponent<Bullet>().GameManager = GameManager;
            BulletSpawned.GetComponent<Bullet>().PlayerObject = PlayerObject;

            //Adds it to the game manager
            GameManager.BulletsActive.Add(BulletSpawned.GetComponent<Bullet>());

            //Generates the shoot time for the timer
            ShootingTimer = UnityEngine.Random.Range(ShootTimeMin, ShootTimeMax);
        }

        
    }

	#endregion

    #region Coroutines
    //Timestop waiter
    private IEnumerator FinishTimestop(){
        while(Time.timeScale != 1.0f){
            yield return null;
        }
    }
    #endregion

}
