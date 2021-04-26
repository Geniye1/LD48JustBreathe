using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomExplosion : MonoBehaviour
{
    
    public Light explosionLight;
    public float maxIntensity;

    public float minExplosionTimeRange;
    public float maxExplosionTimeRange;

    public AudioSource explosionAudioSource;
    public AudioSource randomYellAudioSource;

    public AudioClip[] yellClips;

    private float timer;
    private float timeToExplosion;

    private bool isYellPlaying = false;

    void Awake()
    {
        explosionLight.intensity = 0;
        timeToExplosion = Random.Range(minExplosionTimeRange, maxExplosionTimeRange);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeToExplosion - 0.4f && !isYellPlaying)
        {
            if (randomYellAudioSource.isPlaying) { randomYellAudioSource.Stop(); }
            randomYellAudioSource.clip = yellClips[Random.Range(0, yellClips.Length)];
            randomYellAudioSource.Play();
            isYellPlaying = true;
        }

        if (timer > timeToExplosion)
        {
            if (explosionAudioSource.isPlaying) { explosionAudioSource.Stop(); }
            explosionAudioSource.Play();
            StartCoroutine(SpawnExplosion());
            timer = 0;

            timeToExplosion = Random.Range(minExplosionTimeRange, maxExplosionTimeRange);

            isYellPlaying = false;
        }
    }

    private IEnumerator SpawnExplosion()
    {
        explosionLight.intensity = Mathf.Lerp(explosionLight.intensity, maxIntensity, 0.4f);
        yield return new WaitForSeconds(0.01f);

        if (explosionLight.intensity < maxIntensity - 0.1f)
        {
            StartCoroutine(SpawnExplosion());
        }
        else
        {
            StartCoroutine(FadeOutLight());
        }
    }

    private IEnumerator FadeOutLight()
    {
        explosionLight.intensity = Mathf.Lerp(explosionLight.intensity, 0, 0.4f);
        yield return new WaitForSeconds(0.01f);

        if (explosionLight.intensity > 0.1f)
        {
            StartCoroutine(FadeOutLight());
        }
    }

}
