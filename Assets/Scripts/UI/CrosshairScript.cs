using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour {

    private Image crosshair;
    private Color starting;
    private Color target;
	// Use this for initialization
	void Start () {
        crosshair = GetComponent<Image>();
        crosshair.color = Color.clear;
        starting = Color.clear;
        target = Color.clear;
	}
	
	// Update is called once per frame
	void Update () {
	    if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            target = Color.white;
           // crosshair.color = Color.white;
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire2"))
        {
            target = Color.clear;
           // crosshair.color = Color.clear;
        }

        crosshair.color = Color.Lerp(crosshair.color, target, Time.deltaTime * 6.0f);
	}
}
