using UnityEngine;

public class Pool
{
    public Pool (Transform holder, GameBall poolObject, int poolSize)
    {
        size = poolSize;
        poolObjects = new GameBall[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            poolObjects[i] = Object.Instantiate(poolObject, holder);
            poolObjects[i].gameObject.SetActive(false);
        }
    }

    private GameBall[] poolObjects;
    private int currentStep = 0;
    private int size;

    public void Dispose()
    {
        for (int i = 0; i < size; i++)
        {
            Object.Destroy(poolObjects[i].gameObject);
        }

        poolObjects = null;
    }

    public GameBall Get()
    {
        var target = poolObjects[currentStep];
        if (++currentStep > size)
            currentStep = 0;

        return target;
    }

    public void Reset()
    {
        for (int i = 0; i < size; i++)
        {
            poolObjects[i].gameObject.SetActive(false);
        }

        currentStep = 0;
    }
}
