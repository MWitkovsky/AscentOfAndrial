using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenHandler : MonoBehaviour {

    public Image background;
    public Text loadingText;
    public Image andrialSprite;
    public Sprite[] sprites;

    private int loadProgress;
    private int spriteIndex;
    private float frameDelay;
    private float timer;

	void Start () {
        background.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
        andrialSprite.gameObject.SetActive(false);

        loadProgress = 0;
        spriteIndex = 0;
        frameDelay = 1.0f / 6.0f;
        timer = frameDelay;
    }
	
	void Update () {
        /*if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(DisplayLoadingScreen(Application.loadedLevel));
        }*/

        if (andrialSprite.IsActive())
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                spriteIndex++;
                if (!(spriteIndex < sprites.Length))
                {
                    spriteIndex = 0;
                }
                andrialSprite.sprite = sprites[spriteIndex];

                timer = frameDelay;
            }
        }
    }

    IEnumerator DisplayLoadingScreen(int level)
    {
        background.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);
        andrialSprite.gameObject.SetActive(true);

        loadingText.text = "Loading Progress: " + loadProgress + "%";

        AsyncOperation async = Application.LoadLevelAsync(level);
        while (!async.isDone)
        {
            loadProgress = (int)(async.progress * 100);
            loadingText.text = "Loading Progress: " + loadProgress + "%";

            yield return null;
        }
    }

    public void loadLevel(int levelToLoad)
    {
        StartCoroutine(DisplayLoadingScreen(levelToLoad));
    }
}
