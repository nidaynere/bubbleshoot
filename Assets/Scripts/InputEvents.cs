using UnityEngine;
using System;

[CreateAssetMenu(fileName = "InputEvents", menuName = "InputEvents", order = 1)]
public class InputEvents : ScriptableObject
{
    public Action<bool> OnGameplayStatusChange;
}

