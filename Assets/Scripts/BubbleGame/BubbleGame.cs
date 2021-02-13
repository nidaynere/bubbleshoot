using System;

namespace Bob
{
    public struct BubbleGame
    {
        public BubbleEvents GameEvents;
        private BubbleGrid map;
        private ushort idCounter;

        private Bubble activeBall;
        private Bubble nextBall;

        public BubbleGame(int gridSizeX, int gridSizeY)
        {
            #region define
            map = new BubbleGrid(new Vector (gridSizeX, gridSizeY));
            idCounter = 0;
            activeBall = null;
            nextBall = null;
            GameEvents = new BubbleEvents();
            #endregion

            GameEvents.OnPutBubble += PutBubble;
        }

        public void NextTurn()
        {
            if (activeBall == null)
            {
                activeBall = CreateBubble(ref idCounter);
                GameEvents.OnActiveBallCreated?.Invoke(activeBall);
            }
            else
            {
                activeBall = nextBall;
                GameEvents.OnNextBallBecomeActive?.Invoke();
            }

            nextBall = CreateBubble(ref idCounter);
            GameEvents.OnNextBallSpawned?.Invoke(nextBall);
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
                for (int x = 0; x < mapSizeX; i++)
                {
                    var rand = new Random();
                    int randomizer = rand.Next(0, 100);
                    if (randomizer <= fillChance)
                    {
                        bubbles[x] = CreateBubble(ref idCounter);
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
        private void PutBubble(int X, int Y)
        {
            //map.SetToPosition ()
        }
    }
}
