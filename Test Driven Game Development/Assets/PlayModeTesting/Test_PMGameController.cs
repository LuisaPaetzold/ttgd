using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;
using NSubstitute;
using TMPro;

public class Test_PMGameController
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
    public IEnumerator Test_EnemiesAreDeletedWhenTheyDie()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
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
        Enemy enemy = CreateEnemy(false);
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.IsInBattle(), "Battle did not start when it should have!");

        enemy.stats.ReceiveDamage(enemy.stats.GetCurrentHealth());
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.IsInBattle(), "Battle did not end automatically after all enemies died!");
    }

    [UnityTest]
    public IEnumerator Test_GameUIDisplayCurrentPoints()
    {
        Player player = CreatePlayer();
        GameObject gameUI = CreateMockObjectWithName("GameUI");
        TextMeshProUGUI pointsText = CreateMockObjectWithName("PointsText").AddComponent<TextMeshProUGUI>();
        pointsText.transform.SetParent(gameUI.transform);

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), "Game UI does not display current points after game start!");
        gameCtr.GetPlayerStats().ModifyPoints(10);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), "Game UI does not display current points after adding points!");
        player.stats.ModifyPoints(-10);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), "Game UI does not display current points after losing points!");
    }

    [UnityTest]
    public IEnumerator Test_BattleUIIsOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
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
    public IEnumerator Test_AttackButtonOnlyInteractableIfCanAct()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.stats.currentHealth = 1000;
        Button attackBtnScript = CreateMockObjectWithName("AttackBtn").AddComponent<Button>();

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(attackBtnScript.IsInteractable(), "Attack Button was interactable before the player waited their turn time!");

        yield return new WaitForSeconds(3);

        Assert.IsTrue(attackBtnScript.IsInteractable(), "Attack Button wasn't interactable after the player waited their turn time!");

        player.stats.AttackOpponent(enemy.stats, false);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(attackBtnScript.IsInteractable(), "Attack Button wasn't reset to not interactable after the player attacked!");
    }

    [UnityTest]
    public IEnumerator Test_ChargeButtonOnlyInteractableIfCanAct()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        Button chargeBtnScript = CreateMockObjectWithName("ChargeBtn").AddComponent<Button>();

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(chargeBtnScript.IsInteractable(), "Charge Button was interactable before the player waited their turn time!");

        yield return new WaitForSeconds(3);

        Assert.IsTrue(chargeBtnScript.IsInteractable(), "Charge Button wasn't interactable after the player waited their turn time!");

        player.stats.UseChargeForDamageBoost();
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(chargeBtnScript.IsInteractable(), "Charge Button wasn't reset to not interactable after the player attacked!");
    }

    [UnityTest]
    public IEnumerator Test_ChargeButtonOnlyInteractableIfChargeCountBelowMax()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        Button chargeBtnScript = CreateMockObjectWithName("ChargeBtn").AddComponent<Button>();

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);

        yield return new WaitForSeconds(3);

        Assert.IsTrue(chargeBtnScript.IsInteractable(), "Charge Button wasn't interactable after the player waited their turn time!");

        for (int i = 0; i < player.stats.GetMaxAmountOfChargings(); i++)
        {
            player.stats.UseChargeForDamageBoost(true);
        }

        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(chargeBtnScript.IsInteractable(), "Charge Button wasn't reset to not interactable when the player already charged the max amount of times!");

        player.stats.AttackOpponent(enemy.stats, false, true);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsTrue(chargeBtnScript.IsInteractable(), "Charge Button wasn't set to interactable after the player fell back to zero chargings");
    }

    [UnityTest]
    public IEnumerator Test_ChargeButtonDisplaysCurrentChargeCount()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        Button chargeBtnScript = CreateMockObjectWithName("ChargeBtn").AddComponent<Button>();
        TextMeshProUGUI chargeBtnText = new GameObject().AddComponent<TextMeshProUGUI>();
        chargeBtnText.transform.SetParent(chargeBtnScript.transform);

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsTrue(chargeBtnText.text.Contains(player.stats.GetCurrentAmountOfChargings().ToString()), "Charge Button doesn't display current charge amount of 0 after start!");

        for (int i = 0; i < player.stats.GetMaxAmountOfChargings(); i++)
        {
            player.stats.UseChargeForDamageBoost(true);
            yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();
            Assert.IsTrue(chargeBtnText.text.Contains(player.stats.GetCurrentAmountOfChargings().ToString()), "Charge Button doesn't display current charge amount after charging!");
        }

        player.stats.AttackOpponent(enemy.stats, false, true);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsTrue(chargeBtnText.text.Contains(player.stats.GetCurrentAmountOfChargings().ToString()), "Charge Button doesn't display current charge amount of 0 after attacking!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerOnlyGetsTeleportedBackwardsOnZAxisWhenBattleStarts()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
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
        Enemy enemy = CreateEnemy(false);
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

    [UnityTest]
    public IEnumerator Test_BattleEndsWhenPlayerDies()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.IsInBattle(), "Battle didn't even start when it should have!");

        player.stats.ReceiveDamage(player.stats.MaxHealth);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.IsInBattle(), "Battle did not end when player died!");

    }

    [UnityTest]
    public IEnumerator Test_ShowsGameOverUIWhenPlayerDies()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        GameController gameCtr = CreateGameController(player);
        GameObject gameOverUI = CreateMockObjectWithName("GameOverUI");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameOverUI.activeSelf, "Game over UI wasn't deactivated on game start!");

        player.stats.ReceiveDamage(player.stats.MaxHealth);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameOverUI.activeSelf, "Game over UI wasn't activated when player died!");
    }

    [UnityTest]
    public IEnumerator Test_StopsTimeScaleWhenPlayerDies()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        player.stats.ReceiveDamage(player.stats.MaxHealth);
        yield return new WaitForEndOfFrame();

        Assert.Zero(Time.timeScale, "Time scale wasn't set to 0 when the game ended!");
    }

    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.inventory = new PlayerInventoryClass();
        return p;
    }

    public Enemy CreateEnemy(bool autoAttack)
    {
        Enemy e = new GameObject().AddComponent<Enemy>();
        e.stats = new EnemyStatsClass();
        e.autoAttack = autoAttack;
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
