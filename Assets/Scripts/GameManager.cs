﻿using System;
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

/**
Have you heard of the critically acclaimed MMORPG Final Fantasy XIV?
With an expanded free trial which you can play through the entirety of
A Realm Reborn and the award-winning Heavensward,
and thrilling Stormblood expansions
up to level 70 for free with no restrictions on playtime.
**/

public class GameManager : MonoBehaviour{

    #region Public Variables
	    [Header("Enemy Related Variables")]
		    [Tooltip("The Enemies that are in the game")]
		    public Enemy[] EnemiesAvailable;
            [Tooltip("The active enemies in the game")]
            public List<Enemy> EnemiesActive;
	
	    [Header("Voting Related Stuff")]
		    [Tooltip("Dictionary of the users playing and their votes ie [PlayerName], [PartName They're Voting for]")]
		    public Dictionary<String, String> PlayerVotes = new Dictionary<String, String>();
		
		    [Tooltip("Dictionary for the voting processing ie [Part Name], [How Many Votes]")]
		    public Dictionary<String, int> PartNamesAndVotes = new Dictionary<string, int>();
		
		    [Tooltip("This is the part name to part reference")]
		    public Dictionary<String, EnemyPart> PartNamesAndEnemyPart = new Dictionary<string, EnemyPart>();
		
		    [Tooltip("Dictionary for the part names and their corresponding enemy")]
		    public Dictionary<String, Enemy> PartNamesAndEnemy = new Dictionary<string, Enemy>();
	    
	    [Header("Game Variables")]
		    [Tooltip("The amount of players currently playing")]
		    public int PlayersActive = 1;
		    
			[Tooltip("This is the MAX amount of bullets the players will have before needing to reload")]
			public int BulletsMax = 3;
			
			[Tooltip("How much health the players have MAX")]
			public int HealthMAX = 5;
			[Tooltip("Player to target who to walk to")]
            public GameObject PlayerObject;
            [Tooltip("Shooting Hitstop Length, how long time will stop")]
            public float ShootHitStopLength = 1.0f;
            
        [Header("Game Objects")]
        	[Tooltip("This is the bullet counter so we can like show you how much bullets you have")]
	        public TMP_Text BulletCounter;
		    
		    [Tooltip("Reload Minigame")]
		    public ReloadMinigame ReloadMinigameObject;

            [Tooltip("Text for the health")]
            public TMP_Text HPText;

            [Tooltip("The camera controller to shake that ass (the camera, but you shake yours neko 😩)")]
            public CameraController CameraController;
            [Tooltip("HitStopper to be cool and freeze the game")]
            public HitStopper HitStopper;
            [Tooltip("The animator that controls the camera rig")]
            public Animator CameraRig;
            [Tooltip("This is the animation checker to see if animations are running")]
            public AnimationPlayingChecker AnimationChecker;

        [Header("Bullets")]
            [Tooltip("This is a list for the bullets")]
            public List<Bullet> BulletsActive;

        [Header("Camera Shake")]
            [Tooltip("How much long the camera shakes")]
            public float HealthLossShakeDuration = 1.0f;
            [Tooltip("How much the camera shakes")]
            public float HealthLossShakeStrength = 1.0f;

        [Header("Audio SFX")]
            [Tooltip("Gunshot SFX when you shoot bang bang")]
            public AudioSource GunShotSFX;
            [Tooltip("Targeting ping when players target an enemy")]
            public AudioSource TargetingPingSFX;
            [Tooltip("PP SFX")]
            public AudioSource PPSFX;
            [Tooltip("The sound when you run out of bullets")]
            public AudioSource NoBulletsSFX;
            [Tooltip("The sound it makes when you reload")]
            public AudioSource ReloadSFX;
            public AudioSource D;
            public AudioSource Deez;

    #endregion
    

    #region Private Variables
    [SerializeField]
    [Header("Private Variables")]
    [Tooltip("This is the amount of bullets the players have left")]
	//This is just how much bullets the players have at the moment
	private int BulletsLeft;

    [SerializeField]
    [Tooltip("This is how much health the players have")]
	//This is how much health the players actually ahve
	private int Health;

    [SerializeField]
    [Tooltip("This is what wave the players are on")]
    //This is the wave the player is currently on
    private int CurrentWave = 0;
    
    private bool Wave0Spawned = false;
    private bool Wave1Spawned = false;
    private bool Wave2Spawned = false;
    private bool Wave3Spawned = false;
    #endregion

    private void Awake(){
        //DontDestroyOnLoad(gameObject);
        }

    // Start is called before the first frame update
    void Start(){
        //Assigns the enemy number to the enemy
        SetUpEnemyNumbers();
        //Sets up Twitch IRC
        SetUpTwitchIRC();
        //Sets up the voting structures
	    SetUpVotingStructures();
        
	    //Sets up bullets left to be bullets max;
	    BulletsLeft = BulletsMax;
        //Sets health to the max for initiation
        Health = HealthMAX;
        HPText.text = "Health: " + Health + "/" + HealthMAX;
        
        
        
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            AdjustCamera();
        }
        ManageWave();
    }

    #region Set Up Functions
    /**
        Sets up the Twitch IRC
        I'm gonna be real chief i don't really get it
        So I just copied the code lol
    **/
    private void SetUpTwitchIRC(){
        //Sets up the Twitch IRC
            TwitchChatClient.instance.Init(() =>
        {
            Debug.Log("Connected!");
            TwitchChatClient.instance.onChatMessageReceived += RouteMessage;
            TwitchChatClient.instance.onChatCommandReceived += RouteCommand;
            //TwitchChatClient.instance.onChatRewardReceived += ShowReward;

            TwitchUserManager.OnUserAdded += twitchUser =>
            {
                Debug.Log($"{twitchUser.Username} has connected to the chat.");
            };

            TwitchUserManager.OnUserRemoved += username =>
            {
                Debug.Log($"{username} has left the chat.");
            };
        },
        message =>
        {
            // Error when initializing.
            Debug.LogError(message);
        });
    }
    
    /**
        Sets up the enemy numbers! This is to assign all enemies a number
        and then gets their dictionaries for voting purposes
    **/
    private void SetUpEnemyNumbers(){
        //Loops through the enemy array
        for(int i = 0; i < EnemiesAvailable.Length; i++){
            EnemiesAvailable[i].EnemyNumber = i.ToString();
            EnemiesAvailable[i].NameLabels();
            EnemiesAvailable[i].ArrayToDictionary();
            Debug.Log("Setting active enemy to off");
            EnemiesAvailable[i].gameObject.SetActive(false);
            EnemiesAvailable[i].EntranceReady = true;
        }
    }

    /**
        Sets up the voting datastructures
        Parts and Votes ([PartName], [Amount of votes])
            ie (!0Head, 6)
        PartNames and Enemy Part ([PartName], [EnemyPart])
            ie (!0Head, [EnemyPart referencing to that enemy's head])
    **/
    public void SetUpVotingStructures(){
        //Loops through the enemy array
        for(int i = 0; i < EnemiesAvailable.Length; i++){
            //Loops through the enemy's Parts array
            for(int j = 0; j < EnemiesAvailable[i].PartsArray.Length; j++){
                //Adds the part name to get voted on
                PartNamesAndVotes.Add(EnemiesAvailable[i].PartsNames[j], 0);
                //Adds the part name and part so we can access it
                PartNamesAndEnemyPart.Add(EnemiesAvailable[i].PartsNames[j], EnemiesAvailable[i].PartsArray[j]);
                //Adds the part name to the enemy
                PartNamesAndEnemy.Add(EnemiesAvailable[i].PartsNames[j], EnemiesAvailable[i]);
            }
        }
    }
    #endregion
    
    #region Twitch IRC Processing Functions
    /**
        This routes the players command to the correct place
    **/
    private void RouteCommand(TwitchChatCommand ChatCommandMessage){
        //Breaks down the players names and message, that's all we need for this project
        //If we need more I'm sure we can extract it somehwere
        //Player is the username of the person
        string Player = ChatCommandMessage.User.DisplayName;
        //Target is the player message, aka what they're trying to target in the game
        string Target = ChatCommandMessage.Message;
        Target = Target.ToUpper();

        //Debug.Log(Player + " typed " + Target);

        //Switch statement to reroute the player
        switch(Target){
            //This is for when the players join
            case "!JOIN":
                //Players Active will go up with the player joinning
                PlayersActive++;
                //First checks if they're in the system, if they're not then add.
                //The player will be added so they can be counted for votes
                if(!PlayerVotes.ContainsKey(Player)){
                    PlayerVotes.Add(Player, "");
                    TwitchChatClient.instance.SendChatMessage(Player + " has joined the training sim. " +
                                                                PlayersActive + " degens currently playing, difficulty increased.");
                }
                break;
            case "!PP":
                PPSFX.Play();
                break;
        }

        //If there are bullets then actually shoot
            if(BulletsLeft > 0){
                VotingProcessing(Player, Target);
            }
    }
    
    private void RouteMessage(TwitchChatMessage ChatMessage){
        if(ChatMessage.Message.Length == 1){
            //Reloads
            ReloadMinigameObject.ReloadLetter(ChatMessage.Message);
            //Shoots bullets
            ShootBullet(ChatMessage.Message);
        }
    }
    #endregion
    
    #region Voting Functions
    /**
		This function processes the voting by finding the player and then voting for the target
	**/
    private void VotingProcessing(String Player, String Target){
		//If the player is in the voting system adjust the votes
        if(PlayerVotes.ContainsKey(Player)){
            //Debug.Log(Player + " is in the voting system");
            //If the target is a valid target continue
            if(PartNamesAndVotes.ContainsKey(Target)){
                
                //Debug.Log(Target + " is in the target list");
                //If the player has a vote in already, adjust for that
                if(PlayerVotes[Player] != ""){
                    //Debug.Log(Player + " does not have a blank vote, subtracting vote from " + PlayerVotes[Player]);
                    //Decrement the part that the player is not voting for anymore
                    PartNamesAndVotes[PlayerVotes[Player]]--;
                    //Debug.Log(PlayerVotes[Player] + " is now " + PartNamesAndVotes[PlayerVotes[Player]]);
                    
                    //Update the label to reflect the change
                    //This is really hard to fucking read so let me break it down
                    //The ENEMY is getting accessed through the player part through PartNamesAndEnemy
                    //Then when the ENEMY is accessed, you go in to find the part that needs to be updated through PlayerVotes
                    //FINALLY, adjust the current vote with the PartsNamesAndVotes Dictionary to get the vote
                    PartNamesAndEnemy[PlayerVotes[Player]].UpdateLabel(PlayerVotes[Player], PartNamesAndVotes[PlayerVotes[Player]]);
                }
                //Debug.Log(Player + " has voted for " + Target + " adjusting votes and updating label");
                //Sets the player vote to the new target
                PlayerVotes[Player] = Target;
                //Increments the targets votes
                PartNamesAndVotes[Target]++;

                //Lets everyone know a player targeted a part
                TwitchChatClient.instance.SendChatMessage(Player +  " targeted part " + Target + 
                                                        ". Votes " +  PartNamesAndVotes[Target] + "/" + PartNamesAndEnemyPart[Target].VotesRequiredRounded);
                
                //Makes a ping noise depending on how close they're to the target
                //Hi neko this is past math neko
                /**
                    You want the pitch to be 1-3 so in order to do that you need to get the a percentage of 2 (difference of 1 to 3) then add it to 1
                    This means that 0 (or low to 0) will be 1, then anything else would be inbetween 1-3
                **/

                //Gets the percentage
                float PercentageLeft = (float)PartNamesAndVotes[Target] / (float)PartNamesAndEnemyPart[Target].VotesRequiredRounded;
                //Gets that percentage of 2
                float PercentageOfTwo = 2 * PercentageLeft;
                //Gets the total
                float PitchTotal = 1 + PercentageOfTwo;
                Debug.Log(PercentageLeft + " " + PercentageOfTwo + " " + PitchTotal);
                //Sets the audio pitch
                TargetingPingSFX.pitch = PitchTotal;
                //Debug.Log(TargetingPingSFX.pitch);
                //Plays
                TargetingPingSFX.Play();

                //Compares the votes to see if it's hit the threshold
                //Checks if the votes for the current target is equal to the part's required destruction number
                if(PartNamesAndVotes[Target] == PartNamesAndEnemyPart[Target].VotesRequiredRounded && BulletsLeft > 0){
                    //Destroys the part owie!
                	ShootPart(PartNamesAndEnemy[Target], PartNamesAndEnemyPart[Target]);
                    TwitchChatClient.instance.SendChatMessage(Target +  " last hit by " + Player + 
                                                ". Part successfully destroyed.");
                }
                //Updates the label of the thing
                PartNamesAndEnemy[Target].UpdateLabel(Target, PartNamesAndVotes[Target]);
         
            }
        }
    }
    
    #endregion
    
    #region Shooting Functions
	/**
		This is to reload the gun
	**/
	public void ReloadGun(){
        ReloadSFX.Play();
        if(UnityEngine.Random.Range(0, 100) <= 28){
						Deez.Play();
					} else {
						D.Play();
					}
		BulletsLeft = BulletsMax;
	}
	
	/**
		Destroys the part if it hits the right amount of votes
	**/
	public void ShootPart(Enemy EnemyTargeted, EnemyPart PartToBeDestroyed){
		//JUICE
        //Shakes the camera bang bang pow pow
        CameraController.ShakeCamera(0.69f,1.0f, 0.9f, 0.9f);
        //Timestop
        HitStopper.HitStop(ShootHitStopLength, 1.0f);
        StartCoroutine(FinishTimestop());

        //Gets the enemy targeted and makes it destroy it's limbs
		//This sounds morbid as fuck lmao
		//It's okay i'm hot sheesh 😎 🥵
		EnemyTargeted.DestroyPart(PartToBeDestroyed);
		//Decrease Bullets by 1
    	BulletsLeft--;
        //Updates the text for the bullets remaining
    	UpdateBulletTextCounter();
        GunShotSFX.Play();
        
    	//Checks if the gun is empty
    	if(BulletsLeft <= 0){
            NoBulletsSFX.Play();
            TwitchChatClient.instance.SendChatMessage("Out of bullets! Initiating reload sequence...");
    		//Activates the Reload Minigame
    		ReloadMinigame();
    	}
	}

    public void ShootBullet(String ChatLetter){
        //Creates a temp version of the string but uppercased
        string ChatUpper = ChatLetter.ToUpper();
        
        //Goes through the bullets active list to see if any bullet matches are active
        foreach(Bullet BulletChecker in BulletsActive){
            //Debug.Log("Letter Check: " + BulletChecker.BulletString + " Player Input: " + ChatLetter.ToUpper());
            if(ChatUpper.Equals(BulletChecker.BulletString)){
                //("Shot!");
                BulletsActive.Remove(BulletChecker);
                BulletChecker.FuckingDies();
            }
        }

    }
	
	/**
		Starts the minigame to reload the gun
	**/
	public void ReloadMinigame(){
		ReloadMinigameObject.gameObject.SetActive(true);
	}
	
	public void UpdateBulletTextCounter(){
		BulletCounter.text = "Bullets Left: " + BulletsLeft + "/" + BulletsMax;
	}
    
    #endregion

    #region Health Functions

    /**
        This deals damage tot he player
    **/
    public void DealPlayerDamage(int DamageAmount){
        Health -= DamageAmount;
        HPText.text = "Health: " + Health + "/" + HealthMAX;
        CameraController.ShakeCamera(HealthLossShakeDuration * DamageAmount, HealthLossShakeStrength * DamageAmount, 0.69f, 0.90f);
    }

    #endregion
    
    #region Wave Management Functions
    /**
        This is the wave manager. I'm sorry I'm hard coding it future neko
        It's okay it shouldn't be too fucked 😭😭😭😭
    **/
    private void ManageWave(){
        
        //Switch statement to handle the waves
        switch(CurrentWave){
            //Tutorial Wave
            case 0:
            Debug.Log("Wave 0");
            break;
            
            //Hallway Wave
            case 1:
            Debug.Log("Wave 1");
  
            if(!Wave1Spawned && !AnimationChecker.IsPlayingAnimation()){
                Debug.Log("Spawning Wave 1 enemies");
                EnemiesAvailable[0].gameObject.SetActive(true);
                EnemiesAvailable[1].gameObject.SetActive(true);
                EnemiesAvailable[0].UpdateRequiredVotes();
                EnemiesAvailable[1].UpdateRequiredVotes();
                EnemiesActive.Add(EnemiesAvailable[0]);
                EnemiesActive.Add(EnemiesAvailable[1]);

                Wave1Spawned = true;
            }

            //If the wave is spawned
            if(Wave1Spawned){
                //Checks if the list is empty
                if(!EnemiesActive.Any()){
                    //No enemies are active, then progress wave
                    AdjustCamera();
                }
            }

            break;
            
            //Cafeteria Wave
            case 2:
            Debug.Log("Wave 2");
            
            if(!Wave2Spawned && !AnimationChecker.IsPlayingAnimation()){
                Debug.Log("Spawning Wave 2 enemies");
                EnemiesAvailable[2].gameObject.SetActive(true);
                EnemiesAvailable[3].gameObject.SetActive(true);
                EnemiesAvailable[4].gameObject.SetActive(true);
                EnemiesAvailable[2].UpdateRequiredVotes();
                EnemiesAvailable[3].UpdateRequiredVotes();
                EnemiesAvailable[4].UpdateRequiredVotes();
                EnemiesActive.Add(EnemiesAvailable[2]);
                EnemiesActive.Add(EnemiesAvailable[3]);
                EnemiesActive.Add(EnemiesAvailable[4]);
                Wave2Spawned = true;
            }

            //If the wave is spawned
            if(Wave2Spawned){
                //Checks if the list is empty
                if(!EnemiesActive.Any()){
                    //No enemies are active, then progress wave
                    AdjustCamera();
                }
            }

            break;

            //Operations Wave
            case 3:
            Debug.Log("Wave 3");

            if(!Wave3Spawned && !AnimationChecker.IsPlayingAnimation()){
                Debug.Log("Spawning Wave 3 enemies");
                EnemiesAvailable[5].gameObject.SetActive(true);
                EnemiesAvailable[6].gameObject.SetActive(true);
                EnemiesAvailable[7].gameObject.SetActive(true);
                EnemiesAvailable[8].gameObject.SetActive(true);
                EnemiesAvailable[5].UpdateRequiredVotes();
                EnemiesAvailable[6].UpdateRequiredVotes();
                EnemiesAvailable[7].UpdateRequiredVotes();
                EnemiesAvailable[8].UpdateRequiredVotes();
                EnemiesActive.Add(EnemiesAvailable[5]);
                EnemiesActive.Add(EnemiesAvailable[6]);
                EnemiesActive.Add(EnemiesAvailable[7]);
                EnemiesActive.Add(EnemiesAvailable[8]);
                Wave3Spawned = true;
            }

            //If the wave is spawned
            if(Wave3Spawned){
                //Checks if the list is empty
                if(!EnemiesActive.Any()){
                    //No enemies are active, then progress wave
                    AdjustCamera();
                }
            }

            break;
        }
    }

    /**
        This is so an enemy can remove itself from the list of active enemies
    **/
    public void RemoveSelf(Enemy EnemyToRemove){
        //If the enemy is on the list remove
        if(EnemiesActive.Contains(EnemyToRemove)){
            EnemiesActive.Remove(EnemyToRemove);
        }
    }

    #endregion

    #region Camera functions

    private void AdjustCamera(){
        //Gets the current wave for the integer
        //int CurrentWave = CameraRig.GetInteger("Wave");
        AnimationChecker.SetAnimationPlayingTrue();
        //Increments the wave
        CurrentWave++;
        //Sets the new camera rig to the next wave lol
        CameraRig.SetInteger("Wave", CurrentWave);

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
