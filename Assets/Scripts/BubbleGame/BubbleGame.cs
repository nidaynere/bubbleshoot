using System;

namespace Bob
{
    public class BubbleGame
    {
#if UNITY_EDITOR
        public BubbleGrid GetMap => map;
#endif
        public BubbleEvents GameEvents;
        private BubbleGrid map;
        private ushort idCounter;

        private Bubble activeBubble;
        private Bubble nextBubble;

        public BubbleGame(int gridSizeX, int gridSizeY)
        {
            #region define
            map = new BubbleGrid(new Vector (gridSizeX, gridSizeY));
            GameEvents = new BubbleEvents();
            #endregion

            GameEvents.RequestPutBubble += PutBubble;
            GameEvents.RequestAvailablePosition += AvailablePosition;
        }

        public void NextTurn()
        {
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
            count = Math.Min(count, map.Size.Y);

            int mapSizeX = map.Size.X;

            // update bubbles at index 1;
            Bubble[] f_bubbles = new Bubble[mapSizeX];
            Vector[] f_positions = new Vector[mapSizeX];

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

                int f_count = map.GetBubblesAtRow(1, mapSizeX, ref f_bubbles, ref f_positions);
                for (int f = 0; f < f_count; f++)
                {
                    GameEvents.OnBubblePositionUpdate?.Invoke(f_bubbles[f].Id, f_positions[f].X, f_positions[f].Y, isInstant);
                }
            }
        }

        private Bubble CreateBubble (ref ushort counter)
        {
            var bubble = new Bubble(++counter);
            return bubble;
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
            Vector[] mixes;
            int mixCount;

            var combines = map.SeekForCombine (position, out mixes, out mixCount);

            OutputLog.AddLog("combine count." + combines.Count);

            if (combines.Count < 2)
            {
                OutputLog.AddLog("no match.");
            }
            else
            {
                var mixTarget = map.GetFromPosition (combines[0]); // first member of combinations will get mixes, and start to combine.
                // first mix
                for (int i = 0; i < mixCount; i++)
                {
                    OutputLog.AddLog("mixing:" + mixes[i] + " to " + mixTarget.Id);
                    GameEvents.OnBubbleMixed?.Invoke (map.GetFromPosition(mixes[i]).Id, mixTarget.Id);
                    map.RemoveFromPosition(mixes[i]);
                }

                int length = combines.Count;

                for (int i = 0; i < length - 1; i++)
                {
                    var first = map.GetFromPosition(combines[i]);
                    var next = map.GetFromPosition(combines[i + 1]);

                    OutputLog.AddLog("combining: " + first.Id + " to " + next.Id);

                    next.IncreaseNumberos();

                    GameEvents.OnBubbleCombined?.Invoke(first.Id, next.Id);

                    /// remove first.
                    map.RemoveFromPosition(combines[i]);
                }
            }
        }

    }

}
