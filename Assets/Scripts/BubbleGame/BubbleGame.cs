using System;
using System.Collections.Generic;

namespace Bob
{
    public class BubbleGame : IDisposable
    {
#if UNITY_EDITOR
        public BubbleGrid GetMap => map;
#endif
        public BubbleEvents GameEvents;
        private BubbleGrid map;
        private ushort idCounter;

        private Bubble activeBubble;
        private Bubble nextBubble;

        private int startingRowCount;
        private int rowCountAtPerTurn;
        private int rowCrowdNess;

        private bool isStarted;

        private int userScore;

        /// <summary>
        /// Creates a bubble game.
        /// </summary>
        /// <param name="gridSizeX">X size of the grid</param>
        /// <param name="gridSizeY">Y size of the grid</param>
        /// <param name="startingRowCount">How many rows will be generated at start.</param>
        /// <param name="rowCountAtPerTurn">How many rows will be generated at a new turn</param>
        /// <param name="rowCrowdNess">How much crowdness rows should have. It's a randomizer. Range is between 0-100</param>
        public BubbleGame (int gridSizeX, int gridSizeY, int startingRowCount, int rowCountAtPerTurn, int rowCrowdNess)
        {
            #region define
            map = new BubbleGrid(new Vector (gridSizeX, gridSizeY));
            GameEvents = new BubbleEvents();
            #endregion

            this.startingRowCount = startingRowCount;
            this.rowCountAtPerTurn = rowCountAtPerTurn;
            this.rowCrowdNess = rowCrowdNess;

            GameEvents.RequestPutBubble += PutBubble;
            GameEvents.RequestAvailablePosition += AvailablePosition;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void NextTurn ()
        {
            if (!isStarted)
            {
                isStarted = true;
                CreateRows(startingRowCount, rowCrowdNess, true);
            }
            else
            {
                // Check for end game.
                Bubble[] bubbles = new Bubble[map.Size.X];
                Vector[] positions = new Vector[map.Size.X];
                int count = map.GetBubblesAtRow(map.Size.Y - 2, map.Size.X, ref bubbles, ref positions);
                if (count > 0)
                {
                    // game over:(
                    GameEvents.OnGameFinished?.Invoke(userScore);
                    return;
                }
                
                CreateRows(rowCountAtPerTurn, rowCrowdNess, false);   
            }

            if (activeBubble == null)
            {
                activeBubble = CreateBubble(ref idCounter);
                GameEvents.OnActiveBallCreated?.Invoke(activeBubble);

                OutputLog.AddLog("[BubbleGame] Active bubble Id => " + activeBubble.Id);
            }
            else
            {
                activeBubble = nextBubble;

                OutputLog.AddLog("[BubbleGame] Active bubble Id => " + activeBubble.Id);

                GameEvents.OnNextBallBecomeActive?.Invoke();
            }

            nextBubble = CreateBubble(ref idCounter);
            GameEvents.OnNextBallSpawned?.Invoke(nextBubble);
        }

        /// <summary>
        /// Create rows as much as the given count.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="fillChance">Fill chance should be between 0 and 100. 100 Means always create points inside row.</param>
        public void CreateRows(int count, int fillChance, bool isInstant)
        {
            int mapSizeX = map.Size.X;
            int mapSizeY = map.Size.Y;
            
            count = Math.Min(count, mapSizeY);

            Bubble[] f_bubbles = new Bubble[mapSizeX];
            Vector[] f_positions = new Vector[mapSizeX];

            // update bubbles at index 1;
            for (int i = 0; i < count; i++)
            {
                Bubble[] bubbles = new Bubble[mapSizeX];

                for (int x = 0; x < mapSizeX; x++)
                {
                    var randomizer = Random.Range(0, 100);

                    if (randomizer <= fillChance)
                    {
                        bubbles[x] = CreateBubble(ref idCounter);
                        GameEvents.OnBubbleSpawned?.Invoke(bubbles[x], x, 0);
                    }
                }

                map.AddBubbles(bubbles);

                for (int y = 1; y < mapSizeY; y++)
                {
                    int f_count = map.GetBubblesAtRow(y, mapSizeX, ref f_bubbles, ref f_positions);
                    for (int f = 0; f < f_count; f++)
                    {
                        GameEvents.OnBubblePositionUpdate?.Invoke(f_bubbles[f].Id, f_positions[f].X, f_positions[f].Y, isInstant);
                    }
                }
            }
        }

        private Bubble CreateBubble (ref ushort counter)
        {
            var bubble = new Bubble(++counter);
            return bubble;
        }

        private void AddScore (Bubble.BubbleType type)
        {
            userScore += (int)type + 1;
            GameEvents.OnGameScoreUpdate?.Invoke(userScore);
        }

        private bool AvailablePosition(int X, int Y, out Vector position)
        {
            position = new Vector(X, Y);

            if (!map.IsPositionAvailable(position))
            {
                if (!map.FindClosePosition(position, ref position))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// put active ball to a position and let the grid run the algorithm.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private bool PutBubble(int X, int Y)
        {
            Vector position;
            if (!AvailablePosition(X, Y, out position))
            {
                return false;
            }

            map.AddToPosition(activeBubble, position);
            GameEvents.OnBubblePlacement?.Invoke(activeBubble.Id, position.X, position.Y);
            CheckForMatch(position);

            GameEvents.OnReadyForVisualization?.Invoke();

            return true;
        }

        private void CheckForMatch(Vector position)
        {
            var combines = map.SeekForCombine (position);

            OutputLog.AddLog("combine count." + combines.Count);

            if (combines.Count < 2)
            {
                OutputLog.AddLog("no match.");
            }
            else
            {
                var mixTarget = map.GetFromPosition(position); // first member of combinations will get mixes, and start to combine.

                var except = new List<Vector>();
                except.AddRange(combines);

                var mixes = map.GetMixes(mixTarget.Numberos, position, except);

                int mixCount = mixes.Count;
                OutputLog.AddLog("mix count => " + mixCount);

                // first mix
                for (int i = 0; i < mixCount; i++)
                {
                    OutputLog.AddLog("mixing:" + mixes[i] + " to " + mixTarget.Id);
                    GameEvents.OnBubbleMixed?.Invoke (map.GetFromPosition(mixes[i]).Id, mixTarget.Id);

                    AddScore(mixTarget.Numberos);

                    map.RemoveFromPosition(mixes[i]);
                }

                int length = combines.Count;

                for (int i = 0; i < length; i++)
                {
                    OutputLog.AddLog("combine member: " + combines[i]);
                }
                
                for (int i = 0; i < length - 1; i++)
                {
                    var first = map.GetFromPosition(combines[i]);
                    var next = map.GetFromPosition(combines[i + 1]);

                    //OutputLog.AddLog("combining: " + first.Id + " to " + next.Id);

                    next.IncreaseNumberos();

                    AddScore(next.Numberos);

                    GameEvents.OnBubbleCombined?.Invoke(first.Id, next.Id);

                    /// remove first.
                    map.RemoveFromPosition(combines[i]);
                }
            }
        }
    }

}
