using UnityEngine;
using Bob;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform holder;
    [SerializeField] private GameBall gameBall;
    [SerializeField] private int poolSize;
    [SerializeField] private Shooter shooter;
    [SerializeField] private int width, height;
    [SerializeField] private GamePlayEvents GamePlayEvents;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Transform activeBallPoint, nextBallPoint;
    [SerializeField] private Transform wallXStart, wallXEnd, wallYStart, wallYEnd;

    private GameBall activeBall, nextBall;

    private Dictionary<ushort, GameBall> spawneds = new Dictionary<ushort, GameBall>();

    private Pool pool;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (currentSession == null)
            return;
        var grid = currentSession.GetMap.GetGrid;
        int length = grid.Length;
        const float size = 50;
        for (int i = 0; i < length; i++)
        {
            int xSize = grid[i].Length;

            for (int x = 0; x < xSize; x++)
            {
                var obj = grid[i][x];
                GUI.color = obj == null ? Color.green : Color.red;
                GUI.Box(new Rect(x * size, i * size, size, size), obj != null ? obj.Id.ToString () : "");
            }
        }
    }
#endif


    public void Start()
    {
        pool = new Pool(holder, gameBall, poolSize);

        shooter.RegisterShootEvent(UserShoot);
    }
    private void UserShoot(Vector3[] positions)
    {
        int length = positions.Length;
        if (length == 0)
            return;

        GamePlayEvents.OnGameplayStatusChange?.Invoke(false);

        var endPosition = positions[length - 1];
        var direciton = (endPosition - positions[length - 2]).normalized;
        endPosition -= direciton;

        Debug.Log(endPosition);
        // clamp by walls.
        endPosition.x = Mathf.Clamp(endPosition.x, wallXStart.position.x+1, wallXEnd.position.x-1);
        endPosition.y = Mathf.Clamp(endPosition.y, wallYStart.position.y+1, wallYEnd.position.y-1);
        //
        Debug.Log("clamped " + endPosition);

        activeBall.Move(positions, gameSettings.BubbleThrowSpeed, () => {
            /*
             * Convert this position to Bubble grid.
             * */
            endPosition = holder.InverseTransformPoint(endPosition);
            endPosition.y *= -1;
            /*
             * */
            
            int x = Mathf.RoundToInt(endPosition.x);
            int y = Mathf.RoundToInt(endPosition.y);

            // Trigger game.
            var result = currentSession.GameEvents.RequestPutBubble(x, y, true);

            if (!result)
            {
                Debug.LogError("Bubble game doesnt accept our position replacement. Try again or contact with the developer at freak99@gmail.com");

                activeBall.Move(activeBallPoint.position, gameSettings.BubbleThrowSpeed, () =>
                {
                    GamePlayEvents.OnGameplayStatusChange?.Invoke(true);
                });
            }
            else
            {
                GamePlayEvents.OnGameplayStatusChange?.Invoke(true);
                Debug.Log("next round !!");
                
                //currentSession.CreateRows(1, 80, false);
                currentSession.NextTurn();
            }
        });
    }

    private BubbleGame currentSession;
    public void CreateGame()
    {
        currentSession = new BubbleGame(width, height);

        // Register outputs to the game.
        currentSession.GameEvents.OnActiveBallCreated += ActiveBallCreated;
        currentSession.GameEvents.OnBubbleCombined += BubbleCombined;
        currentSession.GameEvents.OnBubbleMixed += BubbleMixed;
        currentSession.GameEvents.OnBubbleIsNowFree += BubbleIsNowFree;
        currentSession.GameEvents.OnBubblePositionUpdate += BubblePositionUpdate;
        currentSession.GameEvents.OnBubbleSpawned += BubbleSpawned;
        currentSession.GameEvents.OnBubbleValueUpdate += BubbleValueUpdate;
        currentSession.GameEvents.OnNextBallSpawned += NextBallSpawned;
        currentSession.GameEvents.OnNextBallBecomeActive += NextBallBecomeActive;
        //

        currentSession.CreateRows(2, 40, true);
        currentSession.NextTurn();

        GamePlayEvents.OnGameplayStatusChange?.Invoke(true);
    }

    private void ActiveBallCreated (Bubble bubble)
    {
        Debug.Log("Active ball created => " + bubble.Id + " " + bubble.Numberos);
        var pos = activeBallPoint.localPosition;
        activeBall = SpawnBall(bubble, pos, 0.8f);
    }

    private void BubbleSpawned (Bubble bubble, int X, int Y)
    {
        SpawnBall(bubble, X, Y, 0.8f);
    }

    private GameBall SpawnBall(Bubble bubble, int X, int Y, float scale = 1)
    {
        var gameBall = pool.Get();

        gameBall.SetPosition(X, Y);

        gameBall.transform.localScale = Vector3.one * scale;
        gameBall.bubble = bubble;

        gameBall.gameObject.SetActive(true);

        spawneds.Add(bubble.Id, gameBall);

        return gameBall;
    }

    private GameBall SpawnBall(Bubble bubble, Vector3 pos, float scale = 1)
    {
        var gameBall = SpawnBall(bubble, 0, 0, scale);
        gameBall.transform.localPosition = pos;
        return gameBall;
    }

    private void BubbleCombined (ushort Id, ushort tId)
    {
        Debug.Log("Bubble {"+ Id +"} combined with Id => " +Id);
    }

    private void BubbleMixed(ushort Id, ushort tId)
    {
        Debug.Log("Bubble {" + Id + "} mixed with Id => " + Id);
    }

    private void BubbleIsNowFree (ushort Id)
    {
        Debug.Log("Bubble is now free, fly away => " + Id);
    } 

    private void BubblePositionUpdate (ushort Id, int X, int Y, bool IsInstant)
    {
        Debug.Log("Ball posiiton update at " + Id + " is instant: " + IsInstant);
        if (spawneds.ContainsKey(Id))
        {
            if (IsInstant)
                spawneds[Id].SetPosition(X, Y);
            else spawneds[Id].SetTransition(X, Y);
        }

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
        nextBall = SpawnBall (bubble, pos, 0.5f);
    }

    private void NextBallBecomeActive ()
    {
        activeBall = nextBall;
        activeBall.Move(activeBallPoint.position, gameSettings.PositionUpdateSpeed);
        activeBall.Scale(Vector3.one * 0.8f);

        Debug.Log("Next ball become active!");
    }
}
