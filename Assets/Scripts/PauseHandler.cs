using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    bool paused = false;
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (paused)
            {
                SceneManager.UnloadSceneAsync("Pause Menu");
                Debug.Log("paused");
                paused = false;
            }
            else
            {
                Debug.Log("unpaused");
                SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive);
                paused = true;
            }
        }
        
    }
}
