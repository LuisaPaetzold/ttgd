using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour {

    public Material skyMat;
    [Range(0, 0.001f)]
    public float stepSize = 0.0002f;

    private void Start()
    {
        if (skyMat != null)
        {
            skyMat.mainTextureOffset = new Vector2(0, 0);
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
	}
}
