using UnityEngine;
using Bob;
using System;

public class GameBall : Transition
{
#pragma warning disable CS0649
    [SerializeField] private BallColors ballColors;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Transform visual;

    #region physics
    [SerializeField] private Rigidbody p_rigidbody;
    [SerializeField] private Collider p_collider;
    [SerializeField] private Vector2 p_flyForce;
    #endregion
#pragma warning restore CS0649

    private int posX, posY;

    public void SetPosition(int X, int Y)
    {
        posX = X;
        posY = Y;

        transform.localPosition = new Vector3(X, -Y);
        visual.localPosition = new Vector3 (0, X % 2 == 1 ? 0.1f : -0.1f);
    }

    public void SetTransition (int X, int Y, Action onCompleted = null)
    {
        posX = X;
        posY = Y;
        var tPos = transform.parent.TransformPoint (new Vector3(X, -Y));

        Move(tPos, animationSettings.PositionUpdateSpeed, onCompleted);
    }

    public void GetPosition (out int X, out int Y)
    {
        X = posX;
        Y = posY;
    }

    private Bubble _bubble;
    public Bubble bubble
    {
        get
        {
            return _bubble;
        }

        set
        {
            _bubble = value;
            Upgrade();
        }
    }

    public void Kill()
    {
        Scale(Vector3.zero, animationSettings.ScaleUpdateSpeed,  () => { gameObject.SetActive(false); });
    }

    public void Upgrade()
    {
        meshRenderer.sharedMaterial = ballColors.Materials[(int)_bubble.Numberos];

        var scale = transform.localScale;
        Scale(scale * 1.5f, animationSettings.ScaleUpdateSpeed, 
            () => { Scale(scale, animationSettings.ScaleUpdateSpeed); });
    }

    public void SetCollider(bool value)
    {
        p_collider.enabled = value;
    }

    public void SetRigidbody(bool value)
    {
        p_rigidbody.isKinematic = !value;
    }

    public void FlyAway ()
    {
        SetCollider(false);
        SetRigidbody(true);

        p_rigidbody.AddForce(new Vector3(
            UnityEngine.Random.Range(-p_flyForce.x, p_flyForce.x),
            p_flyForce.y));

        Scale(Vector3.zero, animationSettings.FlyAwayScaleSpeed, () => { gameObject.SetActive(false); });
    }
}
