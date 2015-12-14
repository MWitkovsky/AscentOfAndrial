using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private CursorLockMode currentMode;
    private bool isMenu;

	void Start () {
        SetCursorMode(CursorLockMode.Locked);
	}
	
	void Update () {
        //If escape is pressed free cursor from game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorMode(CursorLockMode.None);
        }

        //If cursor is free and game is clicked on, then relock the cursor
        if (currentMode == CursorLockMode.None && Input.GetMouseButtonDown(0) && !isMenu) //0 is left click
        {
            SetCursorMode(CursorLockMode.Locked);
        }
        
        if(Input.GetKey(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if(Input.GetKey(KeyCode.F2))
		{
			Application.LoadLevel(0);
		}

        if(isMenu && currentMode != CursorLockMode.None)
        {
            SetCursorMode(CursorLockMode.None);
        }
	}

    public void SetCursorMode(CursorLockMode mode)
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

	public CursorLockMode GetCursorMode() {return currentMode;}

    public void SetIsMenu(bool isMenu)
    {
        this.isMenu = isMenu;
    }
}
