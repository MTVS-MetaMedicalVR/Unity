// SinkAreaObject.cs
using UnityEngine;

public class SinkAreaObject : ProcedureObjectBase
{
    [SerializeField] private float activationDistance = 0.3f;
    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        if (!isDone && isInteractionEnabled &&
            Vector3.Distance(playerTransform.position, transform.position) < activationDistance)
        {
            CompleteInteraction();
        }
    }
}