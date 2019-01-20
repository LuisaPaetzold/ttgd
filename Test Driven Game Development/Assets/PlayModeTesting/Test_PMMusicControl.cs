using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PMMusicControl
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
    public IEnumerator Test_GameStartsWithLoopingNormalMusic()
    {
        MusicControl mc = CreateMusicControl();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(mc.source.isPlaying, "Music did not play when game started!");
        Assert.AreEqual(mc.normalBgMusic, mc.source.clip, "Game did not start with normal background music!");
        Assert.IsTrue(mc.source.loop, "Music wasn't looping!");
    }

    [UnityTest]
    public IEnumerator Test_MusicFadesInOnGameStart()
    {
        MusicControl mc = CreateMusicControl();
        mc.staticService = CreateUnityService(mc.IntroFadeDuration / 2, 0, 0);

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(mc.normalMusicVolume / 2, mc.source.volume, "Music did not have the right volume!");

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(mc.normalMusicVolume, mc.source.volume, "Music did not have the right volume!");
    }

    [UnityTest]
    public IEnumerator Test_BattleMusicOnlyPlaysInBattle()
    {
        MusicControl mc = CreateMusicControl();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(mc.source.isPlaying, "Music did not play when battle started!");
        Assert.AreEqual(mc.battleMusic, mc.source.clip, "Battle did not start with battle background music!");
        Assert.AreEqual(mc.battleMusicVolume, mc.source.volume, "Music did not have the right volume in battle!");
        Assert.IsTrue(mc.source.loop, "Battle music wasn't looping!");

        gameCtr.EndBattle();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(mc.source.isPlaying, "Music did not play when returning from battle!");
        Assert.AreEqual(mc.normalBgMusic, mc.source.clip, "Did not return to normal background music after battle ended!");
        Assert.AreEqual(mc.normalMusicVolume, mc.source.volume, "Music did not have the right volume after ending the battle!");
        Assert.IsTrue(mc.source.loop, "Normal music wasn't looping!");
    }

    [UnityTest]
    public IEnumerator Test_SuspenseMusicPlaysInTeleportLocation()
    {
        MusicControl mc = CreateMusicControl();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        TeleportToPosition teleport = CreateTeleporter(new Vector3(1, 1, 1));
        teleport.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        teleport.PlayerTeleport();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(mc.source.isPlaying, "Music did not play in teleport location!");
        Assert.AreEqual(mc.suspenseBgMusic, mc.source.clip, "Teleporting down did not start suspense background music!");
        Assert.AreEqual(mc.suspenseMusicVolume, mc.source.volume, "Music did not have the right volume after teleporting down!");
        Assert.IsTrue(mc.source.loop, "Suspense music wasn't looping!");

        teleport.PlayerTeleport();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(mc.source.isPlaying, "Music did not play after teleporting back up!");
        Assert.AreEqual(mc.normalBgMusic, mc.source.clip, "Did not return to normal background music after teleporting back up!");
        Assert.AreEqual(mc.normalMusicVolume, mc.source.volume, "Music did not have the right volume after teleporting back up!");
        Assert.IsTrue(mc.source.loop, "Normal music wasn't looping!");
    }

    [UnityTest]
    public IEnumerator Test_MusicFadesOutOnGameEnd()
    {
        MusicControl mc = CreateMusicControl();
        mc.staticService = CreateUnityService(mc.OutroFadeDuration, 0, 0);

        yield return new WaitForEndOfFrame();
        mc.InvokeGameEnd();
        
        yield return new WaitForEndOfFrame();

        Assert.AreEqual(0, mc.source.volume, 0.00001f, "Music did not have the right volume!");
    }

    [UnityTest]
    public IEnumerator Test_MusicStopsOnGameOver()
    {
        MusicControl mc = CreateMusicControl();
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        Enemy enemy = CreateEnemy(false);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        player.stats.ReceiveDamage(player.stats.MaxHealth);

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(mc.source.isPlaying, "Music did not stop after game over!");
    }





    // --------------------- helper methods ----------------------------------------

    public MusicControl CreateMusicControl()
    {
        MusicControl m = new GameObject().AddComponent<MusicControl>();
        m.gameObject.AddComponent<AudioSource>();

        m.normalBgMusic = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Music/bensound-acousticbreeze.mp3");
        m.battleMusic = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Music/bensound-epic.mp3");
        m.suspenseBgMusic = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Music/bensound-instinct.mp3");

        return m;
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
}
