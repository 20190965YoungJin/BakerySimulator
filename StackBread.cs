using System.Collections.Generic;
using UnityEngine;

public class StackBread : MonoBehaviour
{
    private Stack<GameObject> breadStack = new Stack<GameObject>();
    public float breadHeight = 0.3f;  // 빵 하나당 쌓이는 높이
    public int maxStackSize = 8;     // 최대 스택 수
    public Animator playerAnim;

    private void Start()
    {
        if (playerAnim == null)
            playerAnim = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        ClearBread();
    }

    public void PushBread(GameObject bread)
    {
        int currentIndex = breadStack.Count;
        Vector3 localTargetPos = new Vector3(0f, breadHeight * currentIndex, 0f);

        bread.transform.SetParent(transform);

        StartCoroutine(BreadMover.LocalMoveBreadCurve(bread, bread.transform.localPosition, localTargetPos, 0.1f));

        bread.transform.localEulerAngles = new Vector3(0, 90f, 0);
        bread.GetComponent<Rigidbody>().isKinematic = true;

        breadStack.Push(bread);

        if (breadStack.Count > 0 && playerAnim != null)
        {
            playerAnim.SetBool("isStack", true);
            MissionManager.Instance.NotifyBreadPicked();
        }
        SoundManager.Instance.Play("Get_Object");
    }

    public GameObject PopBread()
    {
        if (breadStack.Count > 0)
        {
            GameObject topBread = breadStack.Pop();
            topBread.transform.SetParent(null);

            // 빵을 꺼낸 후 비었는지 확인하고 애니메이션 상태 갱신
            if (breadStack.Count == 0 && playerAnim != null)
            {
                playerAnim.SetBool("isStack", false);
            }
            SoundManager.Instance.Play("Put_Object");

            return topBread;
        }

        return null;
    }

    // 들고 있는 빵 전체 초기화
    public void ClearBread()
    {
        while (breadStack.Count > 0)
        {
            GameObject bread = breadStack.Pop();
            if (bread != null)
            {
                Destroy(bread);
            }
        }

        if (playerAnim != null)
        {
            playerAnim.SetBool("isStack", false);
        }
    }



    public bool MaxStackSize()
    {
        return maxStackSize > breadStack.Count;
    }
}