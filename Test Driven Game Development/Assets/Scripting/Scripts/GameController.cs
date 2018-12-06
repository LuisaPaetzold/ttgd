using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    private Player player;
    private PlayerStatsClass playerStats;
    public List<Enemy> currentEnemies = new List<Enemy>();
    private bool isInBattle = false;

    private GameObject battleUI;

    void Start ()
    {
        player = FindObjectOfType<Player>();
        playerStats = player.stats;

        battleUI = GameObject.Find("BattleUI");
        battleUI.SetActive(false);
	}
	
	void Update ()
    {
		if (isInBattle)
        {
            for(int i = currentEnemies.Count - 1; i >= 0; i--)
            {
                Enemy e = currentEnemies[i];
                if (!e.IsAlive())
                {
                    currentEnemies.RemoveAt(i);
                    GameObject.Destroy(e.gameObject);
                }
            }
            if (currentEnemies.Count == 0)
            {
                EndBattle();
            }
        }
	}

    public void StartBattle(Enemy enemy)
    {
        Debug.Log("Start Battle!");
        isInBattle = true;
        currentEnemies.Add(enemy);
        player.transform.position = new Vector3(0, 0, -1);
        battleUI.SetActive(true);
    }

    public void EndBattle()
    {
        isInBattle = false;
        battleUI.SetActive(false);
    }

    public void PlayerAttackEnemy()
    {
        playerStats.AttackOpponent(currentEnemies[0].stats);
    }


    #region Implementation IGameController

    public PlayerStatsClass GetPlayerStats()
    {
        return playerStats;
    }

    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }

    public bool IsInBattle()
    {
        return isInBattle;
    }

    #endregion Implementation IGameController
}

public interface IGameController
{
    PlayerStatsClass GetPlayerStats();
    List<Enemy> GetCurrentEnemies();
    bool IsInBattle();
}
