
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu: MonoBehaviour 
{
    Scene scene;
    public void StartGame()
    {
    scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex+1);
    }
    public void History()
    {
        scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex + 2);
    }
    public void EndGame()
    {
        Application.Quit();
    }
}
