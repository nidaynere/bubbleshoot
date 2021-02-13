using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] GameBall gameBall;
    [SerializeField] int poolSize;

    private Pool pool;
    public void Start()
    {
        pool = new Pool(holder, gameBall, 100);
    }

    public void CreateGame()
    {
        
    }
}
