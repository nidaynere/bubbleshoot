using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public AnimationCurve TransitionCurve;
    public AnimationCurve ScaleCurve;

    public float BubbleThrowSpeed = 10f;
    public float PositionUpdateSpeed = 1f;
    public float CombineAnimationDelay = 0.2f;
    public float ScaleUpdateSpeed = 1f;
}
