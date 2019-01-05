using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PMSkyController
{
    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1;
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            GameObject.Destroy(o);
        }
    }


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
        Assert.AreEqual(skyCtr.stepSize, skyCtr.skyMat.mainTextureOffset.x, "Sky material offset did not increase by step size!");
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerOffsetIsResetToZeroWhenOverOne()
    {
        SkyController skyCtr = CreateSkyController();
        skyCtr.skyMat.mainTextureOffset = new Vector2(1, 0);
        yield return new WaitForEndOfFrame();
        Assert.Zero(skyCtr.skyMat.mainTextureOffset.x, "Sky material was not reset to 0 after exceeding 1!");
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerLightIntensityStartsAtOne()
    {
        SkyController skyCtr = CreateSkyController();
        Assert.AreEqual(1, skyCtr.sceneLight.intensity, "Scene light intensity did not start at 1!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerLightIntensityIsDecreasedFromDayToNight()
    {
        SkyController skyCtr = CreateSkyController();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Assert.NotZero(skyCtr.sceneLight.intensity, "Scene light intensity was not changed at all!");
        Assert.AreEqual(1 - skyCtr.stepSize * skyCtr.lightStepMultiplier, skyCtr.sceneLight.intensity, "Scene light intensity did not decrease by step size times multiplier!");
    }

    [UnityTest]
    public IEnumerator Test_PMSkyControllerLightIntensityIsIncreasedFromNightToDay()
    {
        SkyController skyCtr = CreateSkyController();
        yield return new WaitForEndOfFrame();
        skyCtr.skyMat.mainTextureOffset = new Vector2(0.8f, 0);
        float startValue = skyCtr.sceneLight.intensity;
        yield return new WaitForEndOfFrame();
        Assert.NotZero(skyCtr.sceneLight.intensity, "Scene light intensity was not changed at all!");
        Assert.AreEqual(startValue + skyCtr.stepSize * skyCtr.lightStepMultiplier, skyCtr.sceneLight.intensity, "Scene light intensity did not increase by step size times multiplier!");
    }

    // ------------- helper methods ------------------

    public SkyController CreateSkyController()
    {
        SkyController s = new GameObject().AddComponent<SkyController>();
        s.sceneLight = new GameObject().AddComponent<Light>();
        s.skyMat = new Material(Shader.Find("Unlit/Texture"));
        return s;
    }
}
