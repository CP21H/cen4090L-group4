using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void exitGame() {
        Application.Quit();
        Debug.Log("Exiting game...");
    }
}
