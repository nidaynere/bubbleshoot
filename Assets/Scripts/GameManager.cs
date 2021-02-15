using UnityEngine;
using Bob;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
#pragma warning disable CS0649

    [SerializeField] private EffectPool effectPool;
    [SerializeField] private Transform holder;
    [SerializeField] private Renderer gridRenderer;
    [SerializeField] private GameBall gameBall;
    [SerializeField] private int poolSize;
    [SerializeField] private Shooter shooter;
    [SerializeField] private GamePlayEvents gamePlayEvents;
    [SerializeField] private AnimationSettings animationSettings;
    [SerializeField] private BubbleGameSettings gameSettings;
    [SerializeField] private Transform activeBallPoint, nextBallPoint;
    [SerializeField] private Transform wallXStart, wallXEnd, wallYStart, wallYEnd;
    [SerializeField] private Transform gridPointer;

#pragma warning restore CS0649

    private GameBall activeBall, nextBall;

    private Dictionary<ushort, GameBall> spawneds = new Dictionary<ushort, GameBall>();

    private Pool ballPool;

    private BubbleGame currentSession;

    private AnimationQuery animationQuery;

    private void Start()
    {
        ballPool = new Pool(holder, gameBall, poolSize);
        effectPool.Create(holder);

        shooter.OnShoot += UserShoot;
        shooter.OnAim += UserAim;

        gamePlayEvents.StartGame = CreateGame;
        gamePlayEvents.ClearGame = Clear;
    }

    private void CreateGame()
    {
        Clear();

        animationQuery = new AnimationQuery(animationSettings);
        currentSession = new BubbleGame(gameSettings.GridSizeX,
            gameSettings.GridSizeY,
            gameSettings.StartingRowCount,
            gameSettings.RowsAtPerTurn,
            gameSettings.RowCrowdness);

        gridRenderer.transform.localScale = new Vector3(gameSettings.GridSizeX, gameSettings.GridSizeY);
        gridRenderer.material.SetVector("_Tiling", gridRenderer.transform.localScale);

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

        gamePlayEvents.OnScoreUpdate?.Invoke(0);

        currentSession.NextTurn();

        gamePlayEvents.OnGameplayStatusChange?.Invoke(true);
    }

    private void Clear()
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


    private void UserAim(Vector3[] positions)
    {
        int length = positions.Length;
        if (length == 0)
            return;

        gridPointer.gameObject.SetActive(true);

        if (FixEndPoint(positions, length, out int x, out int y))
        {
            gridPointer.localPosition = new Vector3(x, -y);
        }
    }

    /// <summary>
    /// Takes the direction of the end position on the given positions. Sends it to the system, and get the proper point on 2d grid.
    /// </summary>
    /// <param name="positions">positions</param>
    /// <param name="length">length of positions</param>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    private bool FixEndPoint(Vector3[] positions, int length, out int X, out int Y)
    {
        var endPosition = positions[length - 1];
        var direciton = (endPosition - positions[length - 2]).normalized;
        endPosition -= direciton;

        // clamp by walls.
        endPosition.x = Mathf.Clamp(endPosition.x, wallXStart.position.x + 1, wallXEnd.position.x - 1);
        endPosition.y = Mathf.Clamp(endPosition.y, wallYStart.position.y + 1, wallYEnd.position.y - 1);
        //

        endPosition = holder.InverseTransformPoint(endPosition);
        endPosition.y *= -1;

        X = Mathf.RoundToInt(endPosition.x);
        Y = Mathf.RoundToInt(endPosition.y);

        Vector result;
        if (currentSession.GameEvents.RequestAvailablePosition(X, Y, out result))
        {
            X = result.X;
            Y = result.Y;

            return true;
        }

        return false;
    }

    private void UserShoot(Vector3[] positions)
    {
        gridPointer.gameObject.SetActive(false);

        int length = positions.Length;
        if (length == 0)
            return;

        gamePlayEvents.OnGameplayStatusChange?.Invoke(false);

        if (FixEndPoint(positions, length, out int x, out int y))
        {
            var fix = new Vector3(x, -y); // fix end of the position.

            positions[length - 1] = holder.TransformPoint(fix);

            activeBall.Move(positions, animationSettings.BubbleThrowSpeed, () =>
            {
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
        else
        {
            activeBall.Move(activeBallPoint.position, animationSettings.BubbleThrowSpeed, () =>
            {
                gamePlayEvents.OnGameplayStatusChange?.Invoke(true);
            });
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
        var gameBall = ballPool.Get();
        gameBall.SetRigidbody(false);
        gameBall.SetCollider(true);
        gameBall.SetPosition(X, Y);
        gameBall.gameObject.SetActive(true);
        gameBall.transform.localScale = Vector3.one * scale;
        gameBall.bubble = bubble;

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

        if (spawneds.ContainsKey(Id))
        {
            spawneds[Id].FlyAway();
            spawneds.Remove(Id);
        }
    }

    private void BubbleExploded (int X, int Y, ushort[] Ids)
    {
        // add effect this position.
        // shake the screen.

        effectPool.Play("BubbleExplode", new Vector3(X, -Y));

        int length = Ids.Length;
        for (int i = 0; i < length; i++)
        {
            if (spawneds.ContainsKey(Ids[i]))
            {
                spawneds[Ids[i]].Kill ();
                spawneds.Remove(Ids[i]);
            }
        }

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
        nextBall.SetCollider(false);
    }

    private void NextBallBecomeActive ()
    {
        activeBall = nextBall;
        activeBall.Move(activeBallPoint.position, animationSettings.PositionUpdateSpeed);
        activeBall.Scale(Vector3.one * 1, animationSettings.ScaleUpdateSpeed);
        activeBall.SetCollider(true);

        Debug.Log("Next ball become active!");
    }
}
