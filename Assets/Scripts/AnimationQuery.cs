using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public struct AnimationQuery
{
    private AnimationSettings gameSettings;
    private List<BaseAction> Query;

    public AnimationQuery(AnimationSettings _gameSettings)
    {
        gameSettings = _gameSettings;
        Query = new List<BaseAction>();
    }

    public void AddToQuery(BaseAction animation)
    {
        Query.Add(animation);
    }

    public IEnumerator DoQuery (Action onCompleted)
    {
        if (Query.Count == 0)
        {
            Debug.Log("[CombineAnimationQuery->DoQuery => Query is empty!");
            yield break;
        }

        Debug.Log("DoQuery");
        bool isPlaying = false;
        while (true)
        {
            while (isPlaying)
            {
                yield return new WaitForSeconds(gameSettings.CombineAnimationDelay);
            }

            if (Query.Count == 0)
            {// Done
                Debug.Log("Animation query => completed.");

                onCompleted?.Invoke();
                yield break;
            }

            isPlaying = true;

            Debug.Log("Animation query => started anim.");

            Query[0].Trigger(gameSettings, () => {
                Debug.Log("Animation query => finished anim.");
                isPlaying = false;
            });

            Query.RemoveAt(0);
        }
    }


    public class BaseAction
    {
        protected GameBall ballFrom, ballTo;

        protected Action onTriggered;

        public BaseAction(GameBall ballFrom, GameBall ballTo, Action onTriggered)
        {
            this.ballFrom = ballFrom;
            this.ballTo = ballTo;
            this.onTriggered = onTriggered;
        }

        public virtual void Trigger(AnimationSettings gameSettings, Action onCompleted) {
            onTriggered?.Invoke();
        }
    }

    public class CombineAction : BaseAction
    {
        public CombineAction (GameBall _ballFrom, GameBall _ballTo) : base (_ballFrom, _ballTo, null)
        {

        }

        public override void Trigger(AnimationSettings gameSettings, Action onCompleted)
        {
            var _from = ballFrom;
            var _to = ballTo;
            ballFrom.Move(ballTo.transform.position, gameSettings.PositionUpdateSpeed, () => {
                _from.Kill();
                _to.Upgrade();

                onCompleted?.Invoke();
            });
        }
    }

    public class MixAction : BaseAction
    {
        public MixAction(GameBall _ballFrom, GameBall _ballTo) : base(_ballFrom, _ballTo, null)
        {

        }

        public override void Trigger(AnimationSettings gameSettings, Action onCompleted)
        {
            var _from = ballFrom;
            var _to = ballTo;
            ballFrom.Move(ballTo.transform.position, gameSettings.PositionUpdateSpeed, () => {
                _from.Kill();
                onCompleted?.Invoke();
            });
        }
    }

    public class PlacementAction : BaseAction
    {
        private int X, Y;
        public PlacementAction(GameBall _ballFrom, int _X, int _Y) : base(_ballFrom, null, null)
        {
            X = _X;
            Y = _Y;
        }

        public override void Trigger(AnimationSettings gameSettings, Action onCompleted)
        {
            var _from = ballFrom;
            var _to = ballTo;

            ballFrom.SetTransition(X, Y, () => { onCompleted?.Invoke(); });
        }
    }

    public class ExplodeAction : BaseAction
    {
        public ExplodeAction(Action onTriggered) : base(null, null, onTriggered)
        {

        }

        public override void Trigger(AnimationSettings gameSettings, Action onCompleted)
        {
            base.Trigger(gameSettings, onCompleted);
            onCompleted?.Invoke();
        }
    }
}
