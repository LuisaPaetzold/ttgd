using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TTGD/Weapon")]
public class Weapon : ScriptableObject
{
    public int damage = 1;
    public int durability = 1;

    public Weapon()
    {

    }
}
