using UnityEngine;
using System;
using System.Collections;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public MissionPointer pointer;

    public Transform breadPos;
    public Transform shelfPos;
    public Transform counterPos;
    public Transform moneyPos;
    public Transform tablePos;
    public Transform trashPos;

    public enum MissionState
    {
        None,
        GoToBread,
        FillShelf,
        GoToCounter,
        GetMoney,
        UpgradeTable,
        ClearTable,
        Completed
    }

    public MissionState CurrentState { get; private set; } = MissionState.None;

    public static event Action<MissionState> OnMissionChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (pointer == null)
            pointer = FindObjectOfType<MissionPointer>();
    }

    private void Start()
    {
        StartCoroutine(MissionFlow());
    }

    IEnumerator MissionFlow()
    {
        // 1단계: 빵 습득 하기
        ChangeMission(MissionState.GoToBread);
        yield return new WaitUntil(() => _hasPickedBread);

        // 2단계: 진열대에 빵을 채우기
        ChangeMission(MissionState.FillShelf);
        yield return new WaitUntil(() => _hasFilledShelf);

        // 3단계: 카운터에서 계산하기
        ChangeMission(MissionState.GoToCounter);
        yield return new WaitUntil(() => _hasGoToCounter);

        // 4단계: 돈을 습득하기
        ChangeMission(MissionState.GetMoney);
        yield return new WaitUntil(() => _hasGetMoney);

        // 5단계: 테이블을 해방하기
        ChangeMission(MissionState.UpgradeTable);
        yield return new WaitUntil(() => _hasUpgradeTable);

        // 6단계: 쓰레기 생성 대기
        ChangeMission(MissionState.None);
        yield return new WaitUntil(() => _hasTrashOn);

        // 7단계: 테이블을 치우기
        ChangeMission(MissionState.ClearTable);
        yield return new WaitUntil(() => _hasClearTable);

        // 완료
        ChangeMission(MissionState.Completed);
    }

    private void ChangeMission(MissionState newState)
    {
        CurrentState = newState;
        Debug.Log($"[Mission] 상태 전환: {newState}");
        OnMissionChanged?.Invoke(newState);

        switch (newState)
        {
            case MissionState.None:
                pointer.SetTarget(null, 0f);
                break;
            case MissionState.GoToBread:
                pointer.SetTarget(breadPos, 2f);
                break;
            case MissionState.FillShelf:
                pointer.SetTarget(shelfPos, 2f);
                break;
            case MissionState.GoToCounter:
                pointer.SetTarget(counterPos, 2f);
                break;
            case MissionState.GetMoney:
                pointer.SetTarget(moneyPos, 2f);
                break;
            case MissionState.UpgradeTable:
                pointer.SetTarget(tablePos, 2f);
                break;
            case MissionState.ClearTable:
                pointer.SetTarget(trashPos, 2f);
                break;
            case MissionState.Completed:
                pointer.SetTarget(null, 0f); // 숨기기
                break;
            default:
                pointer.SetTarget(null, 0f);
                break;
        }
    }


    // ----------------------------
    // 외부에서 조건을 알려주는 방식
    // ----------------------------
    private bool _hasPickedBread = false;
    private bool _hasFilledShelf = false;
    private bool _hasGoToCounter = false;
    private bool _hasGetMoney = false;
    private bool _hasUpgradeTable = false;
    private bool _hasTrashOn = false;
    private bool _hasClearTable = false;

    public void NotifyBreadPicked()
    {
        _hasPickedBread = true;
    }
    public void NotifyShelfFilled()
    {
        _hasFilledShelf = true;
    }
    public void NotifyGoToCounter()
    {
        _hasGoToCounter = true;
    }
    public void NotifyGetMoney()
    {
        _hasGetMoney = true;
    }
    public void NotifyUpgradeTable()
    {
        _hasUpgradeTable = true;
    }
    public void NotifyTrashOn()
    {
        _hasTrashOn = true;
    }
    public void NotifyClearTable()
    {
        _hasClearTable = true;
    }
}
