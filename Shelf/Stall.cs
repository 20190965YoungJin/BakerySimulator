using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{
    [Header("���� ��ġ ����Ʈ (������� �Ҵ�)")]
    public List<Transform> displayPositions = new List<Transform>(); // ���� ��ġ��
    private Stack<GameObject> stallStack = new Stack<GameObject>();  // ���� ������ ����

    public bool isSupply = false;

    // �� ����
    public void PushBread(GameObject bread)
    {
        if (stallStack.Count >= displayPositions.Count)
        {
            Debug.Log("Stall is full!");
            return;
        }

        Transform targetPos = displayPositions[stallStack.Count];

        bread.transform.SetParent(transform); // �������� �ڽ����� ����
        bread.GetComponent<Rigidbody>().isKinematic = true;
        bread.transform.localRotation = Quaternion.Euler(0, -131f, 0); // ȸ�� ����

        // Bezier � �̵� ����
        StartCoroutine(BreadMover.WorldMoveBreadCurve(bread, bread.transform.position, targetPos.position, 0.05f));

        stallStack.Push(bread);

        MissionManager.Instance.NotifyShelfFilled();
        //Debug.Log($"Bread placed at stall slot {stallStack.Count}");
    }

    public GameObject PopBread()
    {
        if (stallStack.Count == 0) return null;

        GameObject bread = stallStack.Pop();
        bread.transform.SetParent(null);
        return bread;
    }

    public bool HasEmptySlot()
    {
        return stallStack.Count < displayPositions.Count;
    }

    public bool HasBread()
    {
        return stallStack.Count > 0;
    }
}
