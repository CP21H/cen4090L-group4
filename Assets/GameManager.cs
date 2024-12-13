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

    public void ChangeSceneWithDelay(string sceneName)
    {
        StartCoroutine(SceneChangeCoroutine(sceneName));
    }

    private IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(0.2f); // Adjust this delay as needed
        SceneManager.LoadScene(sceneName);
    }

    public void exitGame() {
        //bool exit = DeckManager.ExitGame();
        Application.Quit();
        Debug.Log("Exiting game...");
    }
}
