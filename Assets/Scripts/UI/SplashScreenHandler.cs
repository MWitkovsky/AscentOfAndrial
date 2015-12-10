using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashScreenHandler : MonoBehaviour {

    public ThirdPersonCharacter player;
    public Image background, logo;

    public Color temp;
    public float timer, originalTime;
    private bool halfTime;

    void Start()
    {
        player.openTextbox();
        originalTime = 3.5f;
        timer = originalTime;

        temp = background.color;
        temp.a = 0;
        background.color = temp;

        temp = logo.color;
        temp.a = 0;
        logo.color = temp;
    }

    void Update()
    {
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

                player.closeTextbox();
                Destroy(gameObject);
            }
            else
            {
                temp = logo.color;
                temp.a = 255;
                logo.color = temp;

                halfTime = true;
                timer = originalTime;
            }
        }

        //!halfTime first since its the first half of the intro, easier to keep track of code
        if (!halfTime)
        {
            temp = logo.color;
            temp.a += 255.0f / (originalTime * Time.deltaTime);
            logo.color = temp;
        }
        else
        {
            temp = logo.color;
            temp.a -= 255.0f / (originalTime * Time.deltaTime);
            logo.color = temp;

            temp = background.color;
            temp.a -= 255.0f / (originalTime * Time.deltaTime);
            background.color = temp;
        }
    }
}
