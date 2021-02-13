using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public AnimationCurve TransitionCurve;
    public float BubbleThrowSpeed = 10f;
    public float PositionUpdateSpeed = 1f;
}
