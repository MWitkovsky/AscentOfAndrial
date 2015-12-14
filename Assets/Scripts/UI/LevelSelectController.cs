using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectController : MonoBehaviour {
	
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

		for(int i = 1; i <=5; i++)
		{
			Button.ButtonClickedEvent clickEvent = new Button.ButtonClickedEvent();
			clickEvent.AddListener(delegate{goToLevel(1);});
			buttons[i-1].onClick = clickEvent;
		}
	
		Button.ButtonClickedEvent returnEvent = new Button.ButtonClickedEvent();
		returnEvent.AddListener(returnTop);
		buttonReturn.onClick = returnEvent;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	//Loads the selected level
	void goToLevel(int levelOffset)
	{
		Application.LoadLevel(Application.loadedLevel + levelOffset);
	}

	
	//Goes back to the main menu
	void returnTop()
	{
		Application.LoadLevel(Application.loadedLevel - 1);
	}
}

