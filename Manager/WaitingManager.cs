using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 진열대, 계산대, 테이크아웃, 테이블 줄을 통합 관리하는 싱글톤 대기열 관리자
/// </summary>
public class WaitingManager : MonoBehaviour
{
    public static WaitingManager Instance { get; private set; }

    [Header("줄 위치들")]
    public List<Transform> shelfLinePositions;
    public List<Transform> counterLinePositions;
    public List<Transform> takeoutLinePositions;
    public List<Transform> tablesLinePositions;

    [Header("줄 점유 상태")]
    public List<bool> shelfOccupied;
    public List<bool> counterOccupied;
    public List<bool> takeoutOccupied;
    public List<bool> tablesOccupied;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 각 줄에 맞춰 점유 리스트 초기화
        shelfOccupied = new List<bool>(new bool[shelfLinePositions.Count]);
        counterOccupied = new List<bool>(new bool[counterLinePositions.Count]);
        takeoutOccupied = new List<bool>(new bool[takeoutLinePositions.Count]);
        tablesOccupied = new List<bool>(new bool[tablesLinePositions.Count]);
    }

    // ========================
    // 줄 할당 및 해제 메서드들
    // ========================

    public Transform GetNextShelfPosition() => GetNextPosition(shelfLinePositions, shelfOccupied);
    public void ReleaseShelfPosition(Transform pos) => ReleasePosition(pos, shelfLinePositions, shelfOccupied);

    public Transform GetNextCounterPosition() => GetNextPosition(counterLinePositions, counterOccupied);
    public void ReleaseCounterPosition(Transform pos) => ReleasePosition(pos, counterLinePositions, counterOccupied);

    public Transform GetNextTakeoutPosition() => GetNextPosition(takeoutLinePositions, takeoutOccupied);
    public void ReleaseTakeoutPosition(Transform pos) => ReleasePosition(pos, takeoutLinePositions, takeoutOccupied);

    public Transform GetNextTablePosition() => GetNextPosition(tablesLinePositions, tablesOccupied);
    public void ReleaseTablePosition(Transform pos) => ReleasePosition(pos, tablesLinePositions, tablesOccupied);

    // ========================
    // 줄 선 위치 확인 메서드들
    // ========================

    public bool IsFirstInShelfLine(Transform pos) => IsFirstInLine(pos, shelfLinePositions, shelfOccupied);
    public bool IsFirstInCounterLine(Transform pos) => IsFirstInLine(pos, counterLinePositions, counterOccupied);
    public bool IsFirstInTakeoutLine(Transform pos) => IsFirstInLine(pos, takeoutLinePositions, takeoutOccupied);
    public bool IsFirstInTableLine(Transform pos) => IsFirstInLine(pos, tablesLinePositions, tablesOccupied);

    // ========================
    // 줄의 앞자리가 비었는지 확인
    // ========================

    public bool IsFrontSpotEmpty(Transform myPos)
    {
        if (myPos == null) return false;

        return CheckFrontEmpty(myPos, shelfLinePositions, shelfOccupied)
            || CheckFrontEmpty(myPos, counterLinePositions, counterOccupied)
            || CheckFrontEmpty(myPos, takeoutLinePositions, takeoutOccupied)
            || CheckFrontEmpty(myPos, tablesLinePositions, tablesOccupied);
    }

    // ========================
    // 내부 공통 처리 함수들
    // ========================

    private Transform GetNextPosition(List<Transform> line, List<bool> occupied)
    {
        for (int i = 0; i < line.Count; i++)
        {
            if (!occupied[i])
            {
                occupied[i] = true;
                return line[i];
            }
        }
        return null;
    }

    private void ReleasePosition(Transform pos, List<Transform> line, List<bool> occupied)
    {
        int index = line.IndexOf(pos);
        if (index >= 0) occupied[index] = false;
    }

    private bool IsFirstInLine(Transform pos, List<Transform> line, List<bool> occupied)
    {
        if (pos == null) return false;

        for (int i = 0; i < line.Count; i++)
        {
            if (!occupied[i]) continue;
            return line[i] == pos; // 첫 번째 점유 자리가 내 자리면 true
        }

        return false;
    }

    private bool CheckFrontEmpty(Transform pos, List<Transform> line, List<bool> occupied)
    {
        int index = line.IndexOf(pos);
        return index > 0 && !occupied[index - 1];
    }
}
