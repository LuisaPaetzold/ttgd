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
        TextMeshProUGUI pointsText = new GameObject("PointsText").AddComponent<TextMeshProUGUI>();
        pointsText.transform.SetParent(gameUI.transform);

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), 
            "Game UI does not display current points after game start!");
        gameCtr.GetPlayerStats().ModifyPoints(10);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), 
            "Game UI does not display current points after adding points!");
        player.stats.ModifyPoints(-10);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(pointsText.text.Contains(player.stats.GetCurrentPoints().ToString()), 
            "Game UI does not display current points after losing points!");
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

        yield return new WaitForSeconds(player.stats.TurnTime);

        Assert.IsTrue(attackBtnScript.IsInteractable(), "Attack Button wasn't interactable after the player waited their turn time!");

        player.stats.AttackOpponent(enemy.stats, false);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(attackBtnScript.IsInteractable(), "Attack Button wasn't reset to not interactable after the player attacked!");
    }

    [UnityTest]
    public IEnumerator Test_AttackButtonDisplaysCurrentAttackBoost()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        Button attackBtnScript = CreateMockObjectWithName("AttackBtn").AddComponent<Button>();
        TextMeshProUGUI attackBtnText = new GameObject().AddComponent<TextMeshProUGUI>();
        attackBtnText.transform.SetParent(attackBtnScript.transform);

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        float boost = player.stats.GetCurrentAttackDamage(false) / player.stats.GetDefaultAttackDamage();
        Assert.IsTrue(attackBtnText.text.Contains(boost.ToString()), "Attack Button doesn't display current attack boost!");

        for (int i = 0; i < player.stats.GetMaxAmountOfChargings(); i++)
        {
            player.stats.UseChargeForDamageBoost(true);
            boost = player.stats.GetCurrentAttackDamage(false) / player.stats.GetDefaultAttackDamage();
            yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();
            Assert.IsTrue(attackBtnText.text.Contains(boost.ToString()), "Attack Button doesn't display current attack boost!");
        }

        player.stats.AttackOpponent(enemy.stats, false, true);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();
        boost = player.stats.GetCurrentAttackDamage(false) / player.stats.GetDefaultAttackDamage();
        Assert.IsTrue(attackBtnText.text.Contains(boost.ToString()), "Attack Button doesn't display current attack boost!");
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

        yield return new WaitForSeconds(player.stats.TurnTime);

        Assert.IsTrue(chargeBtnScript.IsInteractable(), "Charge Button wasn't interactable after the player waited their turn time!");

        player.stats.UseChargeForDamageBoost();
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(chargeBtnScript.IsInteractable(), "Charge Button wasn't reset to not interactable after the player charged!");
    }

    [UnityTest]
    public IEnumerator Test_FleeButtonOnlyInteractableIfCanAct()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 0;
        Button fleeBtnScript = CreateMockObjectWithName("FleeBtn").AddComponent<Button>();

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();
        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(fleeBtnScript.IsInteractable(), "Flee Button was interactable before the player waited their turn time!");

        yield return new WaitForSeconds(player.stats.TurnTime);

        Assert.IsTrue(fleeBtnScript.IsInteractable(), "Flee Button wasn't interactable after the player waited their turn time!");

        gameCtr.PlayerTryFleeBattle();
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(fleeBtnScript.IsInteractable(), "Flee Button wasn't reset to not interactable after the player tried to flee!");
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

        yield return new WaitForSeconds(player.stats.TurnTime);

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
    public IEnumerator Test_FleeButtonNotInteractableIfFleeingNotAllowed()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = false;
        Button fleeBtnScript = CreateMockObjectWithName("FleeBtn").AddComponent<Button>();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForSeconds(player.stats.TurnTime);

        Assert.IsFalse(fleeBtnScript.IsInteractable(), "Flee Button was interactable even though fleeing is not allowed!");
    }

    [UnityTest]
    public IEnumerator Test_RedXOverFleeButtonIfFleeingNotAllowed()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = false;
        GameObject fleeBtn = CreateMockObjectWithName("FleeBtn");
        fleeBtn.AddComponent<Button>();
        GameObject redX = CreateMockObjectWithName("X");
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(redX.activeSelf, "Red X over flee Button was not active even though fleeing is not allowed!");

        gameCtr.EndBattle();

        yield return new WaitForEndOfFrame();

        enemy.playerCanFlee = true;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(redX.activeSelf, "Red X over flee Button was not active even though fleeing is not allowed!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerGetsTeleportedBackwardsOnZAxisFromEnemyPositionWhenBattleStarts()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        GameController gameCtr = CreateGameController(player);

        Vector3 playerPos = new Vector3(2, 3, 4);
        Vector3 enemyPos = new Vector3(1, 1, 1);
        player.transform.position = playerPos;
        enemy.transform.position = enemyPos;
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.AreNotEqual(playerPos, player.transform.position, "Player didn't get teleported at all!");
        Assert.AreEqual(enemy.transform.position.x, player.transform.position.x, "Player got moved wrong on x axis when battle started!");
        //Assert.AreEqual(enemy.transform.position.y, player.transform.position.y, "Player got moved wrong on y axis when battle started!");
        Assert.AreNotEqual(enemy.transform.position.z, player.transform.position.z, "Player didn't get moved on z axis when battle started!");
    }

    [UnityTest]
    public IEnumerator Test_CameraOnlyGetsTeleportedBackwardsPartOfTheDistance()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        CameraFollow cameraMock = CreateCameraMock();
        GameController gameCtr = CreateGameController(player);

        Vector3 playerPos = new Vector3(2, 3, 4);
        Vector3 enemyPos = new Vector3(1, 1, 1);
        player.transform.position = playerPos;
        enemy.transform.position = enemyPos;

        Vector3 camPos = cameraMock.transform.position;
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        float playerMovedZ = player.transform.position.z - enemyPos.z;
        float cameraMovedZ = cameraMock.transform.position.z - camPos.z;

        Assert.AreEqual(camPos.x, cameraMock.transform.position.x, "Camera got moved on x axis when battle started!");
        Assert.AreEqual(camPos.y, cameraMock.transform.position.y, "Camera got moved on y axis when battle started!");
        Assert.AreNotEqual(camPos.z, cameraMock.transform.position.z, "Camera didn't get moved on z axis when battle started!");
        Assert.Less(Mathf.Abs(cameraMovedZ), Mathf.Abs(playerMovedZ), "Camera did not get moved part of the distance the player was moved!");
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
    public IEnumerator Test_BattleEndsWhenPlayerFleesSuccessfully()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 1;
        GameController gameCtr = CreateGameController(player);

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameCtr.IsInBattle(), "Battle didn't even start when it should have!");

        gameCtr.PlayerTryFleeBattle();
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(gameCtr.IsInBattle(), "Battle did not end when player fled successfully!");
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

    [UnityTest]
    public IEnumerator Test_TeleporterMovesPlayerToTeleportPosition()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        TeleportToPosition teleport = CreateTeleporter(new Vector3(1, 1, 1));
        teleport.GameCtr = gameCtr;

        Vector3 playerPos = new Vector3(2, 3, 4);
        player.transform.position = playerPos;

        yield return new WaitForEndOfFrame();

        teleport.PlayerTeleport();

        yield return new WaitForSeconds(teleport.TeleportTime);

        Assert.AreNotEqual(playerPos, player.transform.position, "Player didn't get teleported at all!");
        Assert.AreEqual(teleport.TeleportTo.transform.position, player.transform.position, "Player did not get teleported to teleport position!");
    }

    [UnityTest]
    public IEnumerator Test_TeleporterIsBlockedWhileTeleportInProgress()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        TeleportToPosition teleport = CreateTeleporter(new Vector3(1, 1, 1));
        teleport.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(teleport.CanTeleport(), "Teleporter was blocked before any teleporting started!");

        teleport.PlayerTeleport();

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(teleport.CanTeleport(), "Teleporter was not blocked after teleporting started!");

        yield return new WaitForSeconds(teleport.TeleportTime);

        Assert.IsTrue(teleport.CanTeleport(), "Teleporter was still blocked after teleporting finished!");
    }

    [UnityTest]
    public IEnumerator Test_TeleporterTriggersBlackScreen()
    {
        Canvas blackScreenUI = new GameObject("BlackScreenUI").AddComponent<Canvas>();
        Image black = new GameObject("Black").AddComponent<Image>();
        black.gameObject.transform.SetParent(blackScreenUI.transform);

        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.introFadeDuration = 0;
        TeleportToPosition teleport = CreateTeleporter(new Vector3(1, 1, 1));
        teleport.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(black.gameObject.activeSelf, "Black screen image was not activated on game start!");

        yield return new WaitForSeconds(0.001f);

        teleport.PlayerTeleport();

        yield return new WaitForSeconds(teleport.TeleportTime / 2);

        Assert.NotZero(black.canvasRenderer.GetAlpha(), "Black screen was not set visible during teleportation!");

        yield return new WaitForSeconds(teleport.TeleportTime / 2);

        Assert.Zero(black.canvasRenderer.GetAlpha(), "Black screen was not set transparent after teleportation finished!");
    }

    [UnityTest]
    public IEnumerator Test_BlackScreenFadesOutOnGameStart()
    {
        Canvas blackScreenUI = new GameObject("BlackScreenUI").AddComponent<Canvas>();
        Image black = new GameObject("Black").AddComponent<Image>();
        black.gameObject.transform.SetParent(blackScreenUI.transform);

        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.introFadeDuration = 0.001f;

        Assert.NotZero(black.canvasRenderer.GetAlpha(), "Black screen did not start non-transparent!");

        yield return new WaitForSeconds(gameCtr.introFadeDuration);

        Assert.Zero(black.canvasRenderer.GetAlpha(), "Black screen did not fade out after game start!");

    }

    [UnityTest]
    public IEnumerator Test_IntroTextVisibleOnGameStart()
    {
        Canvas introUI = new GameObject("IntroUI").AddComponent<Canvas>();
        Image bg = new GameObject("introBG").AddComponent<Image>();
        bg.gameObject.transform.SetParent(introUI.transform);
        TextMeshProUGUI text = new GameObject("introText").AddComponent<TextMeshProUGUI>();
        text.gameObject.transform.SetParent(bg.transform);

        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.introFadeDuration = 0.001f;

        yield return new WaitForSeconds(gameCtr.introFadeDuration);

        Assert.NotZero(bg.canvasRenderer.GetAlpha(), "Intro background did not fade in!");
        Assert.NotZero(text.canvasRenderer.GetAlpha(), "Intro text did not fade in!");

        gameCtr.EndIntro();

        yield return new WaitForSeconds(gameCtr.introFadeDuration);

        Assert.Zero(bg.canvasRenderer.GetAlpha(), "Intro background did not fade out after intro!");
        Assert.Zero(text.canvasRenderer.GetAlpha(), "Intro text did not fade out after intro!");
    }

    [UnityTest]
    public IEnumerator Test_GameEndScreenFadesInWhenGameEnds()
    {
        Canvas gameEndUI = new GameObject("GameEndUI").AddComponent<Canvas>();
        Image white = new GameObject("White").AddComponent<Image>();
        white.transform.SetParent(gameEndUI.transform);
        TextMeshProUGUI endText = new GameObject("EndText").AddComponent<TextMeshProUGUI>();

        Canvas blackScreenUI = new GameObject("BlackScreenUI").AddComponent<Canvas>();
        Image black = new GameObject("Black").AddComponent<Image>();
        black.gameObject.transform.SetParent(blackScreenUI.transform);

        endText.transform.SetParent(white.transform);
        float endDuration = 0.001f;

        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(gameEndUI.gameObject.activeSelf, "White screen UI was not activated on game start!");
        Assert.IsTrue(white.gameObject.activeSelf, "White screen image was not activated on game start!");
        Assert.IsTrue(endText.gameObject.activeSelf, "End text was not activated on game start!");

        Assert.Zero(white.canvasRenderer.GetAlpha(), "White screen was not set transparent on game start!");
        Assert.Zero(endText.canvasRenderer.GetAlpha(), "End text was not set transparent on game start!");


        gameCtr.InvokeGameEnd(endDuration, null);

        yield return new WaitForSeconds(endDuration);

        Assert.AreEqual(white.canvasRenderer.GetAlpha(), 1, "White screen did not fade in!");

        yield return new WaitForSeconds(endDuration);

        Assert.AreEqual(endText.canvasRenderer.GetAlpha(), 1, "End text did not fade in!");

        yield return new WaitForSeconds(endDuration * 2.5f);

        Assert.Zero(white.canvasRenderer.GetAlpha(), "White screen did not fade back out again!");

        yield return new WaitForSeconds(endDuration * 2.5f);

        Assert.AreEqual(black.canvasRenderer.GetAlpha(), 1, "Black screen did not fade in!");

        yield return new WaitForSeconds(endDuration);

        Assert.Zero(endText.canvasRenderer.GetAlpha(), "End text did not fade out in the end!");
    }

    [UnityTest]
    public IEnumerator Test_SwapsNormalForBrokenRoomWhenGameEnds()
    {
        float endDuration = 0.001f;
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        GameObject normalRoom = new GameObject("NormalRoom");
        GameObject brokenRoom = new GameObject("BrokenRoom");
        gameCtr.NormalRoom = normalRoom;
        gameCtr.BrokenRoom = brokenRoom;
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(normalRoom.activeSelf, "Normal room was not active on game start!");
        Assert.IsFalse(brokenRoom.activeSelf, "Broken room was active on game start!");

        gameCtr.InvokeGameEnd(endDuration, null);

        yield return new WaitForSeconds(endDuration * 3);

        Assert.IsFalse(normalRoom.activeSelf, "Normal room was not deactivated on game end!");
        Assert.IsTrue(brokenRoom.activeSelf, "Broken room not actived on game end!");
    }

    [UnityTest]
    public IEnumerator Test_ResetsFlareLightIntensityWhenGameEnds()
    {
        float endDuration = 0.001f;
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Light flareLight = new GameObject("FlareLight").AddComponent<Light>();
        flareLight.intensity = 20
            ;
        yield return new WaitForEndOfFrame();

        gameCtr.InvokeGameEnd(endDuration, flareLight);

        yield return new WaitForSeconds(endDuration * 3);

        Assert.Zero(flareLight.intensity, "Flare light intensity was not reset on game end!");
    }

    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.stats.TurnTime = 0.1f;
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

    public TeleportToPosition CreateTeleporter(Vector3 teleportPos)
    {
        TeleportToPosition t = new GameObject().AddComponent<TeleportToPosition>();
        t.TeleportTo = new GameObject();
        t.TeleportTo.transform.position = teleportPos;
        t.TeleportTime = 0.2f;
        return t;
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
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }
}
