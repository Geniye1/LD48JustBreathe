using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{

    private float amplitudeGain;
    private float frequencyGain;
    private float duration;

    public void BeginShake(float _amplitudeGain, float _frequencyGain, float _duration)
    {

        amplitudeGain = _amplitudeGain;
        frequencyGain = _frequencyGain;
        duration = _duration;

        Shake();
    }

    public void Shake()
    {
        CinemachineVirtualCamera cm = GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin noiseProf = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        StartCoroutine(LerpToValue(noiseProf, amplitudeGain, frequencyGain));
    }

    public IEnumerator LerpToValue(CinemachineBasicMultiChannelPerlin noiseProf, float a, float b)
    {
        if (noiseProf.m_AmplitudeGain < a - 0.1 || (a == 0 && noiseProf.m_AmplitudeGain > a + 0.1))
        {
            noiseProf.m_AmplitudeGain = Mathf.Lerp(noiseProf.m_AmplitudeGain, a, 0.5f);
            noiseProf.m_FrequencyGain = Mathf.Lerp(noiseProf.m_FrequencyGain, b, 0.5f);
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(LerpToValue(noiseProf, a, b));
        }
        else if (a != 0)
        {
            yield return new WaitForSeconds(duration);
            StartCoroutine(LerpToValue(noiseProf, 0, 0));
        }
        else
        {
            noiseProf.m_AmplitudeGain = 0;
            noiseProf.m_FrequencyGain = 0;
        }
    }
}
