using System;

namespace Bob
{
    public class BubbleEvents
    {
        #region outputs
        public delegate void BubbleSpawned(Bubble bubble, int NewX, int NewY);
        public delegate void BubbleValueUpdate(ushort Id, Bubble.BubbleType newValue);
        public delegate void BubblePositionUpdate(ushort Id, int NewX, int NewY, bool IsInstant);
        public delegate void BubblePlacement (ushort Id, int NewX, int NewY);
        public delegate void BubbleCombined (ushort Id, ushort tId);
        public delegate void BubbleExplode(ushort Id);
        public delegate void BubbleMixed (ushort Id, ushort tId);
        public delegate void BubbleIsNowFree(ushort Id);
        public delegate void ActiveBallCreated(Bubble bubble);
        public delegate void NextBallBecomeActive();
        public delegate void NextBallSpawned(Bubble bubble);
        public delegate void ReadyForVisualiation();
        public delegate void GameScoreUpdate(int Score);
        public delegate void GameFinished(int Score);

        public BubbleSpawned OnBubbleSpawned;
        public BubbleValueUpdate OnBubbleValueUpdate;
        public BubbleExplode OnBubbleExplode;
        public BubblePositionUpdate OnBubblePositionUpdate;
        public BubblePlacement OnBubblePlacement;
        public BubbleCombined OnBubbleCombined;
        public BubbleMixed OnBubbleMixed;
        public BubbleIsNowFree OnBubbleIsNowFree;
        public ActiveBallCreated OnActiveBallCreated;
        public NextBallBecomeActive OnNextBallBecomeActive;
        public ActiveBallCreated OnActiveBallPlaced;
        public NextBallSpawned OnNextBallSpawned;
        public ReadyForVisualiation OnReadyForVisualization;
        public GameScoreUpdate OnGameScoreUpdate;
        public GameFinished OnGameFinished;
        #endregion

        #region inputs
        public delegate bool PutBubble(int X, int Y);
        public delegate bool AvailablePosition(int X, int Y, out Vector result);

        public PutBubble RequestPutBubble;
        public AvailablePosition RequestAvailablePosition;
        #endregion
    }
}

