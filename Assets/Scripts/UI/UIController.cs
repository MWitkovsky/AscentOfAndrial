using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Text spellDisplay;
    public RawImage iconFireball;
    public RawImage iconTSpike;
    public RawImage iconSHand;
    
    public Texture fBall_On;
    public Texture fBall_Off;
    public Texture tSpike_On;
    public Texture tSpike_Off;
    public Texture sHand_On;
    public Texture sHand_Off;
	
	// Update is called once per frame
	void Update () {
        //update spell display
        if(player.GetSpell() == ThirdPersonUserControl.Spell.Fireball)
        {
            spellDisplay.text = "Current Spell: Fireball";
            iconFireball.texture = fBall_On;
            iconTSpike.texture = tSpike_Off;
            iconSHand.texture = sHand_Off;
        }
        else if(player.GetSpell() == ThirdPersonUserControl.Spell.GroundSpike)
        {
            spellDisplay.text = "Current Spell: Ground Spike";
            iconFireball.texture = fBall_Off;
            iconTSpike.texture = tSpike_On;
            iconSHand.texture = sHand_Off;
        }
        else
        {
            spellDisplay.text = "Current Spell: Spectral Hand";
            iconFireball.texture = fBall_Off;
            iconTSpike.texture = tSpike_Off;
            iconSHand.texture = sHand_On;
        }
        
	}
}
