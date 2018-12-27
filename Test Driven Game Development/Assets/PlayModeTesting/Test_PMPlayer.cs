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
        Time.timeScale = 1;
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            GameObject.Destroy(o);
        }
    }

    [UnityTest]
    public IEnumerator Test_PlayerCreatesInventoryIfNotSet()
    {
        Player player = CreatePlayer(false);
        yield return null;
        Assert.IsNotNull(player.inventory, "Player did not have an inventory after game start!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerCreatesStatsIfNotSet()
    {
        Player player = CreatePlayer(false);
        yield return null;
        Assert.IsNotNull(player.stats, "Player did not have stats after game start!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerMovesAlongZAxisForHorizontalInput()
    {
        Player player = CreatePlayer();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 1, 0);
        player.staticService = staticService;

        yield return null;

        Debug.Log(player.charContr);
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
    public IEnumerator Test_PlayerIsAffectedByGravity()
    {
        Player player = CreatePlayer();
        player.gravityValue = -1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 0, 0);
        player.staticService = staticService;

        yield return null;

        Assert.AreEqual(-1, player.transform.position.y, 0.1f, "Player wasn't affected by gravity!");
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
    public IEnumerator Test_TurnTimeBarIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject turnTimeBar = new GameObject();
        GameObject turnTimeBarParent = new GameObject();
        turnTimeBar.transform.parent = turnTimeBarParent.transform;
        player.healthBar = turnTimeBar;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(turnTimeBarParent.activeSelf, "Player turn time bar was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(turnTimeBarParent.activeSelf, "Player turn time bar wasn't active during a battle!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(turnTimeBarParent.activeSelf, "Player turn time bar didn't get deactivated again after a battle ended!");
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

        enemy.stats.AttackOpponent(player.stats, true, true);
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(dodgedSign.gameObject.activeSelf, "Player dodged sign wasn't active in battle when the player dodged!");

        yield return new WaitForSeconds(1f);

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Player dodged sign wasn't deactivated!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerStartsBattleWithTurnTimeAtZero()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        yield return null;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        Assert.Zero(player.stats.currentTurnTime, "Player did not start the battle with their turn time at 0!");
    }

    [UnityTest]
    public IEnumerator Test_CanAttackAfterWaitingTurnTime()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.CanAct(), "Player wasn't able to attack after waiting their turn time!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsOnlyUpdatedInBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();

        yield return new WaitForEndOfFrame();
        Assert.Zero(player.stats.currentTurnTime, "Player turn time was updated outside of battle!");

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.NotZero(player.stats.currentTurnTime, "Player turn time was not updated inside battle!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        player.stats.AttackOpponent(enemy.stats, false, true);

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(player.stats.CanAct(), "Player turn time did not reset after their attack!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterCharge()
    {
        Player player = CreatePlayer();
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        player.stats.UseChargeForDamageBoost();

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(player.stats.CanAct(), "Player turn time did not reset after charging!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetEvenIfOpponentDodged()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        enemy.stats.DodgePropability = 1;
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return null;

        player.stats.AttackOpponent(enemy.stats, true, true);

        Assert.AreEqual(enemy.stats.MaxHealth, enemy.stats.currentHealth, "Enemy did not dodge!");
        Assert.IsFalse(player.stats.CanAct(), "Player turn time did ot reset after an unsuccessful attack!");
    }

    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer(bool setUpComponentsInTest = true)
    {
        Player p = new GameObject().AddComponent<Player>();
        if (setUpComponentsInTest)
        {
            p.stats = new PlayerStatsClass();
            p.inventory = new PlayerInventoryClass();
            p.gameObject.AddComponent<CharacterController>();
        }
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
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }

}
