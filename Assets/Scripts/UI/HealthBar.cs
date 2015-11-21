using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{ 
    public GameObject fillGraphic;

    private Image healthBar;
    private float health;

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
    }

    //Takes two values and takes the resulting amount of damage
    //1.0f of rawDamage == 1% of total life
    public void applyDamage(float damage)
    {
        health -= damage / 100.0f;
        if (health < 0.0f)
        {
            health = 0.0f;
            //die
        }
        updateGraphics();
    }

    public void heal(float amount)
    {
        health += amount;
        updateGraphics();
    }

    //A lot easier than the voltage meter lol
    private void updateGraphics()
    {
        healthBar.fillAmount = health;
    }
}
