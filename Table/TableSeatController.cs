using UnityEngine;
using System.Collections;

public class TableSeatController : MonoBehaviour
{
    public MoneyStacker moneyStacker;
    public GameObject breadVisual;
    public Transform chair;
    public GameObject trash;
    public Transform sittingPos;
    private int usagePrice = 0;

    void Awake()
    {
        if (breadVisual != null) breadVisual.SetActive(false);
        else breadVisual = transform.Find("Croassant").gameObject; breadVisual.SetActive(false);
        if (trash != null) trash.SetActive(false);
        else trash = transform.Find("Trash").gameObject; trash.SetActive(false);
    }

    public void OnCustomerSitting(GameObject customerObject)
    {
        // ������Ʈ ����
        Animator npcAnimator = customerObject.GetComponent<Animator>();
        Transform armatureTransform = customerObject.transform.childCount > 0
        ? customerObject.transform.GetChild(0)
        : null;

        // 1. �� ���̱�
        if (breadVisual != null)
            breadVisual.SetActive(true);

        // 2. ��ġ �̵�
        if (armatureTransform != null && sittingPos != null)
        {
            armatureTransform.position = sittingPos.position;
            armatureTransform.rotation = sittingPos.rotation;
        }

        // 3. �ִϸ����� ����
        if (npcAnimator != null)
            npcAnimator.SetBool("isSit", true);

        // 4. ���� ������
        chair.localRotation = Quaternion.Euler(0,0,0);

        // 4�� �� �ʱ�ȭ
        StartCoroutine(ResetSeat(npcAnimator, armatureTransform));
    }

    private IEnumerator ResetSeat(Animator npcAnimator, Transform armatureTransform)
    {
        yield return new WaitForSeconds(4f);
        // 1. �� ����
        if (breadVisual != null)
            breadVisual.SetActive(false);

        // 2. ��ġ ����
        armatureTransform.localPosition = Vector3.zero;
        armatureTransform.localRotation = Quaternion.identity;

        // 3. �ִϸ����� ����
        if (npcAnimator != null)
            npcAnimator.SetBool("isSit", false);

        // 4. ���� ����
        chair.localRotation = Quaternion.Euler(0, 30, 0);

        // 5. ������ �ѱ�
        trash.SetActive(true);
        MissionManager.Instance.NotifyTrashOn();

        // 6. �� ����
        usagePrice = Random.Range(20, 30);
        moneyStacker.SetMoney(usagePrice);

        SoundManager.Instance.Play("cash");
    }

}
