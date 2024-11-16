using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
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
}
