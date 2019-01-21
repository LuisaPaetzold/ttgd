using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;
using TMPro;

public class Test_PMSoundEffectControl
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
    public IEnumerator Test_SoundWhenPlayerAttacks()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.playerHit);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerAttackEnemyNoDodging();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.playerHit, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenEnemyAttacks()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.enemyHit);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        enemy.AttackProbability = 1;
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.ChooseRandomBattleActionAndAct(false, true);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.enemySource.isPlaying, "Enemy audio source did not play anything!");
        Assert.AreEqual(sfx.enemyHit, sfx.enemySource.clip, "Enemy audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenPlayerCharges()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.playerCharge);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerChargeForBoost();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.playerCharge, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenEnemyCharges()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.enemyCharge);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        enemy.AttackProbability = 0;
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.ChooseRandomBattleActionAndAct(false, true);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.enemySource.isPlaying, "Enemy audio source did not play anything!");
        Assert.AreEqual(sfx.enemyCharge, sfx.enemySource.clip, "Enemy audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenPlayerThrowsBomb()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.bomb);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        gameCtr.PlayerThrowBomb();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.bomb, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenPlayerHeals()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.heal);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.Healing;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        item.Use(player.stats, gameCtr);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.heal, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenPlayerUsesBoost()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.boost);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.AttackBoost;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        item.Use(player.stats, gameCtr);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.boost, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenEnemyDies()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.enemyDeath);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        enemy.AttackProbability = 0;
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.stats.ReceiveDamage(enemy.stats.MaxHealth);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.enemySource.isPlaying, "Enemy audio source did not play anything!");
        Assert.AreEqual(sfx.enemyDeath, sfx.enemySource.clip, "Enemy audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenCollectingItemDrop()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.itemPickUp);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.sfxControl = sfx;
        ItemDrop drop = CreateItemDrop();
        drop.player = player;

        yield return new WaitForEndOfFrame();

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.itemPickUp, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenCollectingKey()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.keyPickUp);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.sfxControl = sfx;
        ItemDrop drop = CreateItemDrop(true);
        drop.player = player;

        yield return new WaitForEndOfFrame();

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.keyPickUp, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenChestOpens()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.chestOpen);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        gameCtr.sfxControl = sfx;
        TreasureChest chest = CreateChest();
        chest.player = player;
        chest.transform.position = new Vector3(chest.openDistance + 1, 0, 0);

        yield return new WaitForEndOfFrame();

        player.transform.position = new Vector3(chest.openDistance, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.chestOpen, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenGameEnds()
    {
        float endDuration = 0.001f;
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.desaster);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        gameCtr.InvokeGameEnd(endDuration, null);

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.desaster, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenFleeing()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.flee);
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 1;
        GameController GameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        GameCtr.PlayerTryFleeBattle();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.flee, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenFailingToFlee()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.failFlee);
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        enemy.playerCanFlee = true;
        enemy.playerFleeProbability = 0;
        GameController GameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        GameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        GameCtr.PlayerTryFleeBattle();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.failFlee, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenTeleporting()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.teleport);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        TeleportToPosition teleport = CreateTeleporter(new Vector3(1, 1, 1));
        teleport.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        teleport.PlayerTeleport();

        yield return new WaitForSeconds(teleport.TeleportTime);

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.teleport, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenPlayerDodges()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.dodged);
        Player player = CreatePlayer();
        player.stats.DodgePropability = 1;
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        enemy.stats.AttackOpponent(player.stats, true, true);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.dodged, sfx.playerSource.clip, "Enemy audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenEnemyDodges()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.dodged);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        enemy.stats.DodgePropability = 1;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        player.stats.AttackOpponent(enemy.stats, true, true);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.enemySource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.dodged, sfx.enemySource.clip, "Enemy audio source did not play the right track!");
    }

    [UnityTest]
    public IEnumerator Test_SoundWhenGameOver()
    {
        SoundEffectControl sfx = CreateSFXControl(EffectToTest.gameOver);
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);
        gameCtr.sfxControl = sfx;

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        player.stats.ReceiveDamage(player.stats.MaxHealth);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(sfx.playerSource.isPlaying, "Player audio source did not play anything!");
        Assert.AreEqual(sfx.gameOver, sfx.playerSource.clip, "Player audio source did not play the right track!");
    }






    // --------------------- helper methods ----------------------------------------

    public enum EffectToTest
    {
        playerHit, enemyHit, playerCharge, enemyCharge, bomb, heal, boost, enemyDeath, itemPickUp, keyPickUp, chestOpen, desaster, flee, failFlee, teleport, dodged, gameOver
    };


    public SoundEffectControl CreateSFXControl(EffectToTest effect)
    {
        SoundEffectControl s = new GameObject().AddComponent<SoundEffectControl>();
        s.playerSource = s.gameObject.AddComponent<AudioSource>();
        s.enemySource = s.gameObject.AddComponent<AudioSource>();


        switch(effect)
        {
            case EffectToTest.playerHit:
                s.playerHit = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/PlayerHits/218996__yap-audio-production__swordclash01.mp3");
                break;
            case EffectToTest.enemyHit:
                s.enemyHit = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/EnemyHits/219002__yap-audio-production__weaponswipe01.mp3");
                break;
            case EffectToTest.playerCharge:
                s.playerCharge = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/PlayerCharge/234805__richerlandtv__magic4.mp3");
                break;
            case EffectToTest.enemyCharge:
                s.enemyCharge = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/EnemyCharge/349468_vr_magic (mp3cut.net).mp3");
                break;
            case EffectToTest.bomb:
                s.bomb = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Bomb/322484__liamg-sfx__explosion-11b.wav");
                break;
            case EffectToTest.heal:
                s.heal = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Heal/220173__gameaudio__spacey-1up-power-up.wav");
                break;
            case EffectToTest.boost:
                s.boost = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Boost/187407__mazk1985__power-up-grab.wav");
                break;
            case EffectToTest.enemyDeath:
                s.enemyDeath = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Death/446124__justinvoke__poof-puff.wav");
                break;
            case EffectToTest.itemPickUp:
                s.itemPickUp = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/PickupItem/443334__rolandseer__money-001.wav");
                break;
            case EffectToTest.keyPickUp:
                s.keyPickUp = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Keys/97461__egolessdub__keys10.wav");
                break;
            case EffectToTest.chestOpen:
                s.chestOpen = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/ChestOpen/246253__frankyboomer__magic-harp.wav");
                break;
            case EffectToTest.desaster:
                s.desaster = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/End/445194__juliusmabe__demon-laugh-2.wav");
                break;
            case EffectToTest.flee:
                s.flee = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Flee/19245__deathpie__shuffle.wav");
                break;
            case EffectToTest.failFlee:
                s.failFlee = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Flee/415764__thebuilder15__wrong.wav");
                break;
            case EffectToTest.teleport:
                s.teleport = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Teleport/442298__tmpz-1__vdhits-6.wav");
                break;
            case EffectToTest.dodged:
                s.dodged = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/Dodge/74690__benboncan__swoosh-3.wav");
                break;
            case EffectToTest.gameOver:
                s.gameOver = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Effects/GameOver/133283__leszek-szary__game-over.wav");
                break;
        }

        return s;
    }

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

    IUnityStaticService CreateUnityService(float deltaTimeReturn, float horizontalReturn, float verticalReturn)
    {
        IUnityStaticService s = Substitute.For<IUnityStaticService>();
        s.GetDeltaTime().Returns(deltaTimeReturn);
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }

    public TeleportToPosition CreateTeleporter(Vector3 teleportPos)
    {
        TeleportToPosition t = new GameObject().AddComponent<TeleportToPosition>();
        t.TeleportTo = new GameObject();
        t.TeleportTo.transform.position = teleportPos;
        t.TeleportTime = 0;
        return t;
    }

    public ItemDrop CreateItemDrop(bool isKey = false)
    {
        ItemDrop drop = new GameObject("drop").AddComponent<ItemDrop>();
        drop.droppedItem = ScriptableObject.CreateInstance<Item>();

        Canvas canvas = new GameObject("canvas").AddComponent<Canvas>();
        canvas.gameObject.transform.SetParent(drop.transform);

        TextMeshProUGUI text = new GameObject("text").AddComponent<TextMeshProUGUI>();
        text.gameObject.transform.SetParent(canvas.transform);

        if (isKey)
        {
            drop.droppedItem = null;
            drop.door = new GameObject().AddComponent<DoorControl>();
        }


        return drop;
    }

    public TreasureChest CreateChest()
    {
        TreasureChest t = new GameObject().AddComponent<TreasureChest>();

        return t;
    }
}
