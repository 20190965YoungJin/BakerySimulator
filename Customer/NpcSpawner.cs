using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC�� ���� �ð����� ��ȯ�ϴ� ���� �Ŵ���.
/// ������Ʈ Ǯ�� ����Ͽ� ���� ����ȭ.
/// </summary>
public class NpcSpawner : MonoBehaviour
{
    [Header("��ȯ ����")]
    public GameObject npcPrefab;            // ��ȯ�� NPC ������
    public int maxPoolSize = 10;            // ������Ʈ Ǯ �ִ� ũ��
    public int maxActiveNpc = 5;            // ���ÿ� Ȱ��ȭ ������ �ִ� NPC ��

    [Header("����� �ý���")]
    public PaymentManager paymentManager;
    public Stall stall;
    public StackBread stackBread;
    public TableSeatController tableSeatController;
    public Transform shelfPosition;
    public Transform takeoutPosition;
    public Transform counterPosition;

    [Header("������Ʈ Ǯ")]
    public List<GameObject> npcPool = new List<GameObject>();

    private bool isGameOver = false;

    void Awake()
    {
        // ���� ���·� �ý��۵� ���
        GameServices.RegisterServices(paymentManager, stall, stackBread, tableSeatController, shelfPosition, takeoutPosition, counterPosition);
    }

    void Start()
    {
        CreateNpcPool();
        StartCoroutine(SpawnNpcLoop());
    }

    /// <summary>
    /// NPC ������Ʈ Ǯ ���� �� �ʱ�ȭ
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
    /// ���� �ð� �������� NPC�� Ȱ��ȭ (��ȯ)
    /// </summary>
    private IEnumerator SpawnNpcLoop()
    {
        while (!isGameOver)
        {
            // ���� ��ȯ���� ���
            float delay = Random.Range(2f, 7f);
            yield return new WaitForSeconds(delay);

            if (isGameOver) yield break;

            // ���� Ȱ��ȭ�� NPC �� Ȯ��
            int activeCount = npcPool.FindAll(npc => npc.activeSelf).Count;

            if (activeCount >= maxActiveNpc)
                continue;

            // ��Ȱ��ȭ�� NPC �� �ϳ��� Ȱ��ȭ
            GameObject nextNpc = npcPool.Find(npc => !npc.activeSelf);
            if (nextNpc != null)
            {
                nextNpc.transform.SetPositionAndRotation(transform.position, transform.rotation);
                nextNpc.SetActive(true);
            }
        }
    }
}
