﻿using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections;
using NSubstitute;
using UnityEditor.Animations;

public class Test_PMEnemy
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
    public IEnumerator Test_EnemyCreatesStatsIfNotSet()
    {
        Enemy enemy = CreateEnemy(false);
        yield return null;
        Assert.IsNotNull(enemy.stats, "Enemy did not have stats after game start!");
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
        enemy.stats.AttackDamage = player.stats.GetCurrentHealth();
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

        player.stats.ReceiveDamage(player.stats.MaxHealth);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(healthBarParent.activeSelf, "Enemy health bar didn't get deactivated again after a battle ended!");
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
        enemy.stats.DodgeDuration = 0.01f;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign was active outside of a battle!");

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign was active in battle when the enemy didn't dodge!");

        player.stats.AttackOpponent(enemy.stats, true, true);
        yield return new WaitForSeconds(enemy.stats.DodgeDuration / 2);

        Assert.IsTrue(dodgedSign.gameObject.activeSelf, "Enemy dodged sign wasn't active in battle when the enemy dodged!");

        yield return new WaitForSeconds(enemy.stats.DodgeDuration);

        Assert.IsFalse(dodgedSign.gameObject.activeSelf, "Enemy dodged sign wasn't deactivated!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsAttackParticlesAfterLandingAHit()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem hitParticles = new GameObject("hitParticles").AddComponent<ParticleSystem>();
        enemy.AttackParticle = hitParticles.gameObject;
        enemy.AttackProbability = 1;
        enemy.AttackParticleLength = 0.01f;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        enemy.ChooseRandomBattleActionAndAct(false, true);

        yield return new WaitForEndOfFrame();

        ParticleSystem[] foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        bool foundInstantiated = false;

        foreach(ParticleSystem p in foundParticles)
        {
            if (p.gameObject.name.Contains("(Clone)"))
            {
                foundInstantiated = true;
            }
        }

        Assert.IsTrue(foundParticles.Length == 2, "No new particle system was spawned!");
        Assert.IsTrue(foundInstantiated, "Enemy did not spawn a correct particle system after landing a hit!");

        yield return new WaitForSeconds(enemy.AttackParticleLength + 0.1f);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsChargeParticlesAfterCharging()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem chargeParticles = new GameObject("chargeParticles").AddComponent<ParticleSystem>();
        enemy.ChargeParticle = chargeParticles.gameObject;
        enemy.AttackProbability = 0;
        enemy.ChargeParticleLength = 0.01f;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        enemy.ChooseRandomBattleActionAndAct(false, true);

        yield return new WaitForEndOfFrame();

        ParticleSystem[] foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        bool foundInstantiated = false;

        foreach (ParticleSystem p in foundParticles)
        {
            if (p.gameObject.name.Contains("(Clone)"))
            {
                foundInstantiated = true;
            }
        }

        Assert.IsTrue(foundParticles.Length == 2, "No new particle system was spawned!");
        Assert.IsTrue(foundInstantiated, "Enemy did not spawn a correct particle system after charging!");

        yield return new WaitForSeconds(enemy.ChargeParticleLength + 0.1f);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsDeathParticlesAfterDying()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem deathParticles = new GameObject("deathParticles").AddComponent<ParticleSystem>();
        enemy.DeathParticle = deathParticles.gameObject;
        enemy.DeathParticleLength = 0.1f;
        enemy.autoAttack = false;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.stats.ReceiveDamage(enemy.stats.GetMaxHealth());

        yield return new WaitForEndOfFrame();

        ParticleSystem[] foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        bool foundInstantiated = false;

        foreach (ParticleSystem p in foundParticles)
        {
            if (p.gameObject.name.Contains("(Clone)"))
            {
                foundInstantiated = true;
            }
        }

        Assert.IsTrue(foundParticles.Length == 2, "No new particle system was spawned!");
        Assert.IsTrue(foundInstantiated, "Enemy did not spawn a correct particle system after dying!");

        yield return new WaitForSeconds(enemy.DeathParticleLength + 0.1f);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_DropsItemAfterDying()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ItemDrop droppedItem = new GameObject("droppedItem").AddComponent<ItemDrop>();
        enemy.DroppableItems = new System.Collections.Generic.List<ItemDrop>();
        enemy.DroppableItems.Add(droppedItem);
        enemy.autoAttack = false;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.stats.ReceiveDamage(enemy.stats.GetMaxHealth());

        yield return new WaitForEndOfFrame();

        ItemDrop[] foundDrops = GameObject.FindObjectsOfType<ItemDrop>();
        bool foundInstantiated = false;

        foreach (ItemDrop drop in foundDrops)
        {
            if (drop.gameObject.name.Contains("(Clone)"))
            {
                foundInstantiated = true;
            }
        }

        Assert.IsTrue(foundDrops.Length == 2, "No new item drop was spawned!");
        Assert.IsTrue(foundInstantiated, "Enemy did not drop a correct item drop after dying!");
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
    public IEnumerator Test_CanActAfterWaitingTurnTime()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(enemy.stats.TurnTime, 0, 0);
        enemy.staticService = staticService;
        enemy.autoAttack = false;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(enemy.stats.CanAct(), "Enemy wasn't able to attack after waiting their turn time!");
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

        Assert.IsFalse(enemy.stats.CanAct(), "Enemy turn time did not reset after their attack!");
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
        Assert.IsFalse(enemy.stats.CanAct(), "Enemy turn time did ot reset after an unsuccessful attack!");
    }

    [UnityTest]
    public IEnumerator Test_AttacksIfFullyCharged()
    {
        Player player = CreatePlayer();
        player.stats.DodgePropability = 0;
        Enemy enemy = CreateEnemy();
        IUnityStaticService staticService = CreateUnityService(enemy.stats.TurnTime, 0, 0);
        enemy.staticService = staticService;
        enemy.AttackProbability = 1;
        enemy.stats.MaxAmountOfChargings = 1;

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;
        float oldHealth = player.stats.GetCurrentHealth();

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.AreNotEqual(player.stats.GetCurrentHealth(), oldHealth, "Enemy wasn't forced to attack after charging to max!");
    }

    [UnityTest]
    public IEnumerator Test_CannotDodgeBomb()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        enemy.stats.DodgePropability = 1;
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerThrowBomb();

        yield return new WaitForEndOfFrame();

        int damagedHealth = enemy.stats.GetCurrentHealth();

        Assert.Less(damagedHealth, enemy.stats.GetMaxHealth(), "Enemy was able to dodge a bomb!");
    }

    [UnityTest]
    public IEnumerator Test_EnemyHasLoopingIdleAnimation()
    {
        Enemy enemy = CreateEnemy(true, true);

        yield return new WaitForEndOfFrame();

        Animator animator = enemy.GetComponent<Animator>();
        Assert.IsNotNull(animator, "There was no animator added to the enemy!");
        Assert.IsNotNull(animator.runtimeAnimatorController, "There was no animator controller added to the enemy's animator!");

        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("idle"), "Current animation did not have a matching name!");
        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"), "Current animation did not have a matching tag!");

        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).loop, "Idle animation was not looping!");
        Assert.AreEqual(1.5f, animator.GetCurrentAnimatorStateInfo(0).speed, "Idle animation speed was not 1.5f!");
    }








    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.inventory = new PlayerInventoryClass();
        return p;
    }

    public Enemy CreateEnemy(bool setUpComponentsInTest = true, bool addAnimator = false)
    {
        Enemy e = new GameObject().AddComponent<Enemy>();
        if (setUpComponentsInTest)
        {
            e.stats = new EnemyStatsClass();
        }
        if (addAnimator)
        {
            Animator anim = e.gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AssetStore/TAZO_3D_C/BOSS/Bull/Bull.controller");
        }
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
