using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

/**
Have you heard of the critically acclaimed MMORPG Final Fantasy XIV?
With an expanded free trial which you can play through the entirety of
A Realm Reborn and the award-winning Heavensward,
and thrilling Stormblood expansions
up to level 70 for free with no restrictions on playtime.
**/

public enum PartFunction {Speed, Shoot, Melee, Brain};

public class EnemyPart : MonoBehaviour{

	[Tooltip("If the part is active or not")]
	public bool PartActive = true;
	
    //The Name of the Part
    [Tooltip("The Name of the Part")]
    public String PartName;

    //The Damage it'll do when broken
    [Tooltip("The Name of the Part")]
    [Range(1, 10)]
    public int Damage;

    //Vote Percentage required rememebr values need to be less than 1 unless 100% is necessary
    [Tooltip("The Name of the Part")]
    [Range(0.1f, 1.0f)]
    public double PercentageOfVotesRequired;

    [Tooltip("Amount of players needed for the votes")]
    public int VotesRequiredRounded;

    [Tooltip("Text to display the votes needed")]
	public TMP_Text VotingText;
	
	[Tooltip("This is the part function ie speed boost / if it's able to shoot / melee / etc etc")]
	public PartFunction FunctionType;
	
	[Tooltip("This is the modifier for the part, could be variable so just keep track ig im lazy lol")]
	public float FunctionModifier;
}
