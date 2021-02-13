using UnityEngine;
using Bob;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] GameBall gameBall;
    [SerializeField] int poolSize;
    [SerializeField] Shooter shooter;

    private Pool pool;
    public void Start()
    {
        pool = new Pool(holder, gameBall, 100);
    }

    private BubbleGame currentSession;
    public void CreateGame()
    {
        currentSession = new BubbleGame(12, 8);

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

        currentSession.NextTurn();
    }

    private void ActiveBallCreated (Bubble bubble)
    {
        Debug.Log("Active ball created => " + bubble.Id + " " + bubble.Numberos);
    }

    private void BubbleSpawned (Bubble bubble, int X, int Y)
    {
        Debug.Log("Bubble spawned => " + bubble.Id + " at " + X + "," + Y);
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
        Debug.Log("Next ball spawned => " + bubble.Id +" " + bubble.Numberos);
    }

    private void NextBallBecomeActive ()
    {
        Debug.Log("Next ball become active!");
    }
}
