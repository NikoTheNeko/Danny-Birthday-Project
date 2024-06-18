using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class EnemyPart : MonoBehaviour{

    //The Name of the Part
    [Tooltip("The Name of the Part")]
    public String PartName;

    //The Damage it'll do when broken
    [Tooltip("The Name of the Part")]
    public int Damage;

    //Vote Percentage required rememebr values need to be less than 1 unless 100% is necessary
    [Tooltip("The Name of the Part")]
    [Range(0.1f, 1.0f)]
    public double PercentageOfVotesRequired;

    [Tooltip("Amount of players needed for the votes")]
    public int VotesRequiredRounded;

    [Tooltip("Text to display the votes needed")]
    public Text VotingText;
}
