using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PMShipIntro
{
    [TearDown]
    public void TearDown()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            GameObject.Destroy(o);
        }
    }

    [UnityTest]
    public IEnumerator Test_ShipSignalizesGameControllerWhenIntroEnded()
    {
        ShipIntro ship = CreateShip();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        ship.gameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.introPlaying, "Game controller did not know that intro is playing on game start!");

        ship.animFinished = true;

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.introPlaying, "Game controller wasn't informed that intro was finished!");
    }

    [UnityTest]
    public IEnumerator Test_ShipForcesPlayerAsChild()
    {
        ShipIntro ship = CreateShip();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        ship.gameCtr = gameCtr;
        Transform oldParent = player.transform.parent;

        yield return new WaitForEndOfFrame();
        
        Assert.AreEqual(ship.transform, player.transform.parent, "Ship did not force player to be its child!");

        ship.animFinished = true;

        yield return new WaitForEndOfFrame();

        Assert.AreNotEqual(ship.transform, player.transform.parent, "Ship did not release player as child!");
        Assert.AreEqual(oldParent, player.transform.parent, "Ship give player back to old parent!");
    }

    [UnityTest]
    public IEnumerator Test_ShipDisablesPlayerComponentsInIntro()
    {
        ShipIntro ship = CreateShip();
        Player player = CreatePlayer();
        BoxCollider coll = player.gameObject.AddComponent<BoxCollider>();
        CharacterController charContr = player.gameObject.AddComponent<CharacterController>();
        GameController gameCtr = CreateGameController(player);
        ship.gameCtr = gameCtr;

        yield return new WaitForEndOfFrame();
        
        Assert.IsFalse(coll.enabled, "Box collider was not disabled!");
        Assert.IsFalse(charContr.enabled, "Character controller was not disabled!");

        ship.animFinished = true;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(coll.enabled, "Box collider was not enabled again!");
        Assert.IsTrue(charContr.enabled, "Character controller was not enabled again!");
    }

    [UnityTest]
    public IEnumerator Test_ShipDisablesItsCollidersInIntro()
    {
        ShipIntro ship = CreateShip();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        ship.gameCtr = gameCtr;
        GameObject shipModel = new GameObject("ShipModel");
        shipModel.transform.SetParent(ship.transform);
        BoxCollider shipCollider1 = shipModel.AddComponent<BoxCollider>();
        BoxCollider shipCollider2 = shipModel.AddComponent<BoxCollider>();

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(shipCollider1.enabled, "Box collider 1 was not disabled!");
        Assert.IsFalse(shipCollider2.enabled, "Box collider 2 was not disabled!");

        ship.animFinished = true;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(shipCollider1.enabled, "Box collider 1 was not enabled again!");
        Assert.IsTrue(shipCollider2.enabled, "Box collider 2 was not enabled again!");
    }

    // signalizes intro ended




    // --------------------- helper methods ----------------------------------------

    public ShipIntro CreateShip(bool addAnim = false)
    {
        ShipIntro s = new GameObject().AddComponent<ShipIntro>();

        if (addAnim)
        {
            Animation anim = s.gameObject.AddComponent<Animation>();
            anim.clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/ShipAnim.anim");
            anim.playAutomatically = true;
        }

        return s;
    }

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        return p;
    }

    public GameController CreateGameController(Player p)
    {
        GameController g = new GameObject().AddComponent<GameController>();
        g.player = p;
        return g;
    }

    IUnityStaticService CreateUnityService(float deltaTimeReturn, float horizontalReturn, float verticalReturn)
    {
        IUnityStaticService s = Substitute.For<IUnityStaticService>();
        s.GetDeltaTime().Returns(deltaTimeReturn);
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }

}
