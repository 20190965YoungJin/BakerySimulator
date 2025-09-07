using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CameraFocusController : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isFocusing = false;

    private Camera cam;

    public Transform focusTarget1;
    public Transform focusTarget2;
    public CameraFollow cameraFollow;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    void OnEnable()
    {
        EventBus.OnMoneyThresholdReached += HandleMoneyFocus;
        EventBus.OnUnlockTable += HandleUnlockTableFocus;
    }

    void OnDisable()
    {
        EventBus.OnMoneyThresholdReached -= HandleMoneyFocus;
        EventBus.OnUnlockTable -= HandleUnlockTableFocus;
    }

    void HandleMoneyFocus()
    {
        FocusOn(focusTarget1.position, focusTarget1.rotation, 2f, 1f);
    }

    void HandleUnlockTableFocus()
    {
        FocusOn(focusTarget2.position, focusTarget2.rotation, 2f, 1f);
    }
    /// <summary>
    /// ī�޶� Ư�� ��ġ�� ȸ���� ���� �ð� ���� �ٶ󺸰� �մϴ�.
    /// </summary>
    /// <param name="targetPosition">ī�޶� �̵��� ��ġ</param>
    /// <param name="targetRotation">ī�޶� ���� ȸ����</param>
    /// <param name="focusDuration">�ٶ� �ð� (��)</param>
    /// <param name="transitionTime">��ȯ �ð� (��)</param>
    public void FocusOn(Vector3 targetPosition, Quaternion targetRotation, float focusDuration, float transitionTime = 1f)
    {
        if (isFocusing) return;
        StartCoroutine(FocusRoutine(targetPosition, targetRotation, focusDuration, transitionTime));
    }

    private IEnumerator FocusRoutine(Vector3 targetPos, Quaternion targetRot, float duration, float transition)
    {
        isFocusing = true;
        cameraFollow.enabled = false;

        // ����
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // �ε巯�� ��ȯ
        float elapsed = 0f;
        while (elapsed < transition)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPos, elapsed / transition);
            transform.rotation = Quaternion.Slerp(originalRotation, targetRot, elapsed / transition);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = targetRot;

        // ���� ����
        yield return new WaitForSeconds(duration);

        // ������� �ε巴�� ����
        elapsed = 0f;
        while (elapsed < transition)
        {
            transform.position = Vector3.Lerp(targetPos, originalPosition, elapsed / transition);
            transform.rotation = Quaternion.Slerp(targetRot, originalRotation, elapsed / transition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isFocusing = false;
        cameraFollow.enabled = true;
    }
}
