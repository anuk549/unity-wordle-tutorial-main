using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
public void LoadVedioScene()
    {
        SceneManager.LoadScene("tracing");
    }
     public void LoadTreeScene()
    {
        SceneManager.LoadScene("1. Intro");
    }
      public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu 3D");
    }
    // public void LoadVideoScene()
    // {
    //     SceneManager.LoadScene("1");
    // }
    // Load Game scene
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    // Load TracingMain scene
    public void LoadTracingMainScene()
    {
        SceneManager.LoadScene("TracingMain");
    }
    public void LoadAiScene()
    {
        SceneManager.LoadScene("ai");
    }
}
