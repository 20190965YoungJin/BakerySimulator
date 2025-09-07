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
        // 1�ܰ�: �� ���� �ϱ�
        ChangeMission(MissionState.GoToBread);
        yield return new WaitUntil(() => _hasPickedBread);

        // 2�ܰ�: �����뿡 ���� ä���
        ChangeMission(MissionState.FillShelf);
        yield return new WaitUntil(() => _hasFilledShelf);

        // 3�ܰ�: ī���Ϳ��� ����ϱ�
        ChangeMission(MissionState.GoToCounter);
        yield return new WaitUntil(() => _hasGoToCounter);

        // 4�ܰ�: ���� �����ϱ�
        ChangeMission(MissionState.GetMoney);
        yield return new WaitUntil(() => _hasGetMoney);

        // 5�ܰ�: ���̺��� �ع��ϱ�
        ChangeMission(MissionState.UpgradeTable);
        yield return new WaitUntil(() => _hasUpgradeTable);

        // 6�ܰ�: ������ ���� ���
        ChangeMission(MissionState.None);
        yield return new WaitUntil(() => _hasTrashOn);

        // 7�ܰ�: ���̺��� ġ���
        ChangeMission(MissionState.ClearTable);
        yield return new WaitUntil(() => _hasClearTable);

        // �Ϸ�
        ChangeMission(MissionState.Completed);
    }

    private void ChangeMission(MissionState newState)
    {
        CurrentState = newState;
        Debug.Log($"[Mission] ���� ��ȯ: {newState}");
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
                pointer.SetTarget(null, 0f); // �����
                break;
            default:
                pointer.SetTarget(null, 0f);
                break;
        }
    }


    // ----------------------------
    // �ܺο��� ������ �˷��ִ� ���
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
