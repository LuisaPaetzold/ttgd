using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipIntro : MonoBehaviour
{
    public Player player;
    public GameController gameCtr;
    Animation anim;
    Transform oldPlayerParent;
    BoxCollider[] colliders;
    CharacterController charContr;
    BoxCollider playerColl;

    public bool animFinished;

    void Start()
    {
        player = FindObjectOfType<Player>();
        gameCtr = FindObjectOfType<GameController>();

        if (player != null)
        {
            oldPlayerParent = player.transform.parent;
            player.transform.SetParent(this.transform);
            charContr = player.GetComponent<CharacterController>();
            if (charContr != null)
            {
                charContr.enabled = false;
            }
            playerColl = player.GetComponent<BoxCollider>();
            if (playerColl != null)
            {
                playerColl.enabled = false;
            }
        }

        anim = GetComponent<Animation>();

        colliders = GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider bc in colliders)
        {
            bc.enabled = false;
        }
    }

    void Update ()
    {
        if (anim != null
            && !anim.isPlaying)
        {
            animFinished = true;
        }

        if (animFinished
            && player != null
            && gameCtr != null
            && gameCtr.introPlaying)
        {
            gameCtr.introPlaying = false;
            
            if (charContr != null)
            {
                player.GetComponent<CharacterController>().enabled = true;
            }
            if (charContr != null)
            {
                player.GetComponent<BoxCollider>().enabled = true;
            }

            player.transform.SetParent(oldPlayerParent);

            foreach (BoxCollider bc in colliders)
            {
                bc.enabled = true;
            }

            gameCtr.EndIntro();
        }
	}
}
