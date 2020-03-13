using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool pauseMenuActive;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (pauseMenuActive == false)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                pauseMenuActive = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
                pauseMenuActive = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
