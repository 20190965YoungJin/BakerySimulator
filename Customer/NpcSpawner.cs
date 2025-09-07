using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC를 일정 시간마다 소환하는 스폰 매니저.
/// 오브젝트 풀을 사용하여 성능 최적화.
/// </summary>
public class NpcSpawner : MonoBehaviour
{
    [Header("소환 설정")]
    public GameObject npcPrefab;            // 소환할 NPC 프리팹
    public int maxPoolSize = 10;            // 오브젝트 풀 최대 크기
    public int maxActiveNpc = 5;            // 동시에 활성화 가능한 최대 NPC 수

    [Header("연결된 시스템")]
    public PaymentManager paymentManager;
    public Stall stall;
    public StackBread stackBread;
    public TableSeatController tableSeatController;
    public Transform shelfPosition;
    public Transform takeoutPosition;
    public Transform counterPosition;

    [Header("오브젝트 풀")]
    public List<GameObject> npcPool = new List<GameObject>();

    private bool isGameOver = false;

    void Awake()
    {
        // 서비스 형태로 시스템들 등록
        GameServices.RegisterServices(paymentManager, stall, stackBread, tableSeatController, shelfPosition, takeoutPosition, counterPosition);
    }

    void Start()
    {
        CreateNpcPool();
        StartCoroutine(SpawnNpcLoop());
    }

    /// <summary>
    /// NPC 오브젝트 풀 생성 및 초기화
    /// </summary>
    private void CreateNpcPool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject npc = Instantiate(npcPrefab);
            npc.SetActive(false);

            var customer = npc.GetComponent<ICustomerDependency>();
            if (customer != null)
            {
                customer.Initialize(stall, stackBread, paymentManager, tableSeatController, shelfPosition, takeoutPosition, counterPosition);
            }

            npcPool.Add(npc);
        }
    }

    /// <summary>
    /// 일정 시간 간격으로 NPC를 활성화 (소환)
    /// </summary>
    private IEnumerator SpawnNpcLoop()
    {
        while (!isGameOver)
        {
            // 다음 소환까지 대기
            float delay = Random.Range(2f, 7f);
            yield return new WaitForSeconds(delay);

            if (isGameOver) yield break;

            // 현재 활성화된 NPC 수 확인
            int activeCount = npcPool.FindAll(npc => npc.activeSelf).Count;

            if (activeCount >= maxActiveNpc)
                continue;

            // 비활성화된 NPC 중 하나를 활성화
            GameObject nextNpc = npcPool.Find(npc => !npc.activeSelf);
            if (nextNpc != null)
            {
                nextNpc.transform.SetPositionAndRotation(transform.position, transform.rotation);
                nextNpc.SetActive(true);
            }
        }
    }
}
