using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Cinemachine;

public class BreathingController : MonoBehaviour
{

    public float maxLengthOfBreathe;
    public float minLengthOfBreathe;
    public float maxTimeWithoutBreathe;
    public float minTimeBetweenBreathes;
    public float deepBreatheOffset;

    [Space(10)]
    public float vignetteIntensityFraction;
    public float cameraSizeOffsetFraction;

    [Space(10)]
    public GameObject explosionEffect;

    [Space(10)]
    public GameObject ragdoll;
    public float ragdollTime;

    [Space(10)]
    public PostProcessVolume volume;

    [Space(10)]
    public Color safeBreatheVignetteColor;
    public Color unsafeBreatheVignetteColor;

    [Space(10)]
    public Slider damageSlider;

    [Space(10)]
    public float shakeDuration;
    public float shakeAmplitude;
    public float shakeFrequency;

    [Space(10)]
    private GameManager gm;

    private Vignette _vignette;

    private CinemachineVirtualCamera cam;
    private float cameraDefaultOrthographicSize = 6;

    private GameObject currentExplosion;

    private Animator anim;

    private bool _isBreathing;
    private float timeMultipier;

    private bool hasDeepBreatheAdded;
    private float originalMaxTimeWithoutBreathe;

    private float currentNotBreathingTime;
    private float currentBreatheTime;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out _vignette);

        _vignette.color.value = unsafeBreatheVignetteColor;

        //cam = Camera.main;
        cam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        //cameraDefaultOrthographicSize = cam.m_Lens.OrthographicSize;

        originalMaxTimeWithoutBreathe = maxTimeWithoutBreathe;

        anim = GetComponent<Animator>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.playerState != GameManager.PlayerState.Free)
        {
            if (!_isBreathing)
            {
                currentNotBreathingTime += Time.deltaTime;
                _vignette.intensity.value += (currentBreatheTime / vignetteIntensityFraction) + 0.001f;
                if (currentNotBreathingTime > maxTimeWithoutBreathe)
                {
                    SpawnExplosion();
                    //StartCoroutine(resetVignette(true));
                    currentNotBreathingTime = 0;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(resetVignette(false));

                if (currentNotBreathingTime < minTimeBetweenBreathes)
                {
                    SpawnExplosion();
                    //StartCoroutine(resetVignette(true));
                }
                else
                {
                    _isBreathing = true;
                }

                currentNotBreathingTime = 0;
            }

            if (Input.GetMouseButton(0))
            {
                currentBreatheTime += Time.deltaTime;
                _vignette.intensity.value += (currentBreatheTime / vignetteIntensityFraction) + 0.001f;
                cam.m_Lens.OrthographicSize -= currentBreatheTime / cameraSizeOffsetFraction;

                if (currentBreatheTime < maxLengthOfBreathe && currentBreatheTime > minLengthOfBreathe)
                {
                    _vignette.color.value = Color.Lerp(_vignette.color.value, unsafeBreatheVignetteColor, 0.05f);
                }
                else if (currentBreatheTime > maxLengthOfBreathe)
                {
                    SpawnExplosion();
                    //ResetState();
                    _isBreathing = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (currentBreatheTime > maxLengthOfBreathe - 0.3f && currentBreatheTime < maxLengthOfBreathe)
                {
                    if (!hasDeepBreatheAdded)
                    {
                        hasDeepBreatheAdded = true;
                        maxTimeWithoutBreathe += deepBreatheOffset;
                    }
                }
                else if (hasDeepBreatheAdded)
                {
                    hasDeepBreatheAdded = false;
                    maxTimeWithoutBreathe = originalMaxTimeWithoutBreathe;
                }

                ResetState();
                _vignette.color.value = unsafeBreatheVignetteColor;
                _isBreathing = false;
            }
        }
        else
        {
            _vignette.intensity.value = 0;
        }
    }

    private void SpawnExplosion()
    {
        //ResetState();
        cam.m_Lens.OrthographicSize = 3.5f;
        currentExplosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        StartCoroutine(DestroyExplosion());
        PrepareRagdoll();
        cam.GetComponent<CameraShake>().BeginShake(shakeAmplitude, shakeFrequency, shakeDuration);
        gm.RagdollPlayer(gameObject, ragdoll, ragdollTime, damageSlider);

    }

    public void ResetState()
    {
        StartCoroutine(resetVignette(true));
        StartCoroutine(resetCamera());
        currentBreatheTime = 0;
    }

    public void PrepareRagdoll()
    {
        anim.SetFloat("speed", 0);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
    }

    IEnumerator zoomCamera()
    {
        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 2, 0.1f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(zoomCamera());
    }

    IEnumerator resetVignette(bool isUnsafe)
    {
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.03f);

        if (_vignette.intensity.value > 0.55f)
        {
            StartCoroutine(resetVignette(isUnsafe));
        }
        else
        {
            if (isUnsafe)
            {
                _vignette.color.value = unsafeBreatheVignetteColor;
            }
            else
            {
                _vignette.color.value = safeBreatheVignetteColor;
            }
        }
    }

    IEnumerator resetCamera()
    {
        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, cameraDefaultOrthographicSize, 0.5f);
        yield return new WaitForSeconds(0.01f);

        if (cam.m_Lens.OrthographicSize < 5.9f)
        {
            StartCoroutine(resetCamera());
        }
    }

    IEnumerator DestroyExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(currentExplosion);
    }
}
