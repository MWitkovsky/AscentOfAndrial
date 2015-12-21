using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

    public LoadingScreenHandler loadingScreen;
    public Button startButton;
    public Button levelButton;
    public Button exitButton;
    public GameController gameController;

	// Use this for initialization
	void Start ()
	{
		Button.ButtonClickedEvent startEvent = new Button.ButtonClickedEvent();
		startEvent.AddListener(startGame);
		startButton.onClick = startEvent;

		Button.ButtonClickedEvent levelEvent = new Button.ButtonClickedEvent();
		levelEvent.AddListener(switchToLevelMenu);
		levelButton.onClick = levelEvent;


		Button.ButtonClickedEvent exitEvent = new Button.ButtonClickedEvent();
		exitEvent.AddListener(exitGame);
		exitButton.onClick = exitEvent;

	    gameController.SetCursorMode(CursorLockMode.None);
        gameController.SetIsMenu(true);
	}
	
	//Loads the first level.
	void startGame()
	{
        loadingScreen.loadLevel(Application.loadedLevel + 2);
        //Application.LoadLevel(Application.loadedLevel + 2);
    }

	void switchToLevelMenu()
	{
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	//Ends the application
	//Why the hell do we need this again?
	void exitGame()
	{
		Application.Quit();
	}
}
