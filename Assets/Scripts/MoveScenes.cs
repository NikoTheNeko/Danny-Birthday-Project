using System.Collections;
using System.Collections.Generic;
using ExternalPropertyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScenes : MonoBehaviour{
    public string SceneName;

    public void TransitionToScene(){
        SceneManager.LoadScene(SceneName);
    }

}
