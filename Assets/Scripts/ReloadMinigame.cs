using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;
using TwitchChatConnect.Manager;
using UnityEngine;
using TMPro;

public class ReloadMinigame : MonoBehaviour{

    #region Public Variables
    [Header("Game Objects")]
    [Tooltip("This is the text for the Reload prompt")]
    public TMP_Text ReloadTextObject;
    
    [Tooltip("This is the Game Manager")]
    public GameManager GameManagerObject;

	[Header("Reload Sounds")]
	public AudioSource[] ReloadSounds;

    #endregion

    #region Private Variables
    //This is is to traverse the reload number
    private int ReloadTextCounter = 0;
    #endregion
    
	void OnEnable(){
		ResetGame();
	}

    // Start is called before the first frame update
    void Start(){
        UpdateText();
    }

    // Update is called once per frame
    void Update(){
        UpdateText();
		
    }
    
    /**
    	Called on per message
    **/
    public void ReloadLetter(string ChatMessage){
    	Debug.Log("Accessed");
    	//Gets the message, trims it, then uppercases it
		string UpperCasedMessage = ChatMessage.Trim().ToUpper();
	//Checks where we are in the reload string
		switch(UpperCasedMessage){
			case "R":
				if(ReloadTextCounter == 0){
					ReloadSounds[ReloadTextCounter].Play();
					ReloadTextCounter++;
				}
				break;
			case "E":
				if(ReloadTextCounter == 1){
					ReloadSounds[ReloadTextCounter].Play();
					ReloadTextCounter++;
				}
				break;
			case "L":
				if(ReloadTextCounter == 2){
					ReloadSounds[ReloadTextCounter].Play();
					ReloadTextCounter++;
				}
				break;
			case "O":
				if(ReloadTextCounter == 3){
					ReloadSounds[ReloadTextCounter].Play();
					ReloadTextCounter++;
				}
				break;
			case "A":
				if(ReloadTextCounter == 4){
					ReloadSounds[ReloadTextCounter].Play();
					ReloadTextCounter++;
				}
				break;
			case "D":
				if(ReloadTextCounter == 5){
					
					ReloadTextCounter++;
				}
				break;
		}
		//Resets the game
		if(ReloadTextCounter == 6){
			GameManagerObject.ReloadGun();
			GameManagerObject.UpdateBulletTextCounter();
			ResetGame();
			gameObject.SetActive(false);
		}
		
    }
    
    private void UpdateText(){
    	switch (ReloadTextCounter){
    		case 0:
    			ReloadTextObject.text = "<color=white><color=yellow><b>R</b></color>ELOAD</color>";
    		break;
    		case 1:
	    		ReloadTextObject.text = "<color=white><color=green>R</color><color=yellow><b>E</b></color>LOAD</color>";
    		break;
    		case 2:
    			ReloadTextObject.text = "<color=white><color=green>RE</color><color=yellow><b>L</b></color>OAD</color>";
    		break;
    		case 3:
	    		ReloadTextObject.text = "<color=white><color=green>REL</color><color=yellow><b>O</b></color>AD</color>";
    		break;
    		case 4:
    			ReloadTextObject.text = "<color=white><color=green>RELO</color><color=yellow><b>A</b></color>D</color>";
    		break;
    		case 5:
    			ReloadTextObject.text = "<color=white><color=green>RELOA</color><color=yellow><b>D</b></color></color>";
    		break;
    		case 6:
    			ReloadTextObject.text = "<color=green>RELOAD</color>";
    		break;
    	}
    }
    
    private void ResetGame(){
 		ReloadTextCounter = 0;
    }
}
