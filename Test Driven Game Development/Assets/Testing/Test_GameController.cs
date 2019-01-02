using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_GameController
{
    [SetUp]
    public void Setup()
    {
        Debug.ClearDeveloperConsole();
    }



    [Test]
    public void Test_GameControllerKnowsEnemiesWhenBattleStarts()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);

        Assert.IsNotEmpty(gameCtr.currentEnemies, "Game controller doesn't have any reference to the enemies of the current battle!");
    }

    [Test]
    public void Test_GameControllerClearsCurrentEnemiesWhenBattleEnds()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 1;
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);

        Assert.IsNotEmpty(gameCtr.currentEnemies, "Game controller doesn't have any reference to the enemies of the current battle!");

        gameCtr.PlayerTryFleeBattle();

        Assert.IsEmpty(gameCtr.currentEnemies, "Game controller did not clear the enemies of the current battle!");
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
}
