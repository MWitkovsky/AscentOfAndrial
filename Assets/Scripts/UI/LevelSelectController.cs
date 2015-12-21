using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectController : MonoBehaviour {

    public LoadingScreenHandler loadingScreen;
	public Button buttonMV;
	public Button buttonIC;
	public Button buttonAF;
	public Button buttonCP;
	public Button buttonBC;
	public Button buttonReturn;
	public GameController gameController;

	// Use this for initialization
	void Start ()
	{
		Button[] buttons = {buttonMV, buttonIC, buttonAF, buttonCP, buttonBC};

		/*for(int i = 1; i <= buttons.Length; i++)
		{
			Button.ButtonClickedEvent clickEvent = new Button.ButtonClickedEvent();
			clickEvent.AddListener(delegate{goToLevel(i-1);});
			buttons[i-1].onClick = clickEvent;
		}*/

        Button.ButtonClickedEvent clickEvent = new Button.ButtonClickedEvent();
        clickEvent.AddListener(delegate { goToLevel(1); });
        buttonMV.onClick = clickEvent;

        clickEvent = new Button.ButtonClickedEvent();
        clickEvent.AddListener(delegate { goToLevel(2); });
        buttonIC.onClick = clickEvent;

        clickEvent = new Button.ButtonClickedEvent();
        clickEvent.AddListener(delegate { goToLevel(3); });
        buttonAF.onClick = clickEvent;

        clickEvent = new Button.ButtonClickedEvent();
        clickEvent.AddListener(delegate { goToLevel(4); });
        buttonCP.onClick = clickEvent;

        clickEvent = new Button.ButtonClickedEvent();
        clickEvent.AddListener(delegate { goToLevel(5); });
        buttonBC.onClick = clickEvent;

        Button.ButtonClickedEvent returnEvent = new Button.ButtonClickedEvent();
		returnEvent.AddListener(returnTop);
		buttonReturn.onClick = returnEvent;

        gameController.SetCursorMode(CursorLockMode.None);
        gameController.SetIsMenu(true);
	}
	
	//Loads the selected level
	void goToLevel(int levelOffset)
	{
		loadingScreen.loadLevel(Application.loadedLevel + levelOffset);
	}

	
	//Goes back to the main menu
	void returnTop()
	{
		Application.LoadLevel(Application.loadedLevel - 1);
	}
}

