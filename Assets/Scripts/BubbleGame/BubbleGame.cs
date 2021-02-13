using System.Collections.Generic;
using System;

namespace Bob
{
    public struct BubbleGame
    {
        public BubbleEvents GameEvents;

        private Grid map;

        private ushort idCounter;

        public BubbleGame(int gridSizeX, int gridSizeY)
        {
            map = new Grid(new Vector (gridSizeX, gridSizeY));
            idCounter = 0;

            GameEvents = new BubbleEvents();
            GameEvents.OnSetBubblePosition += SetBubblePosition;
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
                        bubbles[x] = CreateBubble();
                    }
                }
            }

            map.AddBubbles(bubbles);
        }

        private Bubble CreateBubble ()
        {
            var bubble = new Bubble(++idCounter);
            return bubble;
        }

        private void SetBubblePosition(int Index, int X, int Y)
        {
            
        }
    }
}
