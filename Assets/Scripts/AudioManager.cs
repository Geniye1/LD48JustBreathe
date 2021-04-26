using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource travellingAudio;
    public AudioSource explosionAudio;
    public AudioSource hittingObjectSoundEffect;
    public AudioSource explosionSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        TriggerTravellingMusic();
        travellingAudio.Play();
    }

    public void TriggerExplosionMusic()
    {
        travellingAudio.Stop();
        explosionAudio.enabled = true;
        travellingAudio.enabled = false;
        explosionAudio.Play();
    }

    public void TriggerTravellingMusic()
    {
        explosionAudio.Stop();
        travellingAudio.enabled = true;
        explosionAudio.enabled = false;
        travellingAudio.Play();
    }

    public void PlayHit()
    {
        hittingObjectSoundEffect.Play();
    }

    public void PlayExplosion()
    {
        explosionSoundEffect.Play();
    }
}
