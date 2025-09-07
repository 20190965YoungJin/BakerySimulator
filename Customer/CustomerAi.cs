using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// CustomerAi�� �մ��� �ൿ ���¸� �����մϴ�.
/// </summary>

public class CustomerAi : MonoBehaviour, ICustomerDependency
{
    // ������, ī����, ����ũ�ƿ� ������Ʈ ��ġ
    public List<Transform> lookTargets = new List<Transform>();

    // �� ����
    [HideInInspector] public int breadTargetCount;
    [HideInInspector] public int breadCount = 0;
    [HideInInspector] public bool getBread = false;

    // �̿� ���
    [HideInInspector] public bool isTakeout = false;
    [HideInInspector] public bool isBuy = false;
    private bool isEngry = false;

    // ���� ����
    private bool isInitialized = false;
    private bool isRunningStateCoroutine = false;
    private Coroutine stateCoroutine;

    // ��ġ �� �̵�
    private NavMeshAgent agent;
    [HideInInspector] public Transform myPos;
    private bool isArrived = false;
    private bool isDestination = false;
    private int tryCount = 0; // ��ġ ���� ���� Ƚ��

    // �ý��� ����
    private Stall stall;
    [HideInInspector] public StackBread stackBread;
    private PaymentManager paymentManager;
    private TableSeatController tableSeatController;
    [HideInInspector] public EmoteController emoteController;

    // ���̺� ���� (���� ����)
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

        InitializeCustomerStatus(); // ���� �ʱ�ȭ

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
            // ��� ������ ����
            WaitingManager.Instance.ReleaseTakeoutPosition(myPos);
            myPos = null;
            ChangeState(State.EXIT);
            emoteController.UpdateEmote();
            return;
        }
        else
        {
            // ���뿡 ��� ��û
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
        // �ڸ� ����� �˸���
        WaitingManager.Instance.ReleaseShelfPosition(myPos);
    }
    #endregion


    #region MOVE
    private void MOVEEnter()
    {
        // �Ŵ������� �� �����ޱ�
        SetmyPos();
    }
    private void MOVE()
    {
        //�����ߴ��� üũ
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
        // ������ �ڸ��� ������
        if(myPos == null)
        {
            ChangeState(State.MOVE);
        }


        // �� ���� ������� Ȯ���ϰ� �̵�
        if (WaitingManager.Instance.IsFrontSpotEmpty(myPos))
        {
            ChangeState(State.MOVE);
        }

        // ���� ���� ���̸� ��� ����
        if (getBread && isTakeout && WaitingManager.Instance.IsFirstInTakeoutLine(myPos))
        {
            ChangeState(State.PAY);
        }
        else if (getBread && !isTakeout && WaitingManager.Instance.IsFirstInCounterLine(myPos))
        {
            // ���̺� ������ üũ
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
        // ī���� �ڸ� ����
        WaitingManager.Instance.ReleaseCounterPosition(myPos);

        // ���̺�� ����
        myPos = WaitingManager.Instance.GetNextTablePosition();
        if(myPos != null)
        {
            agent.SetDestination(myPos.position);
            isDestination = true;
        }

        // �̸�Ʈ ����
        isBuy = true;
        emoteController.UpdateEmote();
    }
    private void EATIN()
    {
        // ���� ����
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
        // �̸�Ʈ ����
        if(!isEngry)
            emoteController.emojiSmile.Play();
        // �ⱸ�� ���ϱ�
        Vector3 ExitPos = new Vector3(0.182f, 1.377f, -11f);
        agent.SetDestination(ExitPos);
        isDestination = true;
    }
    private void Exit()
    {
        // ���� ������
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
        // ��� �ʱ�ȭ
        agent.ResetPath();

        // ��ΰ� ������ �ʱ�ȭ�� ������ ���
        yield return new WaitUntil(() => agent.path == null);
        yield return null;
    }

    void CheckArrival()
    {
        // ���� ��θ� ��� ���̸� ���� �� �� ����
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

    // �Ŵ������� �� ���� �ޱ�
    void SetmyPos()
    {
        Transform newPos = null;

        // �� �ڸ� ���� �õ�
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

        // ���� ���� �� ��õ�
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

        // �ڸ� Ȯ�� ���� �� ���� �ڸ� ����
        if (myPos != null)
        {
            WaitingManager.Instance.ReleaseShelfPosition(myPos);
            WaitingManager.Instance.ReleaseTakeoutPosition(myPos);
            WaitingManager.Instance.ReleaseCounterPosition(myPos);
        }

        // �̵�
        myPos = newPos;
        agent.SetDestination(myPos.position);
        isDestination = true;
        tryCount = 0;

        //Debug.Log($"[{name}] ������ ��ġ: {myPos.name}, ������ ���� �Ϸ�");
    }

    // �� ����
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

    void CheckTableActive() // ���̺� ��� ����
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

    private void EatBreadInTable() // ���̺� �Ļ� ����
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
            lookPos.y = transform.position.y; // y�� ����

            transform.LookAt(lookPos);
        }
    }

    /// <summary>
    /// ����ũ�ƿ� ���θ� �����ϴ� �޼���
    /// </summary>
    private static bool DecideTakeout()
    {
        if (takeoutCustomerCount >= 3)
        {
            // ���� 3�� �̻� ����ũ�ƿ� ������ ���� false
            takeoutCustomerCount = 0;
            return false;
        }

        bool result = Random.value > 0.3f; // 70% Ȯ���� true

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

