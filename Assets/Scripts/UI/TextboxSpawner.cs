using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TextboxSpawner : MonoBehaviour {

    public TextboxHandler textbox;
    public Text textToPrint;
    public Sprite portraitToUse;
    public bool automatic, isFinalBox;

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (automatic)
            {
                textbox.SetTextToPrint(textToPrint.text);
                textbox.SetCharacterPortrait(portraitToUse);
                textbox.setFinalBox(isFinalBox);
                Destroy(this);
            }
        }
    }

	void OnTriggerStay (Collider other) {
        if (other.gameObject.CompareTag("Player"))
        {
            if (CrossPlatformInputManager.GetButtonDown("Fire1") && other.GetComponent<ThirdPersonCharacter>().CanOpenTextbox())
            {
                textbox.SetTextToPrint(textToPrint.text);
                textbox.SetCharacterPortrait(portraitToUse);
            }
        }
	}
}
