using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUnityStaticService
{
    float GetInputAxis(string axisName);
    float GetDeltaTime();
}

public class UnityStaticService : IUnityStaticService
{
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    public float GetInputAxis(string axisName)
    {
        switch(axisName)
        {
            case "Horizontal":
                return Input.GetAxis("Horizontal");
            case "Vertical":
                return Input.GetAxis("Vertical");
            default:
                Debug.LogError("String " + axisName + " passed to GetInputAxisRaw was not known!");
                return 0.0f;
        }
    }
}
