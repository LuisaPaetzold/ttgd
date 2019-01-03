using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PMDoorControl
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
    public IEnumerator Test_DoorControlKnowsItsEnemies()
    {
        GameObject enemies = CreateEnemiesToWaitFor(2);
        DoorControl door = CreateDoorControl(enemies, new Vector3(1, 1, 1));

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(door.GetEnemies(), "Enemies to wait for were not correctly initialized!");
        Assert.NotZero(door.GetEnemies().Count, "Door Control did not find any enemies!");
        Assert.IsTrue(door.GetEnemies().Count == 2, "Door Control did not know all of the enemies it should!");
    }

    [UnityTest]
    public IEnumerator Test_DoorControlInformsDyingEnemyWetherTheyAreTheLast()
    {
        GameObject enemies = CreateEnemiesToWaitFor(2);
        DoorControl door = CreateDoorControl(enemies, new Vector3(1, 1, 1));
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        Enemy firstEnemy = door.GetEnemies()[0];
        Enemy secondEnemy = door.GetEnemies()[1];

        gameCtr.StartBattle(firstEnemy);
        firstEnemy.stats.ReceiveDamage(firstEnemy.stats.GetMaxHealth());

        Assert.IsFalse(door.CheckIfLastEnemy(), "Door Control told the wrong enemy that they are the last!");

        yield return new WaitForEndOfFrame();
        
        gameCtr.StartBattle(secondEnemy);
        secondEnemy.stats.ReceiveDamage(secondEnemy.stats.GetMaxHealth());

        Assert.IsTrue(door.CheckIfLastEnemy(), "Door Control did not tell the right enemy that they are the last!");
    }

    [UnityTest]
    public IEnumerator Test_LastDyingEnemySpawnsKeyDrop()
    {
        GameObject enemies = CreateEnemiesToWaitFor(2);
        DoorControl door = CreateDoorControl(enemies, new Vector3(1, 1, 1));
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        Enemy firstEnemy = door.GetEnemies()[0];
        Enemy secondEnemy = door.GetEnemies()[1];

        gameCtr.StartBattle(firstEnemy);
        firstEnemy.stats.ReceiveDamage(firstEnemy.stats.GetMaxHealth());

        yield return new WaitForEndOfFrame();

        ItemDrop[] foundDrops1 = GameObject.FindObjectsOfType<ItemDrop>();

        Assert.IsTrue(foundDrops1.Length == 1, "Key Drop was spawned by wrong enemy!");

        gameCtr.StartBattle(secondEnemy);
        secondEnemy.stats.ReceiveDamage(secondEnemy.stats.GetMaxHealth());

        yield return new WaitForEndOfFrame();

        ItemDrop[] foundDrops2 = GameObject.FindObjectsOfType<ItemDrop>();
        ItemDrop found = null;
        foreach (ItemDrop drop in foundDrops2)
        {
            if (drop.name.Contains("(Clone)"))
            {
                found = drop;
            }
        }

        Assert.IsTrue(foundDrops2.Length == 2, "Key Drop was not spawned by right enemy!");
        Assert.IsNotNull(found, "No Key Drop clone was found!");
    }

    [UnityTest]
    public IEnumerator Test_KeyDropOpensDoor()
    {
        DoorControl door = CreateDoorControl(null, new Vector3(1, 1, 1));
        Player player = CreatePlayer();
        door.keyDrop.player = player;
        door.transform.rotation = new Quaternion(1, 1, 1, 1);

        Vector3 closedPos = door.transform.position;
        Quaternion closedRot = door.transform.rotation;

        yield return new WaitForEndOfFrame();

        door.keyDrop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Vector3 openPos = door.transform.position;
        Quaternion openRot = door.transform.rotation;

        Assert.AreNotEqual(openRot, closedRot, "Door was not rotated to be opened!");
        Assert.AreEqual(openPos, door.positionOpen, "Door was not moved into the correct position to be opened!");
    }


    // --------------------- helper methods ----------------------------------------

    public DoorControl CreateDoorControl(GameObject enemiesToWaitFor, Vector3 openPos)
    {
        DoorControl d = new GameObject().AddComponent<DoorControl>();
        d.keyDrop = new GameObject().AddComponent<ItemDrop>();
        d.EnemiesToWaitFor = enemiesToWaitFor;
        d.positionOpen = openPos;

        return d;
    }

    public GameObject CreateEnemiesToWaitFor(int count)
    {
        GameObject enemies = new GameObject("EnemiesToWaitFor");

        for (int i = 0; i < count; i++)
        {
            Enemy e = CreateEnemy(false);
            e.transform.SetParent(enemies.transform);
        }

        return enemies;
    }

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
}
