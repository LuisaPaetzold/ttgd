using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PMEnemy
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
    public IEnumerator Test_EnemyCanOnlyAttackIfPartOfCurrentBattle()
    {
        Player player = CreatePlayer();
        Enemy enemyRight = CreateEnemy();
        Enemy enemyWrong = CreateEnemy();

        GameController gameCtr = CreateGameController(player);
        enemyRight.GameCtr = gameCtr;
        enemyWrong.GameCtr = gameCtr;
        player.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemyRight);

        enemyWrong.stats.AttackOpponent(player.stats, false, true);

        LogAssert.Expect(LogType.Error, "Enemy that is not part of the current battle tried to attack the player!");
    }

    [UnityTest]
    public IEnumerator Test_HealthBarIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject healthBar = new GameObject();
        GameObject healthBarParent = new GameObject();
        healthBar.transform.parent = healthBarParent.transform;
        enemy.healthBar = healthBar;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(healthBarParent.activeSelf, "Enemy health bar was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(healthBarParent.activeSelf, "Enemy health bar wasn't active during a battle!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeBarIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject turnTimeBar = new GameObject();
        GameObject turnTimeBarParent = new GameObject();
        turnTimeBar.transform.parent = turnTimeBarParent.transform;
        enemy.healthBar = turnTimeBar;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(turnTimeBarParent.activeSelf, "Enemy turn time bar was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(turnTimeBarParent.activeSelf, "Enemy turn time bar wasn't active during a battle!");
    }

    [UnityTest]
    public IEnumerator Test_IndicatesAfterDodgingAnAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        Image dodgedSign = new GameObject().AddComponent<Image>();
        enemy.stats.dodged = dodgedSign.gameObject;
        enemy.stats.DodgePropability = 1;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign was active in battle when the enemy didn't dodge!");

        player.stats.AttackOpponent(enemy.stats, true, true);
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(dodgedSign.gameObject.activeSelf, "Enemy dodged sign wasn't active in battle when the enemy dodged!");

        yield return new WaitForSeconds(1f);

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign wasn't deactivated!");
    }

    [UnityTest]
    public IEnumerator Test_EnemyStartsBattleWithTurnTimeAtZero()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();

        yield return null;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        Assert.Zero(enemy.stats.currentTurnTime, "Enemy did not start the battle with their turn time at 0!");
    }

    [UnityTest]
    public IEnumerator Test_CanAttackAfterWaitingTurnTime()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(enemy.stats.TurnTime, 0, 0);
        enemy.staticService = staticService;
        enemy.autoAttack = false;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(enemy.stats.CanAttack(), "Enemy wasn't able to attack after waiting their turn time!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsOnlyUpdatedInBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();

        yield return new WaitForEndOfFrame();
        Assert.Zero(enemy.stats.currentTurnTime, "Enemy turn time was updated outside of battle!");

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.NotZero(enemy.stats.currentTurnTime, "Enemy turn time was not updated inside battle!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(enemy.stats.TurnTime, 0, 0);
        enemy.staticService = staticService;

        yield return new WaitForEndOfFrame();

        enemy.stats.AttackOpponent(player.stats, false, true);

        Assert.IsFalse(enemy.stats.CanAttack(), "Enemy turn time did not reset after their attack!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetEvenIfOpponentDodged()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        player.stats.DodgePropability = 1;
        IUnityStaticService staticService = CreateUnityService(enemy.stats.TurnTime, 0, 0);
        enemy.staticService = staticService;

        yield return new WaitForEndOfFrame();

        enemy.stats.AttackOpponent(player.stats, true, true);

        Assert.AreEqual(player.stats.MaxHealth, player.stats.currentHealth, "Player did not dodge!");
        Assert.IsFalse(enemy.stats.CanAttack(), "Enemy turn time did ot reset after an unsuccessful attack!");
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
