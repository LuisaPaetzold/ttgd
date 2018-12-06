using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUnityStaticService
{
    float GetInputAxisRaw(string axisName);
    float GetDeltaTime();
}

public class UnityStaticService : IUnityStaticService
{
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    public float GetInputAxisRaw(string axisName)
    {
        switch(axisName)
        {
            case "Horizontal":
                return Input.GetAxisRaw("Horizontal");
            case "Vertical":
                return Input.GetAxisRaw("Vertical");
            default:
                Debug.LogError("String " + axisName + " passed to GetInputAxisRaw was not known!");
                return 0.0f;
        }
    }
}
