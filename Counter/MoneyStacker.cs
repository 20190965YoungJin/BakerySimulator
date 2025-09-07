using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ���� ���� �� ������Ʈ�� �׾� �ø���,
/// �÷��̾ Ʈ���ſ� �����ϸ� ���� ������ �� ����.
/// </summary>
public class MoneyStacker : MonoBehaviour
{
    [Header("�� ������Ʈ ����")]
    public List<GameObject> moneyObjects;         // �̸� ��ġ�� �� ������Ʈ �����յ�
    private List<Vector3> moneyPositions = new(); // �� ���̴� ��ġ ��갪

    [Header("���� ��")]
    public Transform playerTransform;             // �÷��̾� ��ġ

    public int currentMoneyCount = 0;            // ���� ���õ� �� ����

    private void Start()
    {
        InitPlayerTransform();
        InitializeMoneyPositions();
        DeactivateAllMoney();
    }

    /// <summary>
    /// �ܺο��� ���� ���ÿ� �߰�
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
    /// �÷��̾�� ����� �� ��� ���� �÷��̾�� �����ϰ� �ʱ�ȭ
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
    /// �� ������Ʈ�� �÷��̾� ������ �̵� �� ��Ȱ��ȭ
    /// </summary>
    private IEnumerator MoveToPlayerAndBack(GameObject money, Vector3 originalPos)
    {
        yield return BreadMover.WorldMoveBreadCurve(money, originalPos, playerTransform.position, 0.2f, 4f);
        money.SetActive(false);
    }

    /// <summary>
    /// ���� ���̴� ��ġ�� �̸� ����� ����Ʈ�� ����
    /// </summary>
    private void InitializeMoneyPositions()
    {
        for (int i = 0; i < moneyObjects.Count; i++)
        {
            moneyPositions.Add(CalculateMoneyPosition(i));
        }
    }

    /// <summary>
    /// ó���� ��� �� ������Ʈ ��Ȱ��ȭ
    /// </summary>
    private void DeactivateAllMoney()
    {
        foreach (var money in moneyObjects)
            money.SetActive(false);
    }

    /// <summary>
    /// �÷��̾� Transform�� �������� �ʾ����� �ڵ����� ã�Ƽ� ����
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
    /// ���� ���� ��ġ ���
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
