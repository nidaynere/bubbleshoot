using UnityEngine;
using Bob;
using UnityEngine.UI;
using System;

public class GameBall : Transition
{
    [SerializeField] private BallColors ballColors;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Text text;
    [SerializeField] private Transform visual;

    private int posX, posY;

    public void SetPosition(int X, int Y)
    {
        posX = X;
        posY = Y;

        transform.localPosition = new Vector3(X, -Y);
        visual.localPosition = new Vector3 (0, X % 2 * 0.1f);
    }

    public void SetTransition (int X, int Y, Action onCompleted = null)
    {
        posX = X;
        posY = Y;
        var tPos = transform.parent.TransformPoint (new Vector3(X, -Y));

        Move(tPos, gameSettings.PositionUpdateSpeed);
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
            meshRenderer.sharedMaterial = ballColors.Materials[(int)_bubble.Numberos];
        }
    }

    public void Kill()
    {
        //TODO anim.
        gameObject.SetActive(false);
    }

    public void Upgrade()
    {
        text.text = _bubble.Numberos.ToString().Replace("_", "");

        // TODO animation.
    }
}
