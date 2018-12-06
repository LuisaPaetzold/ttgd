using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PMPlayer {

    [UnityTest]
    public IEnumerator Test_PlayerMovesAlongZAxisForHorizontalInput()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.stats = new PlayerStatsClass();
        player.inventory = new PlayerInventoryClass();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = Substitute.For<IUnityStaticService>();
        staticService.GetDeltaTime().Returns(1.0f);
        staticService.GetInputAxisRaw("Horizontal").Returns(1.0f);
        player.staticService = staticService;

        yield return null;

        Assert.AreEqual(1, player.transform.position.z, 0.1f, "Player didn't move on z axis after horizontal input!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerMovesAlongXAxisForVerticalInput()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.stats = new PlayerStatsClass();
        player.inventory = new PlayerInventoryClass();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = Substitute.For<IUnityStaticService>();
        staticService.GetDeltaTime().Returns(1.0f);
        staticService.GetInputAxisRaw("Vertical").Returns(1.0f);
        player.staticService = staticService;

        yield return null;

        Assert.AreEqual(-1, player.transform.position.x, 0.1f, "Player didn't move on x axis after vertical input!");
    }
}
