using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 일정 수의 돈 오브젝트를 쌓아 올리고,
/// 플레이어가 트리거에 진입하면 돈을 전송한 뒤 제거.
/// </summary>
public class MoneyStacker : MonoBehaviour
{
    [Header("돈 오브젝트 관련")]
    public List<GameObject> moneyObjects;         // 미리 배치된 돈 오브젝트 프리팹들
    private List<Vector3> moneyPositions = new(); // 돈 쌓이는 위치 계산값

    [Header("설정 값")]
    public Transform playerTransform;             // 플레이어 위치

    public int currentMoneyCount = 0;            // 현재 스택된 돈 개수

    private void Start()
    {
        InitPlayerTransform();
        InitializeMoneyPositions();
        DeactivateAllMoney();
    }

    /// <summary>
    /// 외부에서 돈을 스택에 추가
    /// </summary>
    public void SetMoney(int count)
    {
        currentMoneyCount += count;

        if (currentMoneyCount > 30)
            EventBus.RaiseMoneyThresholdEvent();

        for (int i = 0; i < moneyObjects.Count; i++)
        {
            bool shouldShow = i < currentMoneyCount;
            moneyObjects[i].SetActive(shouldShow);

            if (shouldShow)
                moneyObjects[i].transform.localPosition = moneyPositions[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ClearMoney());
        }
    }

    /// <summary>
    /// 플레이어와 닿았을 때 모든 돈을 플레이어에게 전송하고 초기화
    /// </summary>
    private IEnumerator ClearMoney()
    {
        MoneyManager.Instance.AddMoney(currentMoneyCount);
        int moneyToClear = currentMoneyCount;
        currentMoneyCount = 0;

        for (int i = moneyObjects.Count - 1; i >= 0; i--)
        {
            GameObject money = moneyObjects[i];

            if (money.activeSelf)
            {
                Vector3 originalPos = money.transform.position;
                StartCoroutine(MoveToPlayerAndBack(money, originalPos));
                SoundManager.Instance.Play("Success");
                MissionManager.Instance.NotifyGetMoney();

                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    /// <summary>
    /// 돈 오브젝트를 플레이어 쪽으로 이동 후 비활성화
    /// </summary>
    private IEnumerator MoveToPlayerAndBack(GameObject money, Vector3 originalPos)
    {
        yield return BreadMover.WorldMoveBreadCurve(money, originalPos, playerTransform.position, 0.2f, 4f);
        money.SetActive(false);
    }

    /// <summary>
    /// 돈이 쌓이는 위치를 미리 계산해 리스트에 저장
    /// </summary>
    private void InitializeMoneyPositions()
    {
        for (int i = 0; i < moneyObjects.Count; i++)
        {
            moneyPositions.Add(CalculateMoneyPosition(i));
        }
    }

    /// <summary>
    /// 처음에 모든 돈 오브젝트 비활성화
    /// </summary>
    private void DeactivateAllMoney()
    {
        foreach (var money in moneyObjects)
            money.SetActive(false);
    }

    /// <summary>
    /// 플레이어 Transform이 설정되지 않았으면 자동으로 찾아서 설정
    /// </summary>
    private void InitPlayerTransform()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    /// <summary>
    /// 돈이 쌓일 위치 계산
    /// </summary>
    private Vector3 CalculateMoneyPosition(int index)
    {
        float xSpacing = 0.77f;
        float zSpacing = 0.46f;
        float ySpacing = 0.11f;

        int layer = index / 9;
        int indexInLayer = index % 9;
        int row = indexInLayer / 3;
        int col = indexInLayer % 3;

        float x = (1 - col) * xSpacing;
        float z = (1 - row) * zSpacing;
        float y = layer * ySpacing;

        return new Vector3(x, y, z);
    }
}
