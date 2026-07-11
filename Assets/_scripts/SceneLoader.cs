using UnityEngine;
using UnityEngine.SceneManagement;

// Scene changing for menu / level / game-over buttons. Every load restores timeScale to 1
// first, so a scene left paused doesn't come up frozen. Wire the public methods to a UI
// Button's OnClick (name and index variants both take a typed argument in the inspector),
// or call them from code.
//
// NOTE: any scene loaded by name or index must be added to Build Settings > Scenes In Build.
public class SceneLoader : MonoBehaviour
{
    // Load a scene by its exact name.
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // Load a scene by its Build Settings index.
    public void LoadSceneByIndex(int buildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    // Restart the current scene (e.g. retry after game over).
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Advance to the next scene in Build Settings, wrapping back to the first.
    public void LoadNextScene()
    {
        Time.timeScale = 1f;
        int next = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(next);
    }

    // Exit the game (stops play mode in the editor).
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
