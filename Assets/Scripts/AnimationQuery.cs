using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public struct AnimationQuery
{
    private GameSettings gameSettings;
    private List<BaseAction> Query;

    public AnimationQuery(GameSettings _gameSettings)
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

    [System.Serializable]
    public class BaseAction
    {
        protected GameBall ballFrom, ballTo;

        public BaseAction(GameBall _ballFrom, GameBall _ballTo)
        {
            ballFrom = _ballFrom;
            ballTo = _ballTo;
        }

        public virtual void Trigger(GameSettings gameSettings, Action onCompleted) { }
    }
    [System.Serializable]
    public class CombineAction : BaseAction
    {
        public CombineAction (GameBall _ballFrom, GameBall _ballTo) : base (_ballFrom, _ballTo)
        {

        }

        public override void Trigger(GameSettings gameSettings, Action onCompleted)
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
    [System.Serializable]
    public class MixAction : BaseAction
    {
        public MixAction(GameBall _ballFrom, GameBall _ballTo) : base(_ballFrom, _ballTo)
        {

        }

        public override void Trigger(GameSettings gameSettings, Action onCompleted)
        {
            var _from = ballFrom;
            var _to = ballTo;
            ballFrom.Move(ballTo.transform.position, gameSettings.PositionUpdateSpeed, () => {
                _from.Kill();
                onCompleted?.Invoke();
            });
        }
    }
}
