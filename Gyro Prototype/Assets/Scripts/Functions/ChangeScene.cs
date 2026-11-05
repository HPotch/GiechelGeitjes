using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SwitchScene("GameStart");
        }
    }
}
