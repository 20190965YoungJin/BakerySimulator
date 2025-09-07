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

        // ī�޶� �ٶ󺸵�, �ݴ� �������� ���� �ʰ�
        Vector3 dir = transform.position - cam.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
