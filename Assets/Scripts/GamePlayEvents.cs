using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GamePlayEvents", menuName = "GamePlayEvents", order = 1)]
public class GamePlayEvents : ScriptableObject
{
    public Action<bool> OnGameplayStatusChange;
}

