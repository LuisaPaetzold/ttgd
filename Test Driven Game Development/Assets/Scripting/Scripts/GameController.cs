using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IGameController
{
    public Player player;
    private PlayerStatsClass playerStats;
    public List<Enemy> currentEnemies = new List<Enemy>();
    internal bool isInBattle = false;

    internal GameObject battleUI;
    internal GameObject attackBtn;
    internal GameObject chargeBtn;
    internal Button attackBtnScript;
    internal Button chargeBtnScript;
    internal Text chargeBtnText;
    internal GameObject gameUI;
    internal Text pointsText;
    internal GameObject gameOverUI;
    internal CameraFollow gameCam;

    void Start ()
    {
        player = FindObjectOfType<Player>();
        playerStats = player.stats;

        battleUI = GameObject.Find("BattleUI");
        attackBtn = GameObject.Find("AttackBtn");
        chargeBtn = GameObject.Find("ChargeBtn");
        gameUI = GameObject.Find("GameUI");
        gameOverUI = GameObject.Find("GameOverUI");
        if (battleUI != null)
        {
            battleUI.SetActive(false);
        }
        if (attackBtn != null)
        {
            attackBtnScript = attackBtn.GetComponent<Button>();
            if (attackBtnScript != null)
            {
                attackBtnScript.interactable = false;
            }
        }
        if (chargeBtn != null)
        {
            chargeBtnScript = chargeBtn.GetComponent<Button>();
            if (chargeBtnScript != null)
            {
                chargeBtnScript.interactable = false;
                if (chargeBtnScript.transform.childCount > 0)
                {
                    Transform textObject = chargeBtnScript.transform.GetChild(0);
                    if (textObject != null)
                    {
                        chargeBtnText = textObject.GetComponent<Text>();
                    }
                }
            }
            
        }
        if (gameUI != null)
        {
            Transform textObj = gameUI.transform.Find("PointsText");
            if (textObj != null)
            {
                pointsText = textObj.gameObject.GetComponent<Text>();
                if (pointsText != null)
                {
                    pointsText.text = "0";
                }
            }
        }
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        
        if (gameCam == null)
        {
            gameCam = FindObjectOfType<CameraFollow>();
        }
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

            if (attackBtnScript != null)
            {
                if (player.stats.CanAct() && !attackBtnScript.IsInteractable())
                {
                    attackBtnScript.interactable = true;
                }
                else if (!player.stats.CanAct() && attackBtnScript.IsInteractable())
                {
                    attackBtnScript.interactable = false;
                }
            }

            if (chargeBtnScript != null)
            {
                if (chargeBtnText != null)
                {
                    chargeBtnText.text = "Charge (" + player.stats.GetCurrentAmountOfChargings() + "x)";
                }
                
                if (!chargeBtnScript.IsInteractable())
                {
                    if (player.stats.CanAct() && player.stats.GetCurrentAmountOfChargings() < player.stats.GetMaxAmountOfChargings())
                    {
                        chargeBtnScript.interactable = true;
                    }
                }
                else if (chargeBtnScript.IsInteractable())
                {
                    if (!player.stats.CanAct() || player.stats.GetCurrentAmountOfChargings() >= player.stats.GetMaxAmountOfChargings())
                    {
                        chargeBtnScript.interactable = false;
                    }
                }
            }
        }

        if (pointsText != null)
        {
            pointsText.text = player.stats.GetCurrentPoints().ToString();
        }
    }

    public void StartBattle(Enemy enemy)
    {
        isInBattle = true;
        currentEnemies.Add(enemy);
        //player.GetComponent<Rigidbody>().isKinematic = false;
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 2);
        if (gameCam != null)
        {
            gameCam.transform.position = new Vector3(gameCam.transform.position.x, gameCam.transform.position.y, gameCam.transform.position.z - 1);
        }
        if (battleUI != null)
        {
            battleUI.SetActive(true);
        }

        player.OnStartBattle();
        foreach (Enemy e in currentEnemies)
        {
            e.OnStartBattle();
        }
    }

    public void EndBattle()
    {
        isInBattle = false;
        if (battleUI != null)
        {
            battleUI.SetActive(false);
        }

        //player.GetComponent<Rigidbody>().isKinematic = true;
        player.OnEndBattle();
    }

    public void PlayerAttackEnemy()
    {
        playerStats.AttackOpponent(currentEnemies[0].stats);
    }

    public void PlayerChargeForBoost()
    {
        playerStats.UseChargeForDamageBoost();
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

    public bool TakesPartInCurrentBattle(EnemyStatsClass enemy)
    {
        foreach (Enemy e in GetCurrentEnemies())
        {
            if (e.stats == enemy)
            {
                return true;
            }
        }
        return false;
    }

    public bool TakesPartInCurrentBattle(Enemy enemy)
    {
        foreach (Enemy e in GetCurrentEnemies())
        {
            if (e == enemy)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsInBattle()
    {
        return isInBattle;
    }

    public void ReactToDodge(GameObject dodged)
    {
        StartCoroutine("HandleDodge", dodged);
    }

    private IEnumerator HandleDodge(GameObject dodged)
    {
        if (dodged != null)
        {
            Image dodgeImg = dodged.GetComponent<Image>();
            if (dodgeImg != null)
            {
                dodged.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                dodgeImg.CrossFadeAlpha(0, 0.5f, true);
                yield return new WaitForSeconds(0.5f);
                if (dodged != null)
                {
                    // check for nullpointer again as fighter might have died in the meantime
                    dodged.SetActive(false);
                }
            }
        }
    }

    public void HandlePlayerDeath()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Time.timeScale = 0;
        EndBattle();
    }
    #endregion Implementation IGameController
}

public interface IGameController
{
    PlayerStatsClass GetPlayerStats();
    List<Enemy> GetCurrentEnemies();
    bool IsInBattle();
    void ReactToDodge(GameObject dodged);
    void HandlePlayerDeath();
    bool TakesPartInCurrentBattle(EnemyStatsClass enemy);
    bool TakesPartInCurrentBattle(Enemy enemy);
}
