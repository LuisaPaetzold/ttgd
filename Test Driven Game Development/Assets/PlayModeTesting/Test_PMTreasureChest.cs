using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor.Animations;
using NSubstitute;

public class Test_PMTreasureChest
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
    public IEnumerator Test_ChestOpensIfPlayerCloseEnough()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest();
        chest.player = player;
        chest.transform.position = new Vector3(chest.openDistance + 1, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(chest.isOpen, "Treasure chest started open instead of closed!");

        player.transform.position = new Vector3(chest.openDistance, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(chest.isOpen, "Treasure chest did not open when player approached!");

    }

    [UnityTest]
    public IEnumerator Test_ChestPlaysAnimationAfterOpening()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest(true);
        chest.player = player;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(chest.animator, "There was no animator added to the chest!");
        Assert.IsNotNull(chest.animator.runtimeAnimatorController, "There was no animator controller added to the chest's animator!");

        chest.transform.position = new Vector3(chest.openDistance - 1, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(chest.animator.IsInTransition(0), "Animator was not in transition to next animation!");
        
        yield return new WaitForSeconds(chest.animator.GetAnimatorTransitionInfo(0).duration);

        Assert.IsTrue(chest.animator.GetCurrentAnimatorStateInfo(0).IsName("Open"), "Current animation did not have a matching name!");
        Assert.IsTrue(chest.animator.GetCurrentAnimatorStateInfo(0).IsTag("Open"), "Current animation did not have a matching tag!");

        // Here we can also test animation speed, playback time, and more. WRITE
    }

    [UnityTest]
    public IEnumerator Test_ChestOpenAnimationIsPlayingAndNotLooping()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest(true);
        chest.player = player;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(chest.animator, "There was no animator added to the chest!");
        Assert.IsNotNull(chest.animator.runtimeAnimatorController, "There was no animator controller added to the chest's animator!");

        chest.transform.position = new Vector3(chest.openDistance - 1, 0, 0);

        yield return new WaitForSeconds(chest.animator.GetAnimatorTransitionInfo(0).duration);

        Assert.IsFalse(chest.animator.GetCurrentAnimatorStateInfo(0).loop, "Open animation was looping but shouldn't!");
        Assert.AreEqual(1, chest.animator.GetCurrentAnimatorStateInfo(0).speed, "Open animation speed was not 1!");
    }

    [UnityTest]
    public IEnumerator Test_LightIncreasesIfChestOpened()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest();
        IUnityStaticService staticService = CreateUnityService(0.1f, 0, 0);
        chest.player = player;
        chest.staticService = staticService;
        player.transform.position = new Vector3(chest.openDistance, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.Zero(chest.flareLight.intensity, "Flare light intensity was not 0 on start!");

        chest.isOpen = true;

        yield return new WaitForEndOfFrame();

        Assert.NotZero(chest.flareLight.intensity, "Flare light intensity was not increased!");

        chest.flareLight.intensity = chest.maxLightIntensity;

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(chest.maxLightIntensity, chest.flareLight.intensity, "Flare light intensity was increased beyond max intensity!");
    }

    [UnityTest]
    public IEnumerator Test_InvokesGameEndAfterLightIncreasedToMax()
    {
        Player player = CreatePlayer();
        TreasureChest chest = CreateChest();
        GameController gameCtr = CreateGameController(player);
        chest.gameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        chest.flareLight.intensity = chest.maxLightIntensity;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.gameEnded, "Game end was not invoked after light increased to max!");
    }






    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer(bool setUpComponentsInTest = true)
    {
        Player p = new GameObject().AddComponent<Player>();
        if (setUpComponentsInTest)
        {
            p.stats = new PlayerStatsClass();
            p.inventory = new PlayerInventoryClass();
        }
        return p;
    }

    public TreasureChest CreateChest(bool addAnimator = false)
    {
        TreasureChest t = new GameObject().AddComponent<TreasureChest>();
        Light flareLight = new GameObject().AddComponent<Light>();
        flareLight.transform.SetParent(t.transform);

        if (addAnimator)
        {
            Animator anim = t.gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AssetStore/Low Poly Toon Chests/Chest.controller");
        }

        return t;
    }

    IUnityStaticService CreateUnityService(float deltaTimeReturn, float horizontalReturn, float verticalReturn)
    {
        IUnityStaticService s = Substitute.For<IUnityStaticService>();
        s.GetDeltaTime().Returns(deltaTimeReturn);
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }

    public GameController CreateGameController(Player p)
    {
        GameController g = new GameObject().AddComponent<GameController>();
        g.player = p;
        return g;
    }
}
