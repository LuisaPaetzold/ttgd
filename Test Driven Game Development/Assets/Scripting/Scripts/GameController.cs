using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Rendering.PostProcessing;
//using UnityEngine.PostProcessing;

public class GameController : MonoBehaviour, IGameController
{
    public Player player;
    private PlayerStatsClass playerStats;
    public List<Enemy> currentEnemies = new List<Enemy>();

    public MusicControl musicControl;
    bool playerEnteredBasement;
    public SoundEffectControl sfxControl;

    public bool introPlaying = true;
    public bool gameEnded;

    public GameObject NormalRoom;
    public GameObject BrokenRoom;
    //public PostProcessingProfile desasterProfile;

    internal bool isInBattle = false;
    bool firstFrame = true;

    public float introFadeDuration = 1;

    internal GameObject battleUI;
    internal GameObject attackBtn;
    internal GameObject chargeBtn;
    internal GameObject fleeBtn;
    internal GameObject redX;
    internal Button attackBtnScript;
    internal Button chargeBtnScript;
    internal Button fleeBtnScript;
    internal TextMeshProUGUI chargeBtnText;
    internal GameObject gameUI;
    internal TextMeshProUGUI pointsText;
    internal GameObject gameOverUI;
    internal GameObject introUI;
    internal Image introBg;
    internal TextMeshProUGUI introText;
    internal GameObject blackScreenUI;
    internal Image black;
    internal GameObject gameEndUI;
    internal Image white;
    internal TextMeshProUGUI endText;
    internal CameraFollow gameCam;
    internal GameObject inventoryUI;

    void Start ()
    {
        player = FindObjectOfType<Player>();
        playerStats = player.stats;

        musicControl = FindObjectOfType<MusicControl>();
        sfxControl = FindObjectOfType<SoundEffectControl>();

        battleUI = GameObject.Find("BattleUI");
        attackBtn = GameObject.Find("AttackBtn");
        chargeBtn = GameObject.Find("ChargeBtn");
        fleeBtn = GameObject.Find("FleeBtn");
        redX = GameObject.Find("X");
        gameUI = GameObject.Find("GameUI");
        gameOverUI = GameObject.Find("GameOverUI");
        introUI = GameObject.Find("IntroUI");
        blackScreenUI = GameObject.Find("BlackScreenUI");
        gameEndUI = GameObject.Find("GameEndUI");
        inventoryUI = GameObject.Find("InventoryUI");

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
                        chargeBtnText = textObject.GetComponent<TextMeshProUGUI>();
                    }
                }
            }
            
        }
        if (fleeBtn != null)
        {
            fleeBtnScript = fleeBtn.GetComponent<Button>();
            if (fleeBtnScript != null)
            {
                fleeBtnScript.interactable = false;
            }
        }

        if (gameUI != null)
        {
            Transform textObj = gameUI.transform.Find("PointsText");
            if (textObj != null)
            {
                pointsText = textObj.gameObject.GetComponent<TextMeshProUGUI>();
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
        if (blackScreenUI != null)
        {
            Transform blackImg = blackScreenUI.transform.GetChild(0);
            if (blackImg != null)
            {
                black = blackImg.GetComponent<Image>();
                if (black != null)
                {
                    black.gameObject.SetActive(true);
                    //black.CrossFadeAlpha(0, introFadeDuration, true);
                }
            }
        }
        if (introUI != null)
        {
            Transform bg = introUI.transform.GetChild(0);
            if (bg != null)
            {
                introBg = bg.GetComponent<Image>();
                if (introBg != null)
                {
                    introBg.gameObject.SetActive(true);
                    introBg.CrossFadeAlpha(1, introFadeDuration, true);
                    
                    introText = introBg.GetComponentInChildren<TextMeshProUGUI>();
                    if (introText != null)
                    {
                        introText.gameObject.SetActive(true);
                        introText.CrossFadeAlpha(1, introFadeDuration, true);
                    }
                }
            }
        }
        if (gameEndUI != null)
        {
            Transform whiteImg = gameEndUI.transform.GetChild(0);
            if (whiteImg != null)
            {
                white = whiteImg.GetComponent<Image>();
                if (white != null)
                {
                    white.gameObject.SetActive(true);
                    white.CrossFadeAlpha(0, 0, true);
                    endText = white.GetComponentInChildren<TextMeshProUGUI>();
                    if (endText != null)
                    {
                        endText.gameObject.SetActive(true);
                        endText.CrossFadeAlpha(0, 0, true);
                    }
                }
            }
        }
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

        if (gameCam == null)
        {
            gameCam = FindObjectOfType<CameraFollow>();
        }

        if (NormalRoom != null)
        {
            NormalRoom.SetActive(true);
        }
        if (BrokenRoom != null)
        {
            BrokenRoom.SetActive(false);
        }
    }
	
	void Update ()
    {
        if (firstFrame)
        {
            firstFrame = false;
            if (black != null)
            {
                black.CrossFadeAlpha(0, introFadeDuration * 7, true);
            }
        }



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

            if (fleeBtnScript != null)
            {
                if (currentEnemies != null 
                    && currentEnemies.Count > 0
                    && currentEnemies[0] != null
                    && currentEnemies[0].playerCanFlee)
                {
                    if (player.stats.CanAct() && !fleeBtnScript.IsInteractable())
                    {
                        fleeBtnScript.interactable = true;
                    }
                    else if (!player.stats.CanAct() && fleeBtnScript.IsInteractable())
                    {
                        fleeBtnScript.interactable = false;
                    }

                    if (redX != null && redX.activeSelf)
                    {
                        redX.SetActive(false);
                    }
                }
                else
                {
                    fleeBtnScript.interactable = false;

                    if (redX != null && !redX.activeSelf)
                    {
                        redX.SetActive(true);
                    }
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
        player.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z - 4);
        if (gameCam != null)
        {
            gameCam.transform.position = new Vector3(gameCam.transform.position.x, gameCam.transform.position.y, player.transform.position.z + 1.5f);
        }
        if (battleUI != null)
        {
            battleUI.SetActive(true);
        }
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
            InventoryUI inventoryScript = GetInventoryUI();
            if (inventoryScript != null)
            {
                inventoryScript.UpdateInventoryUI();
            }
        }

        if (musicControl != null)
        {
            musicControl.StartBattle();
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
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            InventoryUI inventoryScript = GetInventoryUI();
            if (inventoryScript != null)
            {
                inventoryScript.UpdateInventoryUI();
            }
        }

        if (musicControl != null)
        {
            musicControl.EndBattle();
        }

        player.OnEndBattle();
        foreach (Enemy e in currentEnemies)
        {
            if (e != null)
            {
                e.OnEndBattle();
            }
        }

        currentEnemies.Clear();
    }

    public void PlayerAttackEnemy()
    {
        bool attackLanded = playerStats.AttackOpponent(currentEnemies[0].stats);
        if (attackLanded)
        {
            HandleLandedAttack(currentEnemies[0].transform, player.AttackParticle, player.AttackParticleLength);
            if (sfxControl != null)
            {
                sfxControl.PlayerHit();
            }
        }
    }

    public void PlayerThrowBomb()
    {
        currentEnemies[0].stats.ReceiveDamage(100); //TODO nicht hard coden!
        HandleLandedAttack(currentEnemies[0].transform, player.BombParticle, player.BombParticleLength);

        if (sfxControl != null)
        {
            sfxControl.Bomb();
        }
    }

    public void PlayerTryFleeBattle()
    {
        if (!currentEnemies[0].playerCanFlee)
        {
            return;
        }

        float fleeRand = 1;
        fleeRand = Random.value;
        if (fleeRand < currentEnemies[0].playerFleeProbability)
        {
            EndBattle();

            if (sfxControl != null)
            {
                sfxControl.Flee();
            }
        }
        else
        {
            player.stats.currentTurnTime = 0;

            if (sfxControl != null)
            {
                sfxControl.FailFlee();
            }
        }
    }

    public void PlayerAttackEnemyNoDodging()
    {
        bool attackLanded = playerStats.AttackOpponent(currentEnemies[0].stats, false, true);
        if (attackLanded)
        {
            HandleLandedAttack(currentEnemies[0].transform, player.AttackParticle, player.AttackParticleLength);
        }
    }

    public void PlayerChargeForBoost()
    {
        playerStats.UseChargeForDamageBoost();
        HandleCharging(player.transform, player.ChargeParticle, player.ChargeParticleLength);

        if (sfxControl != null)
        {
            sfxControl.PlayerCharge();
        }
    }

    public IEnumerator PlayerTeleport(TeleportToPosition teleport)
    {
        teleport.isInUse = true;
        if (black != null)
        {
            black.CrossFadeAlpha(1, teleport.TeleportTime / 2, true);
        }
        if (sfxControl != null)
        {
            sfxControl.Teleport();
        }
        yield return new WaitForSeconds(teleport.TeleportTime / 2);
        player.transform.position = teleport.TeleportTo.transform.position;

        if (black != null)
        {
            black.CrossFadeAlpha(0, teleport.TeleportTime / 2, true);
        }
        teleport.isInUse = false;

        playerEnteredBasement = !playerEnteredBasement;
        if (musicControl != null)
        {
            if (playerEnteredBasement)
            {
                musicControl.EnterBasement();
            }
            else
            {
                musicControl.LeaveBasement();
            }
        }
    }

    public void EndIntro()
    {
        if (introBg != null
            && introText != null)
        {
            introBg.CrossFadeAlpha(0, introFadeDuration, true);
            introText.CrossFadeAlpha(0, introFadeDuration, true);
        }
    }

    public void InvokeGameEnd(float endDuration, Light treasureChestLight)
    {
        StartCoroutine(EndGame(endDuration, treasureChestLight));
    }

    private IEnumerator EndGame(float endDuration, Light treasureChestLight)
    {
        gameEnded = true;

        if (sfxControl != null)
        {
            sfxControl.Desaster();
        }

        if (white != null
            && endText != null)
        {
            white.CrossFadeAlpha(1, endDuration, true);

            yield return new WaitForSeconds(endDuration);

            endText.CrossFadeAlpha(1, endDuration, true);
        }

        yield return new WaitForSeconds(endDuration * 2);

        if (NormalRoom != null
            && BrokenRoom != null)
        {
            NormalRoom.SetActive(false);
            BrokenRoom.SetActive(true);
        }
        if (treasureChestLight != null)
        {
            treasureChestLight.intensity = 0;
        }

        if (white != null)
        {
            white.CrossFadeAlpha(0, endDuration / 2, true);
        }

        yield return new WaitForSeconds(endDuration);

        if (black != null)
        {
            black.CrossFadeAlpha(1, endDuration, true);
        }

        yield return new WaitForSeconds(endDuration / 2);

        if (endText != null)
        {
            endText.CrossFadeAlpha(0, endDuration / 2, true);
        }

        if (musicControl != null)
        {
            musicControl.InvokeGameEnd();
        }
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

    public void ReactToDodge(GameObject dodged, float duration)
    {
        StartCoroutine(HandleDodge(dodged, duration));
    }

    private IEnumerator HandleDodge(GameObject dodged, float duration)
    {
        if (dodged != null)
        {
            Image dodgeImg = dodged.GetComponent<Image>();
            if (dodgeImg != null)
            {
                dodged.SetActive(true);
                yield return new WaitForSeconds(duration / 2);
                dodgeImg.CrossFadeAlpha(0, duration / 2, true);
                yield return new WaitForSeconds(duration / 2);
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

    public IEnumerator SpawnParticlesAtPosition(Vector3 pos, GameObject particles, float particleTime, Vector3 addHeight = new Vector3())
    {
        if (particles != null)
        {
            GameObject spawnedObject = GameObject.Instantiate(particles, pos + addHeight, particles.transform.rotation);
            yield return new WaitForSeconds(particleTime);
            GameObject.Destroy(spawnedObject);
        }
    }

    public void HandleLandedAttack(Transform hit, GameObject particles, float particleTime)
    {
        Vector3 pos = hit.position + new Vector3(0, 1, 0);
        StartCoroutine(SpawnParticlesAtPosition(pos, particles, particleTime));
    }

    public void HandleBombExplosion(Transform hit, GameObject particles, float particleTime)
    {
        Vector3 pos = hit.position + new Vector3(0, 1, 0);
        StartCoroutine(SpawnParticlesAtPosition(pos, particles, particleTime));
    }

    public void HandleCharging(Transform charger, GameObject particles, float particleTime, Vector3 addHeight = new Vector3())
    {
        StartCoroutine(SpawnParticlesAtPosition(charger.position, particles, particleTime, addHeight));
    }

    public void HandleDeath(Transform deadActor, GameObject particles, float particleTime, Vector3 addHeight = new Vector3())
    {
        StartCoroutine(SpawnParticlesAtPosition(deadActor.position, particles, particleTime, addHeight));
    }

    public InventoryUI GetInventoryUI()
    {
        return inventoryUI.GetComponent<InventoryUI>();
    }

    public SoundEffectControl GetSFXControl()
    {
        return sfxControl;
    }

    #endregion Implementation IGameController
}

public interface IGameController
{
    PlayerStatsClass GetPlayerStats();
    List<Enemy> GetCurrentEnemies();
    bool IsInBattle();
    void ReactToDodge(GameObject dodged, float duration);
    void HandlePlayerDeath();
    bool TakesPartInCurrentBattle(EnemyStatsClass enemy);
    bool TakesPartInCurrentBattle(Enemy enemy);
    void PlayerThrowBomb();
    IEnumerator SpawnParticlesAtPosition(Vector3 pos, GameObject particles, float particleTime, Vector3 addHeight = new Vector3());
    void HandleLandedAttack(Transform hit, GameObject particles, float particleTime);
    void HandleBombExplosion(Transform hit, GameObject particles, float particleTime);
    void HandleCharging(Transform charger, GameObject particles, float particleTime, Vector3 addHeight = new Vector3());
    void HandleDeath(Transform deadFighter, GameObject particles, float particleTime, Vector3 addHeight = new Vector3());
    InventoryUI GetInventoryUI();
    SoundEffectControl GetSFXControl();
}
