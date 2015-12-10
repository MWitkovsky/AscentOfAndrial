using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public ThirdPersonCharacter player;
    public RawImage iconFireball;
    public RawImage iconTSpike;
    public RawImage iconSHand;

    //Timer sprites
    public Image[] timerSlots = new Image[6];
    public Sprite[] numbers = new Sprite[10];

    //Spell icon sprites
    public Texture fBall_On;
    public Texture fBall_Off;
    public Texture tSpike_On;
    public Texture tSpike_Off;
    public Texture sHand_On;
    public Texture sHand_Off;

    //For the timer
    private float timer;
    private bool gameActive;
	
    void Start()
    {
        gameActive = true;
    }

	void Update () {
        //update timer while game is active
        //currently it is just a string, but we can replace the numbers with fancy graphics later by parsing it
        if (gameActive)
        {
            if (!player.IsTextboxOpen() && !player.IsFrozen())
            {
                timer += Time.deltaTime;
                DisplayTime(timer);
            }
        }

        //update spell display
        if(player.GetSpell() == ThirdPersonUserControl.Spell.Fireball)
        {
            iconFireball.texture = fBall_On;
            iconTSpike.texture = tSpike_Off;
            iconSHand.texture = sHand_Off;
        }
        else if(player.GetSpell() == ThirdPersonUserControl.Spell.GroundSpike)
        {
            iconFireball.texture = fBall_Off;
            iconTSpike.texture = tSpike_On;
            iconSHand.texture = sHand_Off;
        }
        else
        {
            iconFireball.texture = fBall_Off;
            iconTSpike.texture = tSpike_Off;
            iconSHand.texture = sHand_On;
        }
        
	}

    
    //Converts time to 00:00.00 format
    private void DisplayTime(float time)
    {
        //This check is just for sanity, otherwise a -1 will appear after the display after 0.0
        //if (time <= 0.0f)
        //{
            //timeDisplay.text = "00:00.00";
        //}

        //strings for handling leading zeroes if needed
        string minutes = "";
        string seconds = "";
        string milliseconds = "";

        int minutesI = (int)time / 60;
        time %= 60;
        int secondsI = (int)time;
        time %= 1.0f;
        int millisecondsI = (int)(time * 100);

        //adding on leading zeroes if needed
        if (minutesI < 10)
        {
            minutes = "0" + minutesI.ToString();
        }
        else
        {
            minutes = minutesI.ToString();
        }

        if (secondsI < 10)
        {
            seconds = "0" + secondsI.ToString();
        }
        else
        {
            seconds = secondsI.ToString();
        }

        if (millisecondsI < 10)
        {
            milliseconds = "0" + millisecondsI.ToString();
        }
        else
        {
            milliseconds = millisecondsI.ToString();
        }

        //timeDisplay.text = (minutes + ":" + seconds + "." + milliseconds);

        setTimerImage(timerSlots[0], minutes.Substring(0, 1));
        setTimerImage(timerSlots[1], minutes.Substring(1, 1));
        setTimerImage(timerSlots[2], seconds.Substring(0, 1));
        setTimerImage(timerSlots[3], seconds.Substring(1, 1));
        setTimerImage(timerSlots[4], milliseconds.Substring(0, 1));
        setTimerImage(timerSlots[5], milliseconds.Substring(1, 1));
    }

    private void setTimerImage(Image slot, string s)
    {
        int num;
        int.TryParse(s, out num);
        slot.sprite = numbers[num];
    }

    //setting the game active/inactive due to level completion or death
    public void setGameActive(bool gameActive)
    {
        this.gameActive = gameActive;
    }
}
