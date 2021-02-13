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
    [SerializeField] GamePlayEvents GamePlayEvents;
    [SerializeField] GameSettings gameSettings;

    [SerializeField] Transform activeBallPoint, nextBallPoint;

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

        var lastPosition = positions[length - 1];

        activeBall.Move(positions, gameSettings.BubbleThrowSpeed, () => {
            lastPosition = holder.InverseTransformPoint(lastPosition);
            lastPosition.y *= -1;
            
            int x = Mathf.RoundToInt(lastPosition.x);
            int y = Mathf.RoundToInt(lastPosition.y);

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

        currentSession.CreateRows(2, 40);
        currentSession.NextTurn();

        GamePlayEvents.OnGameplayStatusChange?.Invoke(true);
    }

    private void ActiveBallCreated (Bubble bubble)
    {
        Debug.Log("Active ball created => " + bubble.Id + " " + bubble.Numberos);
        var pos = activeBallPoint.localPosition;
        activeBall = SpawnBall(bubble, pos, 0.8f);
    }

    private Vector3 getGridPosition(int X, int Y)
    {
        return new Vector3(X, -Y + X % 2 * 0.1f);
    }


    private void BubbleSpawned (Bubble bubble, int X, int Y)
    {
        Debug.Log("Bubble spawned => " + bubble.Id + " at " + X + "," + Y);

        var pos = getGridPosition(X, Y);
        SpawnBall(bubble, pos, 0.8f);
    }

    private GameBall SpawnBall(Bubble bubble, Vector3 pos, float scale = 1)
    {
        var gameBall = pool.Get();
        gameBall.transform.localPosition = pos;
        gameBall.transform.localScale = Vector3.one * scale;
        gameBall.bubble = bubble;

        gameBall.gameObject.SetActive(true);

        spawneds.Add(bubble.Id, gameBall);

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

    private void BubblePositionUpdate (ushort Id, int X, int Y)
    {
        if (spawneds.ContainsKey(Id))
        {
            spawneds[Id].Move(holder.TransformPoint (getGridPosition(X, Y)), gameSettings.PositionUpdateSpeed);
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

        Debug.Log("Next ball become active!");
    }
}
