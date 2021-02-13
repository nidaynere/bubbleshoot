using UnityEngine;
using Bob;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] GameBall gameBall;
    [SerializeField] int poolSize;
    [SerializeField] Shooter shooter;
    [SerializeField] int width, height;
    [SerializeField] InputEvents inputEvents;
    [SerializeField] GameSettings gameSettings;

    [SerializeField] Transform activeBallPoint, nextBallPoint;

    private GameBall activeBall, nextBall;

    private Pool pool;

    public void Start()
    {
        pool = new Pool(holder, gameBall, poolSize);

        shooter.RegisterShootEvent(UserShoot);
    }
    private void UserShoot(Vector3[] positions)
    {
        inputEvents.OnGameplayStatusChange?.Invoke(false);

        int length = positions.Length;
        if (length == 0)
            return;

        Vector3 lastPosition = positions[length-1];

        activeBall.Move(positions, gameSettings.BubbleThrowSpeed, () => {
            Debug.Log("transition completed");
            inputEvents.OnGameplayStatusChange?.Invoke(true);
        });
    }

    private BubbleGame currentSession;
    public void CreateGame()
    {
        currentSession = new BubbleGame(width, height);

        // Register outputs to the game.
        currentSession.GameEvents.OnActiveBallCreated += ActiveBallCreated;
        currentSession.GameEvents.OnBubbleDestroyed += BubbleDestroyed;
        currentSession.GameEvents.OnBubbleIsNowFree += BubbleIsNowFree;
        currentSession.GameEvents.OnBubblePositionUpdate += BubblePositionUpdate;
        currentSession.GameEvents.OnBubbleSpawned += BubbleSpawned;
        currentSession.GameEvents.OnBubbleValueUpdate += BubbleValueUpdate;
        currentSession.GameEvents.OnNextBallSpawned += NextBallSpawned;
        currentSession.GameEvents.OnNextBallBecomeActive += NextBallBecomeActive;
        //

        currentSession.CreateRows(2, 40);
        currentSession.NextTurn();

        inputEvents.OnGameplayStatusChange?.Invoke(true);
    }

    private void ActiveBallCreated (Bubble bubble)
    {
        Debug.Log("Active ball created => " + bubble.Id + " " + bubble.Numberos);
        var pos = activeBallPoint.localPosition;
        int X = (int)pos.x;
        int Y = -(int)pos.y;

        activeBall = SpawnBall(bubble, X, Y, 0.8f);
    }

    private void BubbleSpawned (Bubble bubble, int X, int Y)
    {
        Debug.Log("Bubble spawned => " + bubble.Id + " at " + X + "," + Y);

        SpawnBall(bubble, X, Y + (X%2) * 0.1f, 0.8f);
    }

    private GameBall SpawnBall(Bubble bubble, float X, float Y, float scale = 1)
    {
        var gameBall = pool.Get();
        gameBall.transform.localPosition = new Vector3(X, -Y, 0);
        gameBall.transform.localScale = Vector3.one * scale;
        gameBall.bubble = bubble;

        gameBall.gameObject.SetActive(true);

        return gameBall;
    }

    private void BubbleDestroyed (ushort Id)
    {
        Debug.Log("Bubble destroyed with Id => " +Id);
    }

    private void BubbleIsNowFree (ushort Id)
    {
        Debug.Log("Bubble is now free, fly away => " + Id);
    } 

    private void BubblePositionUpdate (ushort Id, int X, int Y)
    {
        Debug.Log("Bubble position update => " + Id + " X="+ X + " Y=" + Y);
    }

    private void BubbleValueUpdate (ushort Id, Bubble.BubbleType newType)
    {
        Debug.Log("BubbleValue update => " + Id + ", " + newType);
    }

    private void NextBallSpawned (Bubble bubble)
    {
        Debug.Log("Next ball spawned => " + bubble.Id + " " + bubble.Numberos);
        var pos = nextBallPoint.localPosition;
        int X = (int)pos.x;
        int Y = -(int)pos.y;
        nextBall = SpawnBall (bubble, X, Y, 0.5f);
    }

    private void NextBallBecomeActive ()
    {
        activeBall = nextBall;
        Debug.Log("Next ball become active!");
    }
}
