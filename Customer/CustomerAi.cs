using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// CustomerAi는 손님의 행동 상태를 관리합니다.
/// </summary>

public class CustomerAi : MonoBehaviour, ICustomerDependency
{
    // 진열대, 카운터, 테이크아웃 오브젝트 위치
    public List<Transform> lookTargets = new List<Transform>();

    // 빵 관련
    [HideInInspector] public int breadTargetCount;
    [HideInInspector] public int breadCount = 0;
    [HideInInspector] public bool getBread = false;

    // 이용 방식
    [HideInInspector] public bool isTakeout = false;
    [HideInInspector] public bool isBuy = false;
    private bool isEngry = false;

    // 상태 관리
    private bool isInitialized = false;
    private bool isRunningStateCoroutine = false;
    private Coroutine stateCoroutine;

    // 위치 및 이동
    private NavMeshAgent agent;
    [HideInInspector] public Transform myPos;
    private bool isArrived = false;
    private bool isDestination = false;
    private int tryCount = 0; // 위치 배정 실패 횟수

    // 시스템 참조
    private Stall stall;
    [HideInInspector] public StackBread stackBread;
    private PaymentManager paymentManager;
    private TableSeatController tableSeatController;
    [HideInInspector] public EmoteController emoteController;

    // 테이블 상태 (전역 공유)
    private static int takeoutCustomerCount = 0;
    public static bool isTable = false;
    public static bool useTable = false;
    public static bool isClearTrash = true;


    public void Initialize(Stall stall, StackBread stackBread, PaymentManager paymentManager, TableSeatController tableSeatController, Transform shelfPos, Transform takeoutPos, Transform counterPos)
    {
        this.stall = stall;
        this.paymentManager = paymentManager;
        this.tableSeatController = tableSeatController;

        lookTargets.Add(shelfPos);
        lookTargets.Add(takeoutPos);
        lookTargets.Add(counterPos);
    }

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (stackBread == null) stackBread = GetComponentInChildren<StackBread>();
        if (emoteController == null) GetComponent<EmoteController>();
    }

    void OnEnable()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            return;
        }

        InitializeCustomerStatus(); // 상태 초기화

        if (!isRunningStateCoroutine)
        {
            isRunningStateCoroutine = true;
            stateCoroutine = StartCoroutine(CheckCustomerStateLoop());
            ChangeState(State.MOVE);
        }
    }

    void OnDisable()
    {
        CancelInvoke(nameof(SetmyPos));
        ResetCustomerStatus();

        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
            isRunningStateCoroutine = false;
        }
    }

    private void InitializeCustomerStatus()
    {
        breadTargetCount = Random.Range(1, 3);
        isTakeout = DecideTakeout();
        breadCount = 0;
        getBread = false;
        isBuy = false;
        isEngry = false;
    }

    private void ResetCustomerStatus()
    {
        isArrived = false;
        isDestination = false;
        myPos = null;
        tryCount = 0;
    }


    #region FSM
    public enum State
    {
        GETBREAD,
        PAY,
        MOVE,
        WAIT,
        EATIN,
        EXIT
    }

    public State state;

    IEnumerator CheckCustomerStateLoop()
    {
        yield return new WaitForSeconds(0.5f);

        switch (state)
        {
            case State.GETBREAD: GETBREAD(); break;
            case State.PAY: PAY(); break;
            case State.MOVE: MOVE(); break;
            case State.WAIT: WAIT(); break;
            case State.EATIN: EATIN(); break;
            case State.EXIT: Exit(); break;
        }

        if (gameObject.activeSelf)
            StartCoroutine(CheckCustomerStateLoop());
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (state)
        {
            case State.GETBREAD: GETBREADTrigger(other); break;
            case State.PAY: PAYTrigger(other); break;
            case State.MOVE: MOVETrigger(other); break;
            case State.WAIT: WAITTrigger(other); break;
            default: break;
        }
    }

    public void ChangeState(State state)
    {
        switch (this.state)
        {
            case State.GETBREAD: GETBREADExit(); break;
            case State.PAY: PAYExit(); break;
            case State.MOVE: MOVEExit(); break;
            case State.WAIT: WAITExit(); break;
            default: break;
        }

        this.state = state;

        switch (state)
        {
            case State.GETBREAD: GETBREADEnter(); break;
            case State.PAY: PAYEnter(); break;
            case State.MOVE: MOVEEnter(); break;
            case State.WAIT: WAITEnter(); break;
            case State.EATIN: EATINEnter(); break;
            case State.EXIT: ExitEnter(); break;
            default: break;
        }
    }


    #region PAY
    private void PAYEnter()
    {
    }
    private void PAY()
    {
        if (isBuy)
        {
            // 계산 했으면 퇴장
            WaitingManager.Instance.ReleaseTakeoutPosition(myPos);
            myPos = null;
            ChangeState(State.EXIT);
            emoteController.UpdateEmote();
            return;
        }
        else
        {
            // 계산대에 계산 요청
            if (paymentManager.CanPay())
            {
                paymentManager.StartPayment(this);
            }
        }
    }
    private void PAYTrigger(Collider other)
    {

    }
    private void PAYExit()
    {
        
    }
    #endregion


    #region GETBREAD
    private void GETBREADEnter()
    {
        StartCoroutine(PickBread());
    }
    private void GETBREAD()
    {
        if(breadTargetCount == breadCount)
        {
            getBread = true;
            ChangeState(State.MOVE);
            emoteController.UpdateEmote();
        }
    }
    private void GETBREADTrigger(Collider other)
    {

    }
    private void GETBREADExit()
    {
        // 자리 비었다 알리기
        WaitingManager.Instance.ReleaseShelfPosition(myPos);
    }
    #endregion


    #region MOVE
    private void MOVEEnter()
    {
        // 매니저한테 줄 배정받기
        SetmyPos();
    }
    private void MOVE()
    {
        //도착했는지 체크
        CheckArrival();
        
        if (isArrived)
        {
            ChangeState(State.WAIT);
        }
    }
    private void MOVETrigger(Collider other)
    {

    }
    private void MOVEExit()
    {
        isArrived = false;
    }
    #endregion


    #region WAIT
    private void WAITEnter()
    {
        StartCoroutine(ResetPathAndWait());
    }
    private void WAIT()
    {
        // 지정된 자리가 없으면
        if(myPos == null)
        {
            ChangeState(State.MOVE);
        }


        // 앞 줄이 비었는지 확인하고 이동
        if (WaitingManager.Instance.IsFrontSpotEmpty(myPos))
        {
            ChangeState(State.MOVE);
        }

        // 줄의 가장 앞이면 계산 시작
        if (getBread && isTakeout && WaitingManager.Instance.IsFirstInTakeoutLine(myPos))
        {
            ChangeState(State.PAY);
        }
        else if (getBread && !isTakeout && WaitingManager.Instance.IsFirstInCounterLine(myPos))
        {
            // 테이블 쓰레기 체크
            CheckTableActive();
            if (isTable && !useTable && isClearTrash)
            {
                ChangeState(State.EATIN);
                useTable = true;
            }
        }
        else if (!getBread && WaitingManager.Instance.IsFirstInShelfLine(myPos))
        {
            ChangeState(State.GETBREAD);
        }
    }

    private void WAITTrigger(Collider other)
    {

    }
    private void WAITExit()
    {

    }
    #endregion

    #region EATIN
    private void EATINEnter()
    {
        // 카운터 자리 해제
        WaitingManager.Instance.ReleaseCounterPosition(myPos);

        // 테이블로 가기
        myPos = WaitingManager.Instance.GetNextTablePosition();
        if(myPos != null)
        {
            agent.SetDestination(myPos.position);
            isDestination = true;
        }

        // 이모트 끄기
        isBuy = true;
        emoteController.UpdateEmote();
    }
    private void EATIN()
    {
        // 도착 판정
        CheckArrival();

        if (isArrived)
        {
            EatBreadInTable();
            isArrived = false;
        }
    }
    #endregion


    #region EXIT
    private void ExitEnter()
    {
        // 이모트 실행
        if(!isEngry)
            emoteController.emojiSmile.Play();
        // 출구로 향하기
        Vector3 ExitPos = new Vector3(0.182f, 1.377f, -11f);
        agent.SetDestination(ExitPos);
        isDestination = true;
    }
    private void Exit()
    {
        // 상점 나가기
        CheckArrival();
        if (isArrived)
        {
            StartCoroutine(ResetPathAndWait());
            this.gameObject.SetActive(false);
        }
    }
    #endregion

    //end FSM
    #endregion

    IEnumerator ResetPathAndWait()
    {
        // 경로 초기화
        agent.ResetPath();

        // 경로가 완전히 초기화될 때까지 대기
        yield return new WaitUntil(() => agent.path == null);
        yield return null;
    }

    void CheckArrival()
    {
        // 아직 경로를 계산 중이면 도착 안 한 상태
        if (agent.pathPending)
        {
            isArrived = false; return;
        }

        if (agent.remainingDistance <= 0.3f && isDestination && agent.velocity.sqrMagnitude <= 0.1f)
        {
            isDestination = false;
            isArrived = true;

            LookAtClosestTarget();

            if (!getBread)
            {
                emoteController.UpdateEmote();
            }
        }
    }

    // 매니저한테 줄 배정 받기
    void SetmyPos()
    {
        Transform newPos = null;

        // 새 자리 배정 시도
        if (getBread)
        {
            newPos = isTakeout
                ? WaitingManager.Instance.GetNextTakeoutPosition()
                : WaitingManager.Instance.GetNextCounterPosition();
        }
        else
        {
            newPos = WaitingManager.Instance.GetNextShelfPosition();
        }

        // 배정 실패 시 재시도
        if (newPos == null)
        {
            tryCount++;
            if (tryCount < 10)
            {
                Invoke(nameof(SetmyPos), 1f);
            }
            else
            {
                isEngry = true;
                ChangeState(State.EXIT);
            }
            return;
        }

        // 자리 확보 성공 후 기존 자리 해제
        if (myPos != null)
        {
            WaitingManager.Instance.ReleaseShelfPosition(myPos);
            WaitingManager.Instance.ReleaseTakeoutPosition(myPos);
            WaitingManager.Instance.ReleaseCounterPosition(myPos);
        }

        // 이동
        myPos = newPos;
        agent.SetDestination(myPos.position);
        isDestination = true;
        tryCount = 0;

        //Debug.Log($"[{name}] 배정된 위치: {myPos.name}, 목적지 설정 완료");
    }

    // 빵 집기
    IEnumerator PickBread()
    {
        yield return new WaitForSeconds(0.2f);

        if (!stall.isSupply)
        {
            if (breadCount < breadTargetCount)
            {
                if (stall.HasBread())
                {
                    GameObject poppedBread = stall.PopBread();
                    if (poppedBread != null)
                    {
                        stackBread.PushBread(poppedBread);
                        breadCount++;
                    }
                }
                StartCoroutine(PickBread());
            }
            else
            {
                yield return null;
            }
        }
        else
        {
            StartCoroutine(PickBread());
        }
    }

    void CheckTableActive() // 테이블 언락 여부
    {
        if (tableSeatController != null && tableSeatController.gameObject.activeInHierarchy)
        {
            isTable = true;

            if (tableSeatController.trash.gameObject.activeSelf)
                isClearTrash = false;
            else
                isClearTrash = true;
        }
    }

    private void EatBreadInTable() // 테이블 식사 시작
    {
        stackBread.ClearBread();
        tableSeatController.OnCustomerSitting(gameObject);
        Invoke("EatFinsh", 4f);
    }

    private void EatFinsh()
    {
        useTable = false;
        WaitingManager.Instance.ReleaseTablePosition(myPos);
        ChangeState(State.EXIT);
    }

    public void LookAtClosestTarget()
    {
        if (lookTargets == null || lookTargets.Count == 0)
            return;

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in lookTargets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            Vector3 lookPos = closestTarget.position;
            lookPos.y = transform.position.y; // y축 고정

            transform.LookAt(lookPos);
        }
    }

    /// <summary>
    /// 테이크아웃 여부를 결정하는 메서드
    /// </summary>
    private static bool DecideTakeout()
    {
        if (takeoutCustomerCount >= 3)
        {
            // 연속 3번 이상 테이크아웃 했으면 강제 false
            takeoutCustomerCount = 0;
            return false;
        }

        bool result = Random.value > 0.3f; // 70% 확률로 true

        if (result)
        {
            takeoutCustomerCount++;
        }
        else
        {
            takeoutCustomerCount = 0;
        }

        return result;
    }
}

