using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;
using UnityEditor.Animations;

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
        player.gameObject.AddComponent<CharacterController>();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 1, 0);
        player.staticService = staticService;

        yield return new WaitForFixedUpdate();
        
        Assert.AreEqual(1, player.transform.position.z, 0.1f, "Player didn't move on z axis after horizontal input!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerMovesAlongXAxisForVerticalInput()
    {
        Player player = CreatePlayer();
        player.gameObject.AddComponent<CharacterController>();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 0, 1);
        player.staticService = staticService;

        yield return new WaitForFixedUpdate();
        
        Assert.AreEqual(-1, player.transform.position.x, 0.1f, "Player didn't move on x axis after vertical input!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerCannotMoveWhenGameIsOver()
    {
        Player player = CreatePlayer();
        player.gameObject.AddComponent<CharacterController>();
        player.stats.playerSpeed = 1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 1, 1);
        player.staticService = staticService;

        GameController gameCtr = CreateGameController(player);
        gameCtr.gameEnded = true;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(new Vector3(0,0,0), player.transform.position, "Player moved even though game was already over!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerCannotMoveUntilIntroIsOver()
    {
        Player player = CreatePlayer();
        player.gameObject.AddComponent<CharacterController>();
        GameController gameCtr = CreateGameController(player);

        IUnityStaticService staticService = CreateUnityService(1, 1, 1);
        player.staticService = staticService;

        gameCtr.introPlaying = true;
        
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(new Vector3(0, 0, 0), player.transform.position, "Player moved even though intro wasn't over!");

        gameCtr.introPlaying = false;

        yield return new WaitForFixedUpdate();

        Assert.AreNotEqual(player.transform.position, new Vector3(0, 0, 0), "Player wasn't able to move even though intro was over!");

    }

    [UnityTest]
    public IEnumerator Test_PlayerIsAffectedByGravity()
    {
        Player player = CreatePlayer();
        player.gameObject.AddComponent<CharacterController>();
        player.gravityValue = -1.0f;
        IUnityStaticService staticService = CreateUnityService(1, 0, 0);
        player.staticService = staticService;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(-1, player.transform.position.y, 0.1f, "Player wasn't affected by gravity!");
    }

    [UnityTest]
    public IEnumerator Test_HealthBarColorChangesIfOnLastBreath()
    {
        Player player = CreatePlayer();
        player.stats.red = new Color(1, 1, 1);
        player.stats.green = new Color(5, 5, 5);
        Enemy enemy = CreateEnemy();
        Image healthBar = new GameObject().AddComponent<Image>();
        GameObject healthBarParent = new GameObject();
        healthBar.transform.SetParent(healthBarParent.transform);
        player.healthBar = healthBar.gameObject;

        int lastBreathDamage = Mathf.FloorToInt(player.stats.GetMaxHealth() - (player.stats.GetLastBreathThreshold() * player.stats.GetMaxHealth()));
        const int heal = 10;

        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(player.stats.green, healthBar.color, "Health bar was not set to green when player is not on last breath!");

        player.stats.ReceiveDamage(lastBreathDamage);
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(FighterState.lastBreath, player.stats.GetCurrentFighterState(), "Player didn't get to last breath after his health dropped below the threshold!");
        Assert.AreEqual(player.stats.red, healthBar.color, "Health bar was not set to red when player dropped to last breath!");

        player.stats.GetHealedBy(heal);
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(FighterState.alive, player.stats.GetCurrentFighterState(), "Fighter didn't recover from last breath after his health exceeded the threshold!");
        Assert.AreEqual(player.stats.green, healthBar.color, "Health bar was not set back to green when player was healed!");
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
        yield return new WaitForSeconds(player.stats.DodgeDuration / 2);

        Assert.IsTrue(dodgedSign.gameObject.activeSelf, "Player dodged sign wasn't active in battle when the player dodged!");

        yield return new WaitForSeconds(player.stats.DodgeDuration);

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
    public IEnumerator Test_CanActAfterWaitingTurnTime()
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
    public IEnumerator Test_TurnTimeUpdateIsIncreasedByPoints()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();

        GameController gameCtr = CreateGameController(player);
        enemy.GameCtr = gameCtr;
        player.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();
        float turnTimeIncrease = player.stats.currentTurnTime;
        Assert.NotZero(turnTimeIncrease, "Player turn time was not updated inside battle!");

        gameCtr.EndBattle();
        yield return new WaitForEndOfFrame();

        player.stats.ModifyPoints(10);
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();
        float higherTurnTimeIncrease = player.stats.currentTurnTime;
        Assert.NotZero(higherTurnTimeIncrease, "Player turn time was not updated inside battle!");
        Assert.AreNotEqual(higherTurnTimeIncrease, turnTimeIncrease, "Player turn time was not increased any different with more points!");
        Assert.Greater(higherTurnTimeIncrease, turnTimeIncrease, "Player turn time was not increased faster with more points!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterAttack()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController GameCtr = CreateGameController(player);
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.CanAct(), "Player wasn't allowed to act in the first place!");

        player.stats.AttackOpponent(enemy.stats, false, false);

        Assert.IsFalse(player.stats.CanAct(), "Player turn time did not reset after their attack!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterUsingAnItem()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController GameCtr = CreateGameController(player);
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.Healing;
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.CanAct(), "Player wasn't allowed to act in the first place!");

        player.inventory.UseItem(0);

        Assert.IsFalse(player.stats.CanAct(), "Player turn time did not reset after using an item!");
    }

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterCharging()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController GameCtr = CreateGameController(player);
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.CanAct(), "Player wasn't allowed to act in the first place!");

        player.stats.UseChargeForDamageBoost();

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

    [UnityTest]
    public IEnumerator Test_TurnTimeIsResetAfterTryingToFlee()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 0;
        GameController GameCtr = CreateGameController(player);
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.CanAct(), "Player wasn't allowed to act in the first place!");

        GameCtr.PlayerTryFleeBattle();

        Assert.IsFalse(player.stats.CanAct(), "Player turn time did not reset after their attack!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsAttackParticlesAfterLandingAHit()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem hitParticles = new GameObject("hitParticles").AddComponent<ParticleSystem>();
        player.AttackParticle = hitParticles.gameObject;
        player.AttackParticleLength = 0.01f;

        GameController gameCtr = CreateGameController(player);
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerAttackEnemyNoDodging();

        yield return new WaitForSeconds(0.005f);

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
        Assert.IsTrue(foundInstantiated, "Player did not spawn a correct particle system after landing a hit!");

        yield return new WaitForSeconds(player.AttackParticleLength + 0.1f);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsChargeParticlesAfterCharging()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem chargeParticles = new GameObject("chargeParticles").AddComponent<ParticleSystem>();
        player.ChargeParticle = chargeParticles.gameObject;
        player.ChargeParticleLength = 0.01f;

        GameController gameCtr = CreateGameController(player);
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerChargeForBoost();

        yield return new WaitForSeconds(0.005f);

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
        Assert.IsTrue(foundInstantiated, "Player did not spawn a correct particle system after charging!");

        yield return new WaitForSeconds(player.ChargeParticleLength + 0.1f);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_SpawnsBombParticlesAfterBombExplosion()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        ParticleSystem bombParticles = new GameObject("bombParticles").AddComponent<ParticleSystem>();
        player.BombParticle = bombParticles.gameObject;
        player.BombParticleLength = 0.01f;

        GameController gameCtr = CreateGameController(player);
        player.GameCtr = gameCtr;
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerThrowBomb();

        yield return new WaitForSeconds(0.005f);

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
        Assert.IsTrue(foundInstantiated, "Player did not spawn a correct particle system after landing a hit!");

        yield return new WaitForSeconds(player.BombParticleLength);
        foundParticles = GameObject.FindObjectsOfType<ParticleSystem>();
        Assert.IsTrue(foundParticles.Length == 1, "Spawned particle system was not removed!");
    }

    [UnityTest]
    public IEnumerator Test_CanCollectItemDrop()
    {
        Player player = CreatePlayer();
        ItemDrop drop = new GameObject("drop").AddComponent<ItemDrop>();
        drop.droppedItem = ScriptableObject.CreateInstance<Item>();
        Item tmpItem = drop.droppedItem;
        yield return new WaitForEndOfFrame();

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.inventory.PlayerHasItem(tmpItem), "Player could not collect the dropped item!");
    }

    [UnityTest]
    public IEnumerator Test_LastingAttackBoostAreRemovedWhenBattleEnds()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy();
        GameController gameCtr = CreateGameController(player);
        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        player.stats.AddLastingDamageBoost("test", 1);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.stats.lastingDamageBoosts.ContainsKey("test"), "Player did not add lasting damage boost in the first place!");

        gameCtr.EndBattle();

        yield return new WaitForEndOfFrame();

        Assert.Zero(player.stats.lastingDamageBoosts.Count, "Player still had lasting damage boosts after battle ended!");
    }

    [UnityTest]
    public IEnumerator Test_PlayerHasLoopingIdleAnimation()
    {
        Player player = CreatePlayer(true, true);
        
        yield return new WaitForEndOfFrame();

        Animator animator = player.GetComponent<Animator>();
        Assert.IsNotNull(animator, "There was no animator added to the player!");
        Assert.IsNotNull(animator.runtimeAnimatorController, "There was no animator controller added to the player's animator!");

        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("Battle Idle"), "Current animation did not have a matching name!");
        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"), "Current animation did not have a matching tag!");

        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).loop, "Idle animation was not looping!");
        Assert.AreEqual(1, animator.GetCurrentAnimatorStateInfo(0).speed, "Idle animation speed was not 1!");
    }

    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer(bool setUpComponentsInTest = true, bool addAnimator = false)
    {
        Player p = new GameObject().AddComponent<Player>();
        if (setUpComponentsInTest)
        {
            p.stats = new PlayerStatsClass();
            p.stats.TurnTime = 0.1f;
            p.inventory = new PlayerInventoryClass();
        }

        if (addAnimator)
        {
            Animator anim = p.gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/AssetStore/Meshtint Free Knight/Controllers/Meshtint Free Knight.controller");
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
