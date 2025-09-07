using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float groundedOffset = 0f;
    public LayerMask groundLayers;
    public CharacterController controller;
    public bool isGrounded = false;

    public float checkRadius = 0.2f;
    public string currentGroundLayer = "";
    private string previousGroundLayer = "";
    private Coroutine activeCoroutine = null;

    private void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundedCheck();
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        Collider[] hitColliders = Physics.OverlapSphere(spherePosition, checkRadius, groundLayers, QueryTriggerInteraction.Ignore);

        if (hitColliders.Length > 0)
        {
            Collider groundCollider = hitColliders[0];
            currentGroundLayer = LayerMask.LayerToName(groundCollider.gameObject.layer);
            isGrounded = true;

            if (currentGroundLayer != previousGroundLayer)
            {
                // 코루틴 중지 및 새로 실행
                if (activeCoroutine != null)
                    StopCoroutine(activeCoroutine);

                previousGroundLayer = currentGroundLayer;

                switch (currentGroundLayer)
                {
                    case "BakeryGround":
                        activeCoroutine = StartCoroutine(BakeryRoutine());
                        break;

                    case "Stone":
                        activeCoroutine = StartCoroutine(StoneRoutine());
                        break;

                    case "Water":
                        activeCoroutine = StartCoroutine(WaterRoutine());
                        break;

                    default:
                        activeCoroutine = StartCoroutine(DefaultRoutine());
                        break;
                }
            }
        }
        else
        {
            // 범위 벗어났을 때 코루틴 정지
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
                activeCoroutine = null;
            }

            currentGroundLayer = "";
            previousGroundLayer = "";
            isGrounded = false;
        }
    }

    private IEnumerator BakeryRoutine()
    {
        Debug.Log("▶ BakeryGround 진입");
        while (true)
        {
            Debug.Log("Bakery 작업 수행 중...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator StoneRoutine()
    {
        Debug.Log("▶ StoneLayer 진입");
        while (true)
        {
            Debug.Log("Stone 위에서 동작...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator WaterRoutine()
    {
        Debug.Log("▶ WaterLayer 진입");
        while (true)
        {
            Debug.Log("수영 중...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator DefaultRoutine()
    {
        Debug.Log("▶ Default Ground 진입");
        while (true)
        {
            Debug.Log("기본 바닥 동작 중...");
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, checkRadius);
    }
}
