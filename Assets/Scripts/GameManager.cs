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
    [SerializeField] private GamePlayEvents gamePlayEvents;
    [SerializeField] private AnimationSettings animationSettings;
    [SerializeField] private BubbleGameSettings gameSettings;
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

        shooter.OnShoot += UserShoot;
        shooter.OnAim += UserAim;
    }

    [SerializeField] private Transform debugger;

    private void UserAim (Vector3[] positions)
    {
        int length = positions.Length;
        if (length == 0)
            return;

        var endPosition = FixEndPoint(positions, length);

        debugger.position = endPosition;

        endPosition = holder.InverseTransformPoint(endPosition);
        endPosition.y *= -1;

        int x = Mathf.RoundToInt(endPosition.x);
        int y = Mathf.RoundToInt(endPosition.y);

        Vector result;
        if (currentSession.GameEvents.RequestAvailablePosition(x, y, out result))
        {
            debugger.localPosition = new Vector3(result.X, -result.Y);
        }
    }

    private Vector3 FixEndPoint (Vector3[] positions, int length)
    {
        var endPosition = positions[length - 1];
        var direciton = (endPosition - positions[length - 2]).normalized;
        endPosition -= direciton;

        // clamp by walls.
        endPosition.x = Mathf.Clamp(endPosition.x, wallXStart.position.x + 1, wallXEnd.position.x - 1);
        endPosition.y = Mathf.Clamp(endPosition.y, wallYStart.position.y + 1, wallYEnd.position.y - 1);
        //
        return endPosition;
    }

    private void UserShoot(Vector3[] positions)
    {
        int length = positions.Length;
        if (length == 0)
            return;

        gamePlayEvents.OnGameplayStatusChange?.Invoke(false);

        var endPosition = FixEndPoint(positions, length);

        activeBall.Move(positions, animationSettings.BubbleThrowSpeed, () => {
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
            var result = currentSession.GameEvents.RequestPutBubble(x, y);

            if (!result)
            {
                Debug.LogError("Bubble game doesnt accept our position replacement. Try again or contact with the developer at freak99@gmail.com");

                activeBall.Move(activeBallPoint.position, animationSettings.BubbleThrowSpeed, () =>
                {
                    gamePlayEvents.OnGameplayStatusChange?.Invoke(true);
                });
            }
        });
    }

    private BubbleGame currentSession;

    private AnimationQuery animationQuery;

    public void CreateGame()
    {
        Clear();

        animationQuery = new AnimationQuery(animationSettings);
        currentSession = new BubbleGame(gameSettings.GridSizeX,
            gameSettings.GridSizeY,
            gameSettings.StartingRowCount,
            gameSettings.RowsAtPerTurn,
            gameSettings.RowCrowdness);

        // Register outputs to the game.
        currentSession.GameEvents.OnActiveBallCreated += ActiveBallCreated;
        currentSession.GameEvents.OnBubbleCombined += BubbleCombined;
        currentSession.GameEvents.OnBubbleMixed += BubbleMixed;
        currentSession.GameEvents.OnBubbleIsNowFree += BubbleIsNowFree;
        currentSession.GameEvents.OnBubblePositionUpdate += BubblePositionUpdate;
        currentSession.GameEvents.OnBubblePlacement += BubblePlacement;
        currentSession.GameEvents.OnBubbleSpawned += BubbleSpawned;
        currentSession.GameEvents.OnBubbleValueUpdate += BubbleValueUpdate;
        currentSession.GameEvents.OnNextBallSpawned += NextBallSpawned;
        currentSession.GameEvents.OnNextBallBecomeActive += NextBallBecomeActive;
        currentSession.GameEvents.OnReadyForVisualization += ReadyForVisualization;
        currentSession.GameEvents.OnBubbleExploded += BubbleExploded;

        // Bubble<-->GamePlayEvents
        currentSession.GameEvents.OnGameScoreUpdate += (int value) => { gamePlayEvents.OnScoreUpdate?.Invoke(value); };
        currentSession.GameEvents.OnGameFinished += (int value) => {
            Debug.Log("OnGameFinished()");
            gamePlayEvents.OnGameplayStatusChange?.Invoke(false);
            gamePlayEvents.OnGameOver?.Invoke(value); 
        };
        //

        gamePlayEvents.OnGameStarted?.Invoke();

        currentSession.NextTurn();

        gamePlayEvents.OnGameplayStatusChange?.Invoke(true);
    }

    public void Clear()
    {
        if (currentSession != null)
        { /// Clear old game.
            StopAllCoroutines();

            foreach (var obj in spawneds)
                obj.Value.gameObject.SetActive(false);

            spawneds.Clear();

            currentSession.Dispose();

            currentSession = null;
        }
    }

    private void ActiveBallCreated (Bubble bubble)
    {
        Debug.Log("Active ball created => " + bubble.Id + " " + bubble.Numberos);
        var pos = activeBallPoint.localPosition;
        activeBall = SpawnBall(bubble, pos, 1);
    }

    private void BubbleSpawned (Bubble bubble, int X, int Y)
    {
        SpawnBall(bubble, X, Y, 1);
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
        Debug.Log("Bubble {"+ Id +"} combined with Id => " +tId);

        animationQuery.AddToQuery(
            new AnimationQuery.CombineAction(spawneds[Id], spawneds[tId])
            );
    }

    private void BubbleMixed(ushort Id, ushort tId)
    {
        Debug.Log("Bubble {" + Id + "} mixed with Id => " + tId);

        animationQuery.AddToQuery(
            new AnimationQuery.MixAction(spawneds[Id], spawneds[tId])
            );
    }

    private void ReadyForVisualization()
    {
        Debug.Log("Ready for visualization.");
        StartCoroutine(animationQuery.DoQuery(() => {
            Debug.Log("Visualization completed.");

            gamePlayEvents.OnGameplayStatusChange?.Invoke(true);
            Debug.Log("next round !!");

            //currentSession.CreateRows(1, 80, false);
            currentSession.NextTurn();
        }));
    }

    private void BubbleIsNowFree (ushort Id)
    {
        // todo fly away animation
        Debug.Log("Bubble is now free, fly away => " + Id);
    }

    private void BubbleExploded (int X, int Y, ushort[] Ids)
    {
        // add effect this position.
        // shake the screen.

        Debug.Log("Bubble exploded at => " + X +  ", " + Y);
    }

    private void BubblePositionUpdate (ushort Id, int X, int Y, bool IsInstant)
    {
        if (spawneds.ContainsKey(Id))
        {
            if (IsInstant)
                spawneds[Id].SetPosition(X, Y);
            else
            {
                spawneds[Id].SetTransition(X, Y);
            }
        }

        Debug.Log("Bubble position update => " + Id + " X="+ X + " Y=" + Y);
    }

    private void BubblePlacement(ushort Id, int X, int Y)
    {
        animationQuery.AddToQuery(
            new AnimationQuery.PlacementAction(spawneds[Id], X, Y)
        );

        Debug.Log("Bubble placement => " + Id + " X=" + X + " Y=" + Y);
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
        activeBall.Move(activeBallPoint.position, animationSettings.PositionUpdateSpeed);
        activeBall.Scale(Vector3.one * 1);

        Debug.Log("Next ball become active!");
    }
}
