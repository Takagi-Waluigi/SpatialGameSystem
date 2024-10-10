using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectControl : MonoBehaviour
{
    [SerializeField] PostProcessVolume postProcessVolume;
    ColorGrading colorGrading;
    [SerializeField] StateManager characterStateManager;
    // [SerializeField]
    // [ColorUsage(true, true)] Color color;

    int hue = 0;
    float lastCount = 0;
    float beginExposure = 1f;
    float targetExposure = 1f;
    float exposureValue = 1f;
    float _t = 0f;
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
    }

    // Update is called once per frame
    void Update()
    {
        // if(Time.time - lastTime > interval)
        // {
        //     hue += 40;
        //     lastTime = Time.time;
        // }

        if(characterStateManager.count != lastCount) 
        {
            hue += 40;
            exposureValue = 1.85f;
            beginExposure = 1.0f;
            targetExposure = 3.0f;
        }

        exposureValue -= 0.01f;

        if(exposureValue < 1.0f) exposureValue = 1.0f;
        // else
        // {
        //     exposureValue = 1.0f;
        // }

        // if(exposureValue >= 1.99f) 
        // {
            
        //     beginExposure = 2.0f;
        //     targetExposure = 1.0f;
        //     _t = 0f;
        // }

        // exposureValue = Mathf.Lerp(beginExposure, targetExposure, _t);

        //Debug.Log("exposure value:" + exposureValue);

        _t += 0.05f * Time.deltaTime;

        lastCount = characterStateManager.count;
        
        hue = hue % 360;

        colorGrading.colorFilter.value = Color.HSVToRGB((float)hue / 360f, 76f / 100f, 1f);
        colorGrading.postExposure.value = exposureValue;
    }
}
