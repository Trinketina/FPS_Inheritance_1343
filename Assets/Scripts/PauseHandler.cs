using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    bool paused = false;
    [SerializeField] PlayerInput playerInput;
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (paused)
            {
                playerInput.enabled = true;
                SceneManager.UnloadSceneAsync("Pause Menu");
                paused = false;
            }
            else
            {
                playerInput.enabled = false;
                SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive);
                paused = true;
            }
        }
        
    }
}
