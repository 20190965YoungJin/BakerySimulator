using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public class ItemBuyPlatform : MonoBehaviour
{
    [Header("Item Settings")]
    public int itemPrice = 30;                   // �� ������ ����
    public float payInterval = 0.1f;             // ���� ������ ����
    public UnityEvent onPurchaseComplete;        // ���� �Ϸ� �� ������ �̺�Ʈ

    private int currentPaid = 0;                 // ������� ������ ��
    private bool isPurchasing = false;           // ���� ���� ������ ����
    public TMP_Text costText; // ���� ǥ���� �ؽ�Ʈ
    private Coroutine purchaseCoroutine;

    void Start()
    {
        UpdateCostText();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isPurchasing && currentPaid < itemPrice)
        {
            purchaseCoroutine = StartCoroutine(ConsumeMoneyCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPurchasing)
        {
            StopCoroutine(purchaseCoroutine);
            isPurchasing = false;
        }
    }

    private IEnumerator ConsumeMoneyCoroutine()
    {
        isPurchasing = true;

        while (currentPaid < itemPrice)
        {
            if (!MoneyManager.Instance.HasMoney()) break;

            MoneyManager.Instance.UseMoney(1);
            currentPaid += 1;
            UpdateCostText();

            if (currentPaid >= itemPrice)
            {
                onPurchaseComplete.Invoke();
                SoundManager.Instance.Play("Success");
                MissionManager.Instance.NotifyUpgradeTable();
                EventBus.UnlockTableEvent();
                break;
            }

            yield return new WaitForSeconds(payInterval);
        }

        isPurchasing = false;
    }

    private void UpdateCostText()
    {
        if (costText != null)
        {
            int remaining = Mathf.Max(itemPrice - currentPaid, 0);
            costText.text = remaining.ToString();
        }
    }

    // ���� ���� ��Ȳ ����
    public void ResetPurchase()
    {
        currentPaid = 0;
    }
}
