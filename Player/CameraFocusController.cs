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
    /// 카메라가 특정 위치와 회전을 일정 시간 동안 바라보게 합니다.
    /// </summary>
    /// <param name="targetPosition">카메라가 이동할 위치</param>
    /// <param name="targetRotation">카메라가 가질 회전값</param>
    /// <param name="focusDuration">바라볼 시간 (초)</param>
    /// <param name="transitionTime">전환 시간 (초)</param>
    public void FocusOn(Vector3 targetPosition, Quaternion targetRotation, float focusDuration, float transitionTime = 1f)
    {
        if (isFocusing) return;
        StartCoroutine(FocusRoutine(targetPosition, targetRotation, focusDuration, transitionTime));
    }

    private IEnumerator FocusRoutine(Vector3 targetPos, Quaternion targetRot, float duration, float transition)
    {
        isFocusing = true;
        cameraFollow.enabled = false;

        // 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // 부드러운 전환
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

        // 고정 유지
        yield return new WaitForSeconds(duration);

        // 원래대로 부드럽게 복귀
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
