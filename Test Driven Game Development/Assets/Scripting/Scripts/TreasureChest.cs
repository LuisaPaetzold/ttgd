using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public Player player;
    public GameController gameCtr;
    public float openDistance = 3;
    public float maxLightIntensity = 30;
    public float lightIncreaseSpeed = 3.5f;
    public bool isOpen;
    public Animator animator;
    public IUnityStaticService staticService;

    public Light flareLight;

    void Start ()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (gameCtr == null)
        {
            gameCtr = FindObjectOfType<GameController>();
        }

        animator = GetComponent<Animator>();

        flareLight = GetComponentInChildren<Light>();
        if (flareLight != null)
        {
            flareLight.intensity = 0;
        }

        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }
    }

    // Spiel endet nach Abschluss der Animation -> Blackscreen && Thank you for playing canvas




    void Update()
    {
        if (player != null)
        {
            if (!isOpen)
            {
                float dist = player.GetDistanceToPlayer(this.transform.position);
                if (dist < openDistance)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("OpenChest");
                    }

                    isOpen = true;
                }
            }
            else
            {
                if (flareLight != null)
                {
                    if (flareLight.intensity < maxLightIntensity)
                    {
                        flareLight.intensity += staticService.GetDeltaTime() * lightIncreaseSpeed;
                        flareLight.intensity = Mathf.Min(maxLightIntensity, flareLight.intensity);
                    }
                    else
                    {
                        if (gameCtr != null
                            && !gameCtr.gameEnded)
                        {
                            gameCtr.InvokeGameEnd(2, flareLight);
                        }
                    }
                }
            }
        }
    }
}
