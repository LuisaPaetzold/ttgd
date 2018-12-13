using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;
using NSubstitute;

public class Test_PMGameController
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
    public IEnumerator Test_EnemiesAreDeletedWhenTheyDie()
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
    public IEnumerator Test_BattleEndsAfterAllEnemiesDied()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.IsInBattle(), "Battle did not start when it should have!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.IsInBattle(), "Battle did not end automatically after all enemies died!");
    }

    [UnityTest]
    public IEnumerator Test_BattleUIIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameObject battleUI = CreateMockObjectWithName("BattleUI");

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(battleUI.activeSelf, "Battle UI was active outside of battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(battleUI.activeSelf, "Battle UI wasn't active in battle!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(battleUI.activeSelf, "Battle UI wasn't deactivated after battle ended!");
    }

    [UnityTest]
    public IEnumerator Test_AttackButtonOnlyInteractableIfCanAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        enemy.stats.currentHealth = 1000;
        //IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        //player.staticService = staticService;
        Button attackBtnScript = CreateMockObjectWithName("AttackBtn").AddComponent<Button>();

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(attackBtnScript.IsInteractable(), "Attack Button was interactable before the player waited their turn time!");

        yield return new WaitForSeconds(3);

        Assert.IsTrue(attackBtnScript.IsInteractable(), "Attack Button wasn't interactable after the player waited their turn time!");

        player.stats.AttackOpponent(enemy.stats, false);
        yield return new WaitForEndOfFrame();

        //Assert.IsFalse(attackBtnScript.IsInteractable(), "Attack Button wasn't reset to not interactable after the player attacked!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerOnlyGetsTeleportedBackwardsOnZAxisWhenBattleStarts()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);

        Vector3 playerPos = new Vector3(2, 3, 4);
        player.transform.position = playerPos;
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(playerPos.x, player.transform.position.x, "Player got moved on x axis when battle started!");
        Assert.AreEqual(playerPos.y, player.transform.position.y, "Player got moved on y axis when battle started!");
        Assert.AreNotEqual(playerPos.z, player.transform.position.z, "Player didn't get moved on z axis when battle started!");
    }

    [UnityTest]
    public IEnumerator Test_CameraOnlyGetsTeleportedBackwardsHalfTheDistance()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        CameraFollow cameraMock = CreateCameraMock();
        GameController gameCtr = CreateGameController(player);

        Vector3 playerPos = new Vector3(2, 3, 4);
        player.transform.position = playerPos;
        Vector3 camPos = cameraMock.transform.position;
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        float cameraMovedZ = cameraMock.transform.position.z - camPos.z;
        float playerMovedZ = player.transform.position.z - playerPos.z;

        Assert.AreEqual(camPos.x, cameraMock.transform.position.x, "Camera got moved on x axis when battle started!");
        Assert.AreEqual(camPos.y, cameraMock.transform.position.y, "Camera got moved on y axis when battle started!");
        Assert.AreNotEqual(camPos.z, cameraMock.transform.position.z, "Camera didn't get moved on z axis when battle started!");
        Assert.AreEqual(cameraMovedZ * 2, playerMovedZ, "Camera did not get moved half the distance the player was moved!");
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

    public CameraFollow CreateCameraMock()
    {
        CameraFollow c = new GameObject().AddComponent<CameraFollow>();
        return c;
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
