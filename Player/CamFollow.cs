using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Ÿ�� ����")]
    public Transform target;     // ���� Ÿ�� (�÷��̾�)

    [Header("ī�޶� ��ġ ����")]
    public Vector3 offset = new Vector3(0, 10, -5); // ������ �����ٺ��� ��ġ
    public float smoothSpeed = 5f;                  // �ε巴�� �̵��ϴ� �ӵ�

    [Header("ī�޶� ȸ�� ����")]
    public Vector3 cameraRotation = new Vector3(37, 180, 0); // ī�޶� ���� ����

    private void Start()
    {
        // ī�޶� ��带 Orthographic���� ���� ����
        Camera.main.orthographic = true;
        Camera.main.orthographicSize =8.12f; // �ʿ信 ���� ũ�� ����

        // �ʱ� ���� ����
        transform.rotation = Quaternion.Euler(cameraRotation);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ���
        Vector3 desiredPosition = target.position + offset;

        // �ε巴�� �̵�
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
