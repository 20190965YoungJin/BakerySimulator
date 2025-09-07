using System.Collections.Generic;
using UnityEngine;

public class StackBread : MonoBehaviour
{
    private Stack<GameObject> breadStack = new Stack<GameObject>();
    public float breadHeight = 0.3f;  // �� �ϳ��� ���̴� ����
    public int maxStackSize = 8;     // �ִ� ���� ��
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

            // ���� ���� �� ������� Ȯ���ϰ� �ִϸ��̼� ���� ����
            if (breadStack.Count == 0 && playerAnim != null)
            {
                playerAnim.SetBool("isStack", false);
            }
            SoundManager.Instance.Play("Put_Object");

            return topBread;
        }

        return null;
    }

    // ��� �ִ� �� ��ü �ʱ�ȭ
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