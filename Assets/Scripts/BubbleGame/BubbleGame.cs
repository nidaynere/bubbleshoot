using System;

namespace Bob
{
    public struct BubbleGame
    {
        public BubbleEvents GameEvents;
        private BubbleGrid map;
        private ushort idCounter;

        private Bubble activeBubble;
        private Bubble nextBubble;

        public BubbleGame(int gridSizeX, int gridSizeY)
        {
            #region define
            map = new BubbleGrid(new Vector (gridSizeX, gridSizeY));
            idCounter = 0;
            activeBubble = null;
            nextBubble = null;
            GameEvents = new BubbleEvents();
            #endregion

            GameEvents.OnPutBubble += PutBubble;
            GameEvents.OnCheckForMatch += CheckForMatch;
        }

        public void NextTurn()
        {
            if (activeBubble == null)
            {
                activeBubble = CreateBubble(ref idCounter);
                GameEvents.OnActiveBallCreated?.Invoke(activeBubble);
            }
            else
            {
                activeBubble = nextBubble;
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
        public void CreateRows(int count, int fillChance)
        {
            count = Math.Min(count, map.Size.Y);

            int mapSizeX = map.Size.X;

            Bubble[] bubbles = new Bubble[mapSizeX];

            for (int i = 0; i < count; i++)
            {
                for (int x = 0; x < mapSizeX; x++)
                {
                    var rand = new Random(i * x);
                    var randomizer = rand.NextDouble()* 100f;
                    if (randomizer <= fillChance)
                    {
                        bubbles[x] = CreateBubble(ref idCounter);
                        GameEvents.OnBubbleSpawned?.Invoke(bubbles[x], x, i);
                    }
                }
            }

            map.AddBubbles(bubbles);
        }

        private Bubble CreateBubble (ref ushort counter)
        {
            var bubble = new Bubble(++counter);
            return bubble;
        }

        /// <summary>
        /// put active ball to a position and let the grid run the algorithm.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private bool PutBubble(int X, int Y)
        {
            Vector position = new Vector(X, Y);

            if (!map.IsPositionAvailable(position))
            {
                return false;
            }

            map.AddToPosition(activeBubble, position);

            return true;
        }

        private void CheckForMatch(int X, int Y)
        {
            throw new System.NotImplementedException();
        }
    }
}
