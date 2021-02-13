using System;

namespace Bob
{
    public class BubbleEvents
    {
        #region outputs
        public delegate void BubbleSpawned(Bubble bubble, int NewX, int NewY);
        public delegate void BubbleValueUpdate(ushort Id, Bubble.BubbleType newValue);
        public delegate void BubblePositionUpdate(ushort Id, int NewX, int NewY, bool IsInstant);
        public delegate void BubbleCombined (ushort Id, ushort tId);
        public delegate void BubbleMixed (ushort Id, ushort tId);
        public delegate void BubbleIsNowFree(ushort Id);
        public delegate void ActiveBallCreated(Bubble bubble);
        public delegate void NextBallBecomeActive();
        public delegate void NextBallSpawned(Bubble bubble);

        public BubbleSpawned OnBubbleSpawned;
        public BubbleValueUpdate OnBubbleValueUpdate;
        public BubblePositionUpdate OnBubblePositionUpdate;
        public BubbleCombined OnBubbleCombined;
        public BubbleMixed OnBubbleMixed;
        public BubbleIsNowFree OnBubbleIsNowFree;
        public ActiveBallCreated OnActiveBallCreated;
        public NextBallBecomeActive OnNextBallBecomeActive;
        public ActiveBallCreated OnActiveBallPlaced;
        public NextBallSpawned OnNextBallSpawned;
        #endregion

        #region inputs
        public delegate bool PutBubble(int X, int Y, bool ForceClosePosition);
        public PutBubble RequestPutBubble;
        #endregion
    }
}

