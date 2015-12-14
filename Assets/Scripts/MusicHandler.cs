using UnityEngine;
using System.Collections;

public class MusicHandler : MonoBehaviour {

	public AudioClip songIntro, mainSongLoop, victoryTheme;

	public AudioSource source1, source2; //two sources used for gapless playback

    [Range(0.0f, 1.0f)]public float volume;

	//Starts playing the song intro and queues the main loop to play right after
	void Start () {
        volume *= volume;
        source1.volume = volume;
        source2.volume = volume;

        if (songIntro)
        {
            source1.loop = false;
            source1.clip = songIntro;
            source1.Play();

            source2.loop = true;
            source2.clip = mainSongLoop;
            source2.PlayDelayed(songIntro.length);
        }
        else
        {
            source1.loop = true;
            source1.clip = mainSongLoop;
            source1.Play();
        }
	}

	//Pauses song loop and plays the one-up jingle
	/*public void PlayOneUpJingle(){
		source1.clip = oneUpJingle;
		source2.Pause ();
		source1.Play ();
		source2.PlayDelayed (oneUpJingle.length);
	}*/

    public void ChangeVolume(float volume)
    {
        volume *= volume;
        this.volume = volume;
        source1.volume = volume;
        source2.volume = volume;
    }
    
	//Stops all currently playing music
	public void PauseAll(){
		source1.Pause ();
		source2.Pause ();
	}

    public void PlayVictoryTheme()
    {
        PauseAll();
        source1.loop = false;
        source1.clip = victoryTheme;
        source1.Play();
    }
}
