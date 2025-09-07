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
                // �ڷ�ƾ ���� �� ���� ����
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
            // ���� ����� �� �ڷ�ƾ ����
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
        Debug.Log("�� BakeryGround ����");
        while (true)
        {
            Debug.Log("Bakery �۾� ���� ��...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator StoneRoutine()
    {
        Debug.Log("�� StoneLayer ����");
        while (true)
        {
            Debug.Log("Stone ������ ����...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator WaterRoutine()
    {
        Debug.Log("�� WaterLayer ����");
        while (true)
        {
            Debug.Log("���� ��...");
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator DefaultRoutine()
    {
        Debug.Log("�� Default Ground ����");
        while (true)
        {
            Debug.Log("�⺻ �ٴ� ���� ��...");
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
