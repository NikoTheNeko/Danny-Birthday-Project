using System;
using System.Collections;
using System.Collections.Generic;
using TwitchChatConnect.Example.MiniGame;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour{

    #region Public Variables

        #region Enemy Parts
            [Header("Enemy Parts")]
            [Tooltip("Array of enemy's parts, used to keep track of the enemy parts LMAO")]
            public EnemyPart[] PartsArray;

            public String[] PartsNames;
            [Tooltip("This is the dictionary for the enemy parts, used to keep track of voting")]
            [SerializeField]public Dictionary<String, EnemyPart> PartsDictionary = new Dictionary<String, EnemyPart>();
        #endregion
        
        #region Enemy Game Variables
            [Header("Enemy Game Variables")] 
            [Tooltip("Health of the enemy")]
            public int Health = 1;

            [Tooltip("Enemy Number, this is the number the players will access through the ! commands (ie. !1Head <-- damn no head???)")]
            public String EnemyNumber;

        #endregion

        #region Game Objects
            [Header("Other Game Objects")]
            public GameManager GameManager;
        #endregion

    #endregion

    #region Private Variables
    #endregion

    void Awake(){
        PartsNames = new String[PartsArray.Length];
    }
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    #region Part Checker

    #endregion

    #region Set Up Functions
    
    /**
        Converts the names of the parts to the array
    **/
    public void ArrayToDictionary(){
        //Sets the label starter for the enemy number and command
        String LabelStarter = "!" + EnemyNumber;
        //Loops through the array
        for(int i = 0; i < PartsArray.Length; i++){
            EnemyPart TempPart = PartsArray[i];
            String EnemyLabelName = LabelStarter + TempPart.PartName;
            PartsDictionary.Add(EnemyLabelName, TempPart);
        }
    }

    /**
        Sets up the labels for the enemy
    **/
    public void NameLabels(){
        String LabelStarter = "!" + EnemyNumber;
        
        //Loops through the array
        for(int i = 0; i < PartsArray.Length; i++){
            //Gets the part as a temp
            EnemyPart TempPart = PartsArray[i];
            //Creates the label for the part (![number][partname])
            String EnemyLabelName = LabelStarter + TempPart.PartName;
            //Votes required is calculated by the amount of players active and rounds down. (truncated through the int cast)
            TempPart.VotesRequiredRounded = (int)(TempPart.PercentageOfVotesRequired * GameManager.PlayersActive);
            //Adjusts the text on the label
            TempPart.VotingText.text = EnemyLabelName + "\n" + 0 + "/" + TempPart.VotesRequiredRounded;
            //Puts the labels in an array
            PartsNames[i] = EnemyLabelName;
        }

    }

    /**
        Updates the parts
    **/
    public void UpdateLabel(String TargetName, int VoteCountUpdater){
        //If the part contains the name then update the label
        if(PartsDictionary.ContainsKey(TargetName)){
            Debug.Log("Adjusting label for " + TargetName);
            //Gets the part as a temp
            EnemyPart TempPart = PartsDictionary[TargetName];
            TempPart.VotesRequiredRounded = (int)(TempPart.PercentageOfVotesRequired * GameManager.PlayersActive);
            //Adjusts the text on the label
            TempPart.VotingText.text = TargetName + "\n" + VoteCountUpdater + "/" + TempPart.VotesRequiredRounded;
        }
    }

    #endregion

}
