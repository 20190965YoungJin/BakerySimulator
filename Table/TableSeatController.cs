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
        // 컴포넌트 추출
        Animator npcAnimator = customerObject.GetComponent<Animator>();
        Transform armatureTransform = customerObject.transform.childCount > 0
        ? customerObject.transform.GetChild(0)
        : null;

        // 1. 빵 보이기
        if (breadVisual != null)
            breadVisual.SetActive(true);

        // 2. 위치 이동
        if (armatureTransform != null && sittingPos != null)
        {
            armatureTransform.position = sittingPos.position;
            armatureTransform.rotation = sittingPos.rotation;
        }

        // 3. 애니메이터 조작
        if (npcAnimator != null)
            npcAnimator.SetBool("isSit", true);

        // 4. 의자 돌리기
        chair.localRotation = Quaternion.Euler(0,0,0);

        // 4초 후 초기화
        StartCoroutine(ResetSeat(npcAnimator, armatureTransform));
    }

    private IEnumerator ResetSeat(Animator npcAnimator, Transform armatureTransform)
    {
        yield return new WaitForSeconds(4f);
        // 1. 빵 끄기
        if (breadVisual != null)
            breadVisual.SetActive(false);

        // 2. 위치 복귀
        armatureTransform.localPosition = Vector3.zero;
        armatureTransform.localRotation = Quaternion.identity;

        // 3. 애니메이터 복귀
        if (npcAnimator != null)
            npcAnimator.SetBool("isSit", false);

        // 4. 의자 복귀
        chair.localRotation = Quaternion.Euler(0, 30, 0);

        // 5. 쓰레기 켜기
        trash.SetActive(true);
        MissionManager.Instance.NotifyTrashOn();

        // 6. 돈 생성
        usagePrice = Random.Range(20, 30);
        moneyStacker.SetMoney(usagePrice);

        SoundManager.Instance.Play("cash");
    }

}
