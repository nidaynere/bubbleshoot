using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[RequireComponent (typeof (GameObjectEntity))]
public class FollowRectTransformInWorldSpace : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private RectTransform targetToFollow;
    [SerializeField] private float zDistance = 10;

#pragma warning restore CS0649
    public class FollowTargetEntity : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var camera = Camera.main;
            Entities.ForEach((Transform transform, FollowRectTransformInWorldSpace followTarget) =>
            {
                Vector3 pos = followTarget.targetToFollow.position;
                pos.z = followTarget.zDistance;
                transform.position = camera.ScreenToWorldPoint(pos); 
            }
            );
        }
    }
}
