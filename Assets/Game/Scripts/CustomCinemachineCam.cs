using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CustomCinemachineCam : MonoBehaviour
{
    public static CustomCinemachineCam I {get; private set;}
    
    [Range(0, 2)] [SerializeField] float followNoiseFreq, crouchNoiseFreq;

    [SerializeField] CinemachineVirtualCamera followCam, crouchCam, aimCam;

    CinemachineBasicMultiChannelPerlin m_followCamNoise, m_aimCamNoise, m_crouchCamNoise;

    Animator animator;
    int animIDSpeed;
    float shakeTimer, totalShakeTimer, startAmp, startfreq;

    void Awake()
    {
        I = this;
        animator = TPSController.I.GetComponent<Animator>();

        m_followCamNoise      = followCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_aimCamNoise         = aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_crouchCamNoise      = crouchCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        animIDSpeed         = Animator.StringToHash("Speed");
    }

    void Update()
    {
        m_followCamNoise.m_FrequencyGain  = Mathf.Max(0.3f, animator.GetFloat(animIDSpeed) * followNoiseFreq);
        m_crouchCamNoise.m_FrequencyGain  = Mathf.Max(0.3f, animator.GetFloat(animIDSpeed) * crouchNoiseFreq);
        // aimCamNoise.m_FrequencyGain     = Mathf.Max(0.3f, animator.GetFloat(animIDSpeed) * aimNoiseFreq);

        // Timer
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                m_aimCamNoise.m_AmplitudeGain = Mathf.Lerp(startAmp, 0.3f, 1-(shakeTimer/totalShakeTimer));
                m_aimCamNoise.m_FrequencyGain = Mathf.Lerp(startfreq, 0f , 1-(shakeTimer/totalShakeTimer));
            }
        }
        // followCam.m_Lens.FieldOfView = m_FollowCamFOV;
    }

    public void CamShake(float amp,float freq, float time)
    {
        // aimCamNoise.m_NoiseProfile = aimCamNoise;
        m_aimCamNoise.m_AmplitudeGain = amp;
        m_aimCamNoise.m_FrequencyGain = freq;
        totalShakeTimer             = time;
        shakeTimer                  = time;
        startfreq                   = freq;
        startAmp                    = amp;
    }
}
