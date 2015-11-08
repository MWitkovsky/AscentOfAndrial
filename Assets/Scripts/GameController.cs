using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private CursorLockMode currentMode;

	void Start () {
        setCursorMode(CursorLockMode.Locked);
	}
	
	void Update () {
        //If escape is pressed free cursor from game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            setCursorMode(CursorLockMode.None);
        }

        //If cursor is free and game is clicked on, then relock the cursor
        if (currentMode == CursorLockMode.None && Input.GetMouseButtonDown(0)) //0 is left click
        {
            setCursorMode(CursorLockMode.Locked);
        }
	}

    private void setCursorMode(CursorLockMode mode)
    {
        currentMode = mode;
        Cursor.lockState = mode;
        
        if(mode == CursorLockMode.Locked)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}
