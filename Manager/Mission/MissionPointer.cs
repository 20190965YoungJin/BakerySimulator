using UnityEngine;

public class MissionPointer : MonoBehaviour
{
    public Transform currentTarget;
    private float heightOffset = 2f;

    void Update()
    {
        if (currentTarget != null)
        {
            transform.position = currentTarget.position + Vector3.up * heightOffset;
        }
    }

    public void SetTarget(Transform target, float offsetHeight = 2f)
    {
        currentTarget = target;
        heightOffset = offsetHeight;
        gameObject.SetActive(target != null);
    }
}
