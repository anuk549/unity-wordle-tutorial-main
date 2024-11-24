using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
     public void LoadTreeScene()
    {
        SceneManager.LoadScene("1. Intro");
    }
      public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu 3D");
    }
    public void LoadVideoScene()
    {
        SceneManager.LoadScene("1");
    }
    // Load Game scene
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    // Load Wordle scene
    public void LoadWordleScene()
    {
        SceneManager.LoadScene("Wordle");
    }
    public void LoadAiScene()
    {
        SceneManager.LoadScene("ai");
    }
}
