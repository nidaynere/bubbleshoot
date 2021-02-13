using UnityEngine;
using System;
using System.Collections;

public class Transition : MonoBehaviour
{
    private Coroutine currentTransition;

    public void Move(Vector3 target, float speed, Action onCompleted = null)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(startTransition(target, speed, onCompleted));
    }

    public void Move(Vector3[] target, float speed, Action onCompleted = null)
    {
        currentTransition = StartCoroutine(startTransition(target, speed, onCompleted));
    }

    private IEnumerator startTransition(Vector3[] target, float speed, Action onCompleted)
    {
        int pathCount = target.Length;
        int currentPath = 0;

        bool isInTransition = false;
        while (true)
        {
            if (!isInTransition)
            {
                currentTransition = StartCoroutine(startTransition(target[currentPath], speed, () =>
                {
                    currentPath++;
                    isInTransition = false;
                }));

                isInTransition = true;
            }

            else
            {
                if (currentPath >= target.Length)
                {
                    onCompleted?.Invoke();
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator startTransition(Vector3 target, float speed, Action onCompleted)
    {
        Vector3 startPosition = transform.position;
        float progress = 0;
        float progressSpeed = Vector3.Distance(target, startPosition) / speed; 

        while (true)
        {
            progress = Mathf.Max (progress + progressSpeed * Time.deltaTime, 1);
            transform.position = Vector3.Lerp(startPosition, target, progress);

            if (progress == 1)
            {
                onCompleted?.Invoke();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
