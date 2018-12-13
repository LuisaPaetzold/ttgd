using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PMPlayer
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
    public IEnumerator Test_PlayerMovesAlongZAxisForHorizontalInput()
    {
        Player player = CreatePlayer();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 1, 0);
        player.staticService = staticService;

        yield return null;

        Assert.AreEqual(1, player.transform.position.z, 0.1f, "Player didn't move on z axis after horizontal input!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerMovesAlongXAxisForVerticalInput()
    {
        Player player = CreatePlayer();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 0, 1);
        player.staticService = staticService;

        yield return null;

        Assert.AreEqual(-1, player.transform.position.x, 0.1f, "Player didn't move on x axis after vertical input!");
    }

    [UnityTest]
    public IEnumerator Test_HealthBarIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject healthBar = new GameObject();
        GameObject healthBarParent = new GameObject();
        healthBar.transform.parent = healthBarParent.transform;
        player.healthBar = healthBar;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(healthBarParent.activeSelf, "Player health bar was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(healthBarParent.activeSelf, "Player health bar wasn't active during a battle!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(healthBarParent.activeSelf, "Player health bar didn't get deactivated again after a battle ended!");
    }

    [UnityTest]
    public IEnumerator Test_IndicatesAfterDodgingAnAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        Image dodgedSign = new GameObject().AddComponent<Image>();
        player.stats.dodged = dodgedSign.gameObject;
        player.stats.DodgePropability = 1;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Player dodged sign was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Player dodged sign was active in battle when the player didn't dodge!");

        enemy.stats.AttackOpponent(player.stats);
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(dodgedSign.gameObject.activeSelf, "Player dodged sign wasn't active in battle when the player dodged!");

        yield return new WaitForSeconds(1f);

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Player dodged sign wasn't deactivated!");
    }

    [UnityTest]
    public IEnumerator Test_CanAttackAfterWaitingTurnTime()
    {
        Player player = CreatePlayer();
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return null;

        Assert.IsTrue(player.stats.CanAttack(), "Player wasn't able to attack after waiting their turn time!");
    }

    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.inventory = new PlayerInventoryClass();
        return p;
    }

    public Enemy CreateEnemy()
    {
        Enemy e = new GameObject().AddComponent<Enemy>();
        e.stats = new EnemyStatsClass();
        return e;
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
        s.GetInputAxisRaw("Horizontal").Returns(horizontalReturn);
        s.GetInputAxisRaw("Vertical").Returns(verticalReturn);

        return s;
    }

}
