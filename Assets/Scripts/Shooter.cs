﻿using UnityEngine;
using System.Linq;

public class Shooter : MonoBehaviour
{
    #region events
    public delegate void ShootEvent(Vector3[] positions);
    public ShootEvent OnShoot;
    public ShootEvent OnAim;

    #endregion

#pragma warning disable CS0649
    #region serialized variables
    [SerializeField] private AimArrow aimArrow;
    /// <summary>
    /// Focus point will be 3D object, we will get the screenpos of it.
    /// </summary>
    [SerializeField] private Transform focusPoint;
    [SerializeField] private GamePlayEvents inputEvents;
    [SerializeField] private Input mouseInput;
    #endregion
#pragma warning restore CS0649

    const int maxCrosses = 5;
    private Vector3[] positions = new Vector3[maxCrosses + 2]; // 2 means start & end position
    private int positionCount;

    private bool inputActive;
    
    void Start()
    {
        mouseInput.RegisterInput(OnInputHoldAndMove, OnInputDown, OnInputUp);

        inputEvents.OnGameplayStatusChange += (value) => {
            mouseInput.enabled = value;

            if (!value && inputActive)
            {
                OnInputUp (Vector2.zero);
            }
        };
    }

    private Vector2 getAimDirection(ref Vector2 position)
    {
        Vector2 findFocusOn2D = Camera.main.WorldToScreenPoint(focusPoint.position);
        return findFocusOn2D - position;
    }

    private void OnInputHoldAndMove(Vector2 position)
    {
        positionCount = aimArrow.SetDirection(getAimDirection(ref position), maxCrosses, positions);

        OnAim?.Invoke(positions.Take(positionCount + 1).ToArray());
    }

    private void OnInputDown(Vector2 position)
    {
        Debug.Log("[Shooter] OnInputDown, position => " + position);
        inputActive = true;

        aimArrow.SetVisual(true);
        OnInputHoldAndMove(position);
    }

    private void OnInputUp(Vector2 position)
    {
        Debug.Log("[Shooter] OnInputUp, position => " + position);

        inputActive = false;

        aimArrow.SetVisual(false);

        OnShoot?.Invoke(positions.Take (positionCount+1).ToArray ());
    }
}