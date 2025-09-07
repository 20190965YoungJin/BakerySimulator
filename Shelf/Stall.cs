using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{
    [Header("진열 위치 리스트 (순서대로 할당)")]
    public List<Transform> displayPositions = new List<Transform>(); // 진열 위치들
    private Stack<GameObject> stallStack = new Stack<GameObject>();  // 실제 진열된 빵들

    public bool isSupply = false;

    // 빵 진열
    public void PushBread(GameObject bread)
    {
        if (stallStack.Count >= displayPositions.Count)
        {
            Debug.Log("Stall is full!");
            return;
        }

        Transform targetPos = displayPositions[stallStack.Count];

        bread.transform.SetParent(transform); // 진열대의 자식으로 설정
        bread.GetComponent<Rigidbody>().isKinematic = true;
        bread.transform.localRotation = Quaternion.Euler(0, -131f, 0); // 회전 고정

        // Bezier 곡선 이동 시작
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
