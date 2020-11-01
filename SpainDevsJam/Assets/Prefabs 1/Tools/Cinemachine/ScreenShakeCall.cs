using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShakeCall : MonoBehaviour{

    [SerializeField] bool orbitalCamera = false;
    CinemachineFreeLook freeLook;
    CinemachineVirtualCamera vCam;
    float shakeTimer;
    float shakeTimerTotal;
    float startingIntensity;

    public static ScreenShakeCall instance { get; private set; }

    void Awake(){
        instance = this;
        vCam = GetComponent<CinemachineVirtualCamera>();
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    /// <summary> Smoothly shakes with a certain intensity and duration </summary>
    public void ShakeCamera(float intensity, float time){
        startingIntensity = intensity;
        shakeTimer = shakeTimerTotal = time;
    }

    void Update(){
        if (shakeTimer > 0){
            shakeTimer -= Time.deltaTime;
            if (orbitalCamera){
                CinemachineBasicMultiChannelPerlin multiChannelPerlin1 = freeLook.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                CinemachineBasicMultiChannelPerlin multiChannelPerlin2 = freeLook.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                CinemachineBasicMultiChannelPerlin multiChannelPerlin3 = freeLook.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                multiChannelPerlin1.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                multiChannelPerlin2.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                multiChannelPerlin3.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }else{
                CinemachineBasicMultiChannelPerlin multiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                multiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }
}
