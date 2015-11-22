using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{ 
    public GameObject fillGraphic;
    public ThirdPersonCharacter player;

    private Image healthBar;
    private int health;

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
        health = 100;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            applyDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            heal(20);
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
    public void applyDamage(int damage)
    {
        initHealth = health / 100.0f;
        health -= damage;
        targetHealth = health / 100.0f; ;

        displayHealth = initHealth;

        if (health <= 0)
        {
            health = 0;
            targetHealth = 0.00f;

            player.Kill();
        }

        isHealing = false;
        lerpCounter = 0;
        isTakingDamage = true;
    }

    public void heal(int amount)
    {
        if(health < 100)
        {
            if (!isHealing)
            {
                initHealth = health / 100.0f; ;
                health += amount;
                targetHealth = health / 100.0f; ;

                displayHealth = initHealth;
            }
            else
            {
                //If heal effects gained in quick succession, compound the existing fill
                initHealth = displayHealth;
                health += amount;
                targetHealth = health / 100.0f; ;

                lerpCounter = 0;
            }
            //Clamp health to 100%
            if (health > 100)
            {
                health = 100;
                targetHealth = 1.00f;
            }

            isTakingDamage = false;
            lerpCounter = 0;
            isHealing = true;
        }        
    }

    private void updateGraphics()
    {
        healthBar.fillAmount = displayHealth;
    }
}
