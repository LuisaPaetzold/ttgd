using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public GameObject EnemiesToWaitFor;
    public ItemDrop keyDrop;

    public Vector3 positionOpen;

    internal List<Enemy> enemies;

	void Start ()
    {
        enemies = new List<Enemy>();

        if (EnemiesToWaitFor != null)
        {
            foreach (Enemy e in EnemiesToWaitFor.GetComponentsInChildren<Enemy>())
            {
                enemies.Add(e);
            }
        }

        foreach(Enemy e in enemies)
        {
            if (e != null && e.stats != null)
            {
                e.stats.lockedDoor = this;
            }
        }

        keyDrop.door = this;
	}
	

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    public void Open()
    {
        Debug.Log("OPEN");

        this.transform.localPosition = positionOpen;
        this.transform.rotation = new Quaternion();
    }

    public ItemDrop OnEnemyDied()
    {
        return CheckIfLastEnemy() ? keyDrop : null;
    }

    public bool CheckIfLastEnemy()
    {
        int aliveEnemies = 0;

        foreach (Enemy e in enemies)
        {
            if (e != null)
            {
                aliveEnemies++;
            }
        }

        if (aliveEnemies == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
