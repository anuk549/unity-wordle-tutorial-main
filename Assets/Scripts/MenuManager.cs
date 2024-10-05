using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
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
