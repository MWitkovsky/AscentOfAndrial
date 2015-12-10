using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TextboxHandler : MonoBehaviour
{
    public ThirdPersonCharacter player;
    public AudioClip textSound;
    public Image screenOverlay, textboxBackground, characterPortrait;
    public Text textToPrint;
    public bool startOpen;

    private AudioSource source;
    private float soundDelayTimer;
    private string textQueue;
    private int inputDelay;

    void Start()
    {
        textToPrint = GetComponent<Text>();
        textQueue = textToPrint.text;
        textToPrint.text = "";

        source = GetComponent<AudioSource>();
        source.clip = textSound;
        source.loop = false;

        if (!startOpen)
        {
            screenOverlay.enabled = false;
            textboxBackground.enabled = false;
            characterPortrait.enabled = false;
        }
    }

    void Update()
    {
        if (textQueue != "")
        {
            player.OpenTextbox();
            screenOverlay.enabled = true;
            textboxBackground.enabled = true;
            characterPortrait.enabled = true;
            textToPrint.text += textQueue.Substring(0, 1);
            textQueue = textQueue.Substring(1);

            soundDelayTimer -= Time.deltaTime;
            if(soundDelayTimer <= 0.0f)
            {
                source.Play();
                soundDelayTimer = 0.05f;
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire1") && inputDelay < 0)
        {
            if (textQueue == "")
            {
                player.CloseTextbox();
                screenOverlay.enabled = false;
                textboxBackground.enabled = false;
                characterPortrait.enabled = false;
                textToPrint.text = "";
            }
            else
            {
                textToPrint.text += textQueue;
                textQueue = "";
                source.Play();
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
