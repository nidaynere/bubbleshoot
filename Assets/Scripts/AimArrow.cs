using UnityEngine;
using System.Collections.Generic;

public class AimArrow : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Animator aimAnimator;
    [SerializeField] private Transform focusPoint;

    [SerializeField] private float rayLength;
    [SerializeField] private List<Collider> walls;

    const int maxCrosses = 5;
    private Vector3[] positions = new Vector3[maxCrosses+2]; // 2 means start & end position

    public void SetDirection (Vector2 direction)
    {
        var normalize = direction.normalized;
        var dir = new Vector3(normalize.x, normalize.y, 0);

        Vector3 startPosition = focusPoint.position;

        int crosses = 0;
        positions[crosses] = startPosition;

        while (true && crosses < maxCrosses)
        {
            RaycastHit rh;
            if (Physics.Raycast(startPosition, dir, out rh, rayLength))
            {
                if (walls.Contains(rh.collider))
                {
                    // cross the direction.
                    crosses++;
                    startPosition = rh.point;
                    dir = Vector3.Reflect(dir, rh.normal);
                    positions[crosses] = startPosition;
                }
                else break;
            }
            else break;
        }

        positions[crosses+1] = startPosition + dir * rayLength;

        lineRenderer.positionCount = crosses + 1;
        lineRenderer.SetPositions(positions);
    }

    public void SetVisual (bool value)
    {
        aimAnimator.SetBool("IsActive", value);
    }
}
