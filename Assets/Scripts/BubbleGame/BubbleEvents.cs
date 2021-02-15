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
        public delegate void BubbleInteraction (ushort Id, ushort tId);
        public delegate void BubbleEvent (ushort Id);
        public delegate void BubbleExploded(int X, int Y, ushort[] killed);
        public delegate void BubbleCreated(Bubble bubble);
        public delegate void NextBallBecomeActive();
        public delegate void ReadyForVisualiation();
        public delegate void ScoreEvent(int Score);

        public BubbleSpawned OnBubbleSpawned;
        public BubbleValueUpdate OnBubbleValueUpdate;
        public BubblePositionUpdate OnBubblePositionUpdate;
        public BubblePlacement OnBubblePlacement;

        /// <summary>
        /// Two bubbles are combined (etc. 4 + 4 = 8)
        /// </summary>
        public BubbleInteraction OnBubbleCombined;

        /// <summary>
        /// Two bubbles with same type are mixed (4 + 4 = 4). This event throws when some combining happens.
        /// The game selects the best path for combining bubbles, if there are other combining paths too, the first member of that paths, will be mixed with the best way.
        /// Mixing also adds score to the user.
        /// </summary>
        public BubbleInteraction OnBubbleMixed;

        /// <summary>
        /// Called when a bubble exploded. It throws the exploded bubble, and the killeds. Killed list includes the exploded bubble.
        /// </summary>
        public BubbleExploded OnBubbleExploded;

        /// <summary>
        /// Bubble is now free. It means its destroyed on the system and should fly away in the visualization.
        /// </summary>
        public BubbleEvent OnBubbleIsNowFree;
        public BubbleCreated OnActiveBallCreated;
        public NextBallBecomeActive OnNextBallBecomeActive;
        public BubbleCreated OnActiveBallPlaced;
        public BubbleCreated OnNextBallSpawned;
        public ReadyForVisualiation OnReadyForVisualization;
        public ScoreEvent OnGameScoreUpdate;
        public ScoreEvent OnGameFinished;
        #endregion

        #region inputs
        public delegate bool PutBubble(int X, int Y);
        public delegate bool AvailablePosition(int X, int Y, out Vector result);

        public PutBubble RequestPutBubble;
        public AvailablePosition RequestAvailablePosition;
        #endregion
    }
}

