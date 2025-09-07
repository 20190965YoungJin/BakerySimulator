using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform target;     // 따라갈 타겟 (플레이어)

    [Header("카메라 위치 설정")]
    public Vector3 offset = new Vector3(0, 10, -5); // 위에서 내려다보는 위치
    public float smoothSpeed = 5f;                  // 부드럽게 이동하는 속도

    [Header("카메라 회전 설정")]
    public Vector3 cameraRotation = new Vector3(37, 180, 0); // 카메라 각도 설정

    private void Start()
    {
        // 카메라 모드를 Orthographic으로 강제 설정
        Camera.main.orthographic = true;
        Camera.main.orthographicSize =8.12f; // 필요에 따라 크기 조정

        // 초기 각도 설정
        transform.rotation = Quaternion.Euler(cameraRotation);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
