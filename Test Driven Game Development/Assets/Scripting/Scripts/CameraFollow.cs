using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Player PlayerObject;
    public float xDistance = 3;
    public float yDistance = 3;
    public float zDistance = 3;

    private GameController gameCtr;

    // Use this for initialization
    void Start ()
    {
		if (PlayerObject == null)
        {
            PlayerObject = FindObjectOfType<Player>();
        }
        if (gameCtr == null)
        {
            gameCtr = FindObjectOfType<GameController>();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!gameCtr.IsInBattle())
        {
            transform.position = new Vector3(
            PlayerObject.transform.position.x + xDistance,
            PlayerObject.transform.position.y + yDistance,
            PlayerObject.transform.position.z + zDistance);
        }
	}
}
