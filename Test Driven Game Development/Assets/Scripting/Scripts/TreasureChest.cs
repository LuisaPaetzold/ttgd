using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public Player player;
    public float openDistance = 3;
    public bool isOpen;
    public Animator animator;

    void Start ()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        animator = GetComponent<Animator>();
    }

    // Spiel endet nach Abschluss der Animation -> Blackscreen && Thank you for playing canvas




    void Update()
    {
        if (player != null
            && !isOpen)
        {
            float dist = player.GetDistanceToPlayer(this.transform.position);
            if (dist < openDistance)
            {
                if (animator != null)
                {
                    animator.SetTrigger("OpenChest");
                }
                
                isOpen = true;
                Debug.Log("Chest opened!");

                // light intensity over time erhöhen!
            }
        }
    }
}
