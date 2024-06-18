using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;
using TwitchChatConnect.Manager;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    #region Public Variables
    [Header("Enemy Related Variables")]
    [Tooltip("The Enemies that are in the game")]
    public Enemy[] EnemiesAvailable;

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
    #endregion

    #region Private Variables
    #endregion

    private void Awake(){
        DontDestroyOnLoad(gameObject);
        }

    // Start is called before the first frame update
    void Start(){
        //Assigns the enemy number to the enemy
        SetUpEnemyNumbers();
        //Sets up Twitch IRC
        SetUpTwitchIRC();
        //Sets up the voting structures
        SetUpVotingStructures();
        
    }

    // Update is called once per frame
    void Update(){
        
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
            //TwitchChatClient.instance.onChatMessageReceived += ShowMessage;
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

    /**
        This routes the players command to the correct place
    **/
    private void RouteCommand(TwitchChatCommand ChatCommandMessage){
        //Breaks down the players names and message, that's all we need for this project
        //If we need more I'm sure we can extract it somehwere
        //Player is the username of the person
        String Player = ChatCommandMessage.User.DisplayName;
        //Target is the player message, aka what they're trying to target in the game
        String Target = ChatCommandMessage.Message;

        Debug.Log(Player + " typed " + Target);

        //Switch statement to reroute the player
        switch(Target){
            //This is for when the players join
            case "!join":
                //Players Active will go up with the player joinning
                PlayersActive++;
                //The player will be added so they can be counted for votes
                PlayerVotes.Add(Player, "");
                break;
        }

        //If the player is in the voting system adjust the votes
        if(PlayerVotes.ContainsKey(Player)){
            Debug.Log(Player + " is in the voting system");
            //If the target is a valid target continue
            if(PartNamesAndVotes.ContainsKey(Target)){
                Debug.Log(Target + " is in the target list");
                //If the player has a vote in already, adjust for that
                if(PlayerVotes[Player] != ""){
                    Debug.Log(Player + " does not have a blank vote, subtracting vote from " + PlayerVotes[Player]);
                    //Decrement the part that the player is not voting for anymore
                    PartNamesAndVotes[PlayerVotes[Player]]--;
                    Debug.Log(PlayerVotes[Player] + " is now " + PartNamesAndVotes[PlayerVotes[Player]]);
                    //Update the label to reflect the change
                    //This is really hard to fucking read so let me break it down
                    //The ENEMY is getting accessed through the player part through PartNamesAndEnemy
                    //Then when the ENEMY is accessed, you go in to find the part that needs to be updated through PlayerVotes
                    //FINALLY, adjust the current vote with the PartsNamesAndVotes Dictionary to get the vote
                    PartNamesAndEnemy[PlayerVotes[Player]].UpdateLabel(PlayerVotes[Player], PartNamesAndVotes[PlayerVotes[Player]]);
                }
                Debug.Log(Player + " has voted for " + Target + " adjusting votes and updating label");
                //Sets the player vote to the new target
                PlayerVotes[Player] = Target;
                //Increments the targets votes
                PartNamesAndVotes[Target]++;
                //Updates the label of the thing
                PartNamesAndEnemy[Target].UpdateLabel(Target, PartNamesAndVotes[Target]);
                VotingProcessing();
            }
        }
    }

    private void VotingProcessing(){

    }
}
