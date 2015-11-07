using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Text spellDisplay;
	
	// Update is called once per frame
	void Update () {
        //update spell display
        if(player.GetSpell() == ThirdPersonUserControl.Spell.Flamethrower)
        {
            spellDisplay.text = "Current Spell: Flamethrower";
        }
        else if(player.GetSpell() == ThirdPersonUserControl.Spell.GroundSpike)
        {
            spellDisplay.text = "Current Spell: Ground Spike";
        }
        else
        {
            spellDisplay.text = "Current Spell: Spectral Hand";
        }
        
	}
}
