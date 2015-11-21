using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{ 
    public GameObject fillGraphic;

    private Image healthBar;
    private float health;

    //for graphical LERP
    private int lerpCounter;
    private float initHealth;
    private float displayHealth;
    private float targetHealth;
    private bool isHealing, isTakingDamage;

    // Use this for initialization
    void Start()
    {
        healthBar = fillGraphic.GetComponent<Image>();
        //Health is 1.0f because 1.0f = full bar showing, 0.0f = no bar showing
        health = 1.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            applyDamage(20.0f);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            heal(20.0f);
        }

        //Gives a more intense drain when taking damage versus a gentler fill when healing
        if (isTakingDamage)
        {
            displayHealth = Mathf.Lerp(displayHealth, targetHealth, 0.25f);

            if(displayHealth - targetHealth < 0.01f)
            {
                displayHealth = targetHealth;
                isTakingDamage = false;
            }

            updateGraphics();
        }
        else if (isHealing)
        {
            lerpCounter++;
            displayHealth = Mathf.Lerp(initHealth, targetHealth, lerpCounter/100.0f);

            if(lerpCounter == 100)
            {
                displayHealth = targetHealth;
                lerpCounter = 0;
                isHealing = false;
            }

            updateGraphics();
        }
    }

    //Takes two values and takes the resulting amount of damage
    //1.0f of rawDamage == 1% of total life
    public void applyDamage(float damage)
    {
        initHealth = health;
        health -= damage / 100.0f;
        targetHealth = health;

        displayHealth = initHealth;

        if (health < 0.0f)
        {
            health = 0.0f;
            targetHealth = health;
            //die
        }

        isHealing = false;
        lerpCounter = 0;
        isTakingDamage = true;
    }

    public void heal(float amount)
    {
        if(health < 1.00f)
        {
            if (!isHealing)
            {
                initHealth = health;
                health += amount / 100.0f;
                targetHealth = health;

                displayHealth = initHealth;

                isTakingDamage = false;
                lerpCounter = 0;
                isHealing = true;
            }
            else
            {
                //If heal effects gained in quick succession, compound the existing fill
                initHealth = displayHealth;
                health += amount / 100.0f;
                targetHealth = health;

                lerpCounter = 0;
            }
            //Clamp health to 100%
            if (health > 1.00f)
            {
                health = 1.00f;
                targetHealth = 1.00f;
            }
        }        
    }

    private void updateGraphics()
    {
        healthBar.fillAmount = displayHealth;
    }
}
