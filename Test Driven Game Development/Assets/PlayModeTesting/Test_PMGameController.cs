using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PMGameController
{
    // Enemies get removed when dead
    // Healthbars are shown when battle starts
    // battle ends when enemies are dead

    [UnityTest]
    public IEnumerator EnemiesAreDeletedWhenTheyDie()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();
        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsEmpty(gameCtr.currentEnemies, "Dead enemy wasn't deleted from list inside game controller!");
        Assert.IsTrue(enemy == null, "Dead enemy wasn't deleted from scene!");
    }

    [UnityTest]
    public IEnumerator BattleEndsAfterAllEnemiesDied()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.IsInBattle(), "Battle did not start when it should!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.IsInBattle(), "Battle did not end automatically after all enemies died!");
    }

    [UnityTest]
    public IEnumerator BattleUIIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject battleUI = CreateMockObjectWithName("BattleUI");

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(battleUI.activeSelf);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(battleUI.activeSelf);

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(battleUI.activeSelf);
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

    public GameObject CreateMockObjectWithName(string name)
    {
        GameObject o = new GameObject();
        o.name = name;
        return o;
    }
}
