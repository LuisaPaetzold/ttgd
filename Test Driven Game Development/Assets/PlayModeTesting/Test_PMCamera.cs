using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PMCamera
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
    public IEnumerator Test_CameraFollowsPlayerAround()
    {
        Player player = CreatePlayer();
        CameraFollow cameraMock = CreateCameraMock();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForSeconds(1);

        float initialDistance = (player.transform.position - cameraMock.transform.position).magnitude;
        player.transform.position = new Vector3(5, 5, 5);
        yield return new WaitForSeconds(1);

        float distanceAfterMove = (player.transform.position - cameraMock.transform.position).magnitude;

        Assert.AreEqual(initialDistance, distanceAfterMove, "Camera did not follow the player around!");
    }

    [UnityTest]
    public IEnumerator Test_CameraStaysFixedInBattle()
    {
        Player player = CreatePlayer();
        CameraFollow cameraMock = CreateCameraMock();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForSeconds(1);

        float initialDistance = (player.transform.position - cameraMock.transform.position).magnitude;
        gameCtr.StartBattle(enemy);

        player.transform.position = new Vector3(5, 5, 5);
        yield return new WaitForSeconds(1);

        float distanceAfterMove = (player.transform.position - cameraMock.transform.position).magnitude;

        Assert.AreNotEqual(initialDistance, distanceAfterMove, "Camera still followed the player when he moved during battle!");
    }

    [UnityTest]
    public IEnumerator Test_CameraFolllowsPlayerAgainAfterBattleEnded()
    {
        Player player = CreatePlayer();
        CameraFollow cameraMock = CreateCameraMock();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForSeconds(1);

        float initialDistance = (player.transform.position - cameraMock.transform.position).magnitude;
        gameCtr.StartBattle(enemy);
        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForSeconds(1);

        float distanceAfterBattle = (player.transform.position - cameraMock.transform.position).magnitude;

        Assert.AreEqual(initialDistance, distanceAfterBattle, "Camera no longer follows the player after the battle ended!");
    }



    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.inventory = new PlayerInventoryClass();
        return p;
    }

    public CameraFollow CreateCameraMock()
    {
        CameraFollow c = new GameObject().AddComponent<CameraFollow>();
        return c;
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




