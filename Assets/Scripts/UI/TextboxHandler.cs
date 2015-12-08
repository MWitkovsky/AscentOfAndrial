using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TextboxHandler : MonoBehaviour
{
    public ThirdPersonCharacter player;
    public Image background, characterPortrait;
    public Text textToPrint;

    private string textQueue;
    private int inputDelay;

    void Start()
    {
        textToPrint = GetComponent<Text>();
        textQueue = textToPrint.text;
        textToPrint.text = "";
    }

    void Update()
    {
        if (textQueue != "")
        {
            player.openTextbox();
            background.enabled = true;
            characterPortrait.enabled = true;
            textToPrint.text += textQueue.Substring(0, 1);
            textQueue = textQueue.Substring(1);
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && inputDelay < 0)
        {
            if (textQueue == "")
            {
                player.closeTextbox();
                background.enabled = false;
                characterPortrait.enabled = false;
                textToPrint.text = "";
            }
            else
            {
                textToPrint.text += textQueue;
                textQueue = "";
            }
        }
        else
        {
            inputDelay--;
        }
    }

    public void SetTextToPrint(string textToPrint)
    {
        this.textToPrint.text = "";
        textQueue = textToPrint;
        inputDelay = 10;
    }

    public void SetCharacterPortrait(Sprite characterPortrait)
    {
        this.characterPortrait.sprite = characterPortrait;
    }
}
