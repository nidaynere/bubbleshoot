using UnityEngine;
using Bob;
using UnityEngine.UI;

public class GameBall : Transition
{
    [SerializeField] private BallColors ballColors;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Text text;

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
