using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToPosition : MonoBehaviour
{
    public GameObject TeleportTo;
    public float TeleportTime = 1f;

    public GameController GameCtr;
    internal bool isInUse;

    private void Start()
    {
        GameCtr = FindObjectOfType<GameController>();
    }

    public void PlayerTeleport()
    {
        if (GameCtr != null && TeleportTo != null && CanTeleport())
        {
            StartCoroutine(GameCtr.PlayerTeleport(this));
        }
    }

    public bool CanTeleport()
    {
        return !isInUse;
    }
}
