using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class SplashScreenHandler : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Image background, logo;
    public float introLength;

    private Color temp;
    private float timer, originalTime;
    private bool halfTime;

    void Start()
    {
        player.SplashScreenFreeze();
        originalTime = introLength/2.0f;
        timer = originalTime;

        temp = background.color;
        temp.a = 1.0f;
        background.color = temp;

        temp = logo.color;
        temp.a = 0;
        logo.color = temp;
    }

    void Update()
    {
        //Lets player skip to the fade-out
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            if (!halfTime)
            {
                temp = logo.color;
                temp.a = 1;
                logo.color = temp;

                halfTime = true;
                timer = originalTime;
            }
        }

        timer -= Time.deltaTime;
        if(timer < 0.0f)
        {
            if (halfTime)
            {
                temp = logo.color;
                temp.a = 0;
                logo.color = temp;

                temp = background.color;
                temp.a = 0;
                background.color = temp;

                player.EndSplashScreen();
                Destroy(gameObject);
            }
            else
            {
                temp = logo.color;
                temp.a = 1;
                logo.color = temp;

                halfTime = true;
                timer = originalTime;
            }
        }

        //!halfTime first since its the first half of the intro, easier to keep track of code
        if (!halfTime)
        {
            temp = logo.color;
            temp.a += 1.0f / (originalTime / Time.deltaTime);
            logo.color = temp;
        }
        else
        {
            temp = logo.color;
            temp.a -= 1.0f / (originalTime / Time.deltaTime);
            logo.color = temp;

            temp = background.color;
            temp.a -= 1.0f / (originalTime / Time.deltaTime);
            background.color = temp;
        }
    }
}
