using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PMSkyController
{
    [UnityTest]
    public IEnumerator Test_PMSkyControllerOffsetStartsAtZero()
    {
        SkyController skyCtr = CreateSkyController();
        Assert.Zero(skyCtr.skyMat.mainTextureOffset.x, "Sky material offset did not start at 0!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerOffsetIsIncreasedByStepSize()
    {
        SkyController skyCtr = CreateSkyController();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Assert.NotZero(skyCtr.skyMat.mainTextureOffset.x, "Sky material offset was not changed at all!");
        Assert.AreEqual(skyCtr.skyMat.mainTextureOffset.x, skyCtr.stepSize, "Sky material offset did not increase by step size!");
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerOffsetIsResetToZeroWhenOverOne()
    {
        SkyController skyCtr = CreateSkyController();
        skyCtr.skyMat.mainTextureOffset = new Vector2(1, 0);
        yield return new WaitForEndOfFrame();
        Assert.Zero(skyCtr.skyMat.mainTextureOffset.x, "Sky material was not reset to 0 after exceeding 1!");
    }

    // ------------- helper methods ------------------

    public SkyController CreateSkyController()
    {
        SkyController s = new GameObject().AddComponent<SkyController>();
        s.skyMat = new Material(Shader.Find("Unlit/Texture"));
        return s;
    }
}
