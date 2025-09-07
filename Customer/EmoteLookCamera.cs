using UnityEngine;

public class EmoteLookCamera : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // 카메라를 바라보되, 반대 방향으로 돌지 않게
        Vector3 dir = transform.position - cam.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
