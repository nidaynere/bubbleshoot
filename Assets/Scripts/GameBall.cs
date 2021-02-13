using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bob;
using UnityEngine.UI;

public class GameBall : MonoBehaviour
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
            text.text = _bubble.Numberos.ToString().Replace ("_","");
            meshRenderer.sharedMaterial = ballColors.Materials[(int)_bubble.Numberos];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
