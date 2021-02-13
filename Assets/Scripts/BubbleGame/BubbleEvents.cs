using System;

namespace Bob
{
    public class BubbleEvents
    {
        #region outputs
        public delegate void BubbleSpawned(Bubble bubble);
        public delegate void BubbleValueUpdate(int Id, Bubble.BubbleType newValue);
        public delegate void BubblePositionUpdate(int Id, int NewX, int NewY);
        public delegate void BubbleDestroyed(int Id);
        public delegate void BubbleIsNowFree(int Id);

        public BubbleSpawned OnBubbleSpawned;
        public BubbleValueUpdate OnBubbleValueUpdate;
        public BubblePositionUpdate OnBubblePositionUpdate;
        public BubbleDestroyed OnBubbleDestroyed;
        public BubbleIsNowFree OnBubbleIsNowFree;
        #endregion

        #region inputs
        public delegate void SetBubblePosition(int Id, int X, int Y);
        public SetBubblePosition OnSetBubblePosition;
        #endregion
    }
}

