using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour {

    public Material skyMat;
    [Range(0, 0.001f)]
    public float stepSize = 0.0002f;
    public Light sceneLight;
    public float lightStepMultiplier = 1.4f;

    private void Start()
    {
        if (skyMat != null)
        {
            skyMat.mainTextureOffset = new Vector2(0, 0);
        }
        if (sceneLight != null)
        {
            sceneLight.intensity = 1;
        }
    }

    void Update ()
    {
		if (skyMat != null)
        {
            skyMat.mainTextureOffset += new Vector2(stepSize, 0);
            if (skyMat.mainTextureOffset.x >= 1)
            {
                skyMat.mainTextureOffset = new Vector2(0, 0);
            }
        }
        if (sceneLight != null)
        {
            if (skyMat.mainTextureOffset.x < 0.5f)
            {
                sceneLight.intensity -= stepSize * lightStepMultiplier;
            }
            else
            {
                sceneLight.intensity += stepSize * lightStepMultiplier;
            }
            Mathf.Clamp01(sceneLight.intensity);
        }
	}
}
