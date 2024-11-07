using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectControl : MonoBehaviour
{
    [SerializeField] PostProcessVolume postProcessVolume;
    ColorGrading colorGrading;
    Bloom bloom;
    [SerializeField] StateManager gameStateManager;
    [SerializeField] float baseIntensityValue = 20f;
    [SerializeField] float maxIntensityValue = 100f;
    [SerializeField] float gameOverEffectSpeed = 0.25f;
    [SerializeField] int baseHue = 225;

    int hue;
    float lastCount = 0;
    bool lastIsAttacked = false;
    float exposureValue = 1f;
    float gameOverIntensity = 20f;
   
    float _t = 0f;
    // Start is called before the first frame update
    void Start()
    {
        hue = baseHue;
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        postProcessVolume.profile.TryGetSettings<Bloom>(out bloom);
    }

    // Update is called once per frame
    void Update()
    {   
        //ColorGrading周り
        {
            if(gameStateManager.score != lastCount) 
            {
                hue -= 10;
                exposureValue = 1.85f;
            }

            exposureValue -= 0.01f;
            if(exposureValue < 1.0f) exposureValue = 1.0f;

            lastCount = gameStateManager.score;
            
            hue = hue % 360;

            colorGrading.colorFilter.value = Color.HSVToRGB((float)hue / 360f, 87f / 100f, 1f);
            colorGrading.postExposure.value = exposureValue;
        }
        
        //Bloom周り
        {
            if(lastIsAttacked != gameStateManager.isAttacked) _t = Time.time;

            float theta = (Time.time - _t) * gameOverEffectSpeed;

            if(gameStateManager.isAttacked && theta < Mathf.PI)
            {  
                hue = baseHue;
                gameOverIntensity = maxIntensityValue * Mathf.Sin(theta); 
            }

            bloom.intensity.value = baseIntensityValue + gameOverIntensity;

            lastIsAttacked = gameStateManager.isAttacked;
        }
       
    }
}
