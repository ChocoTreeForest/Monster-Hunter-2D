using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStatus", menuName = "GameStatus")]
public class GameStatus : ScriptableObject
{
    public bool questClear = false;
    public bool returnToVillage = false;
}
