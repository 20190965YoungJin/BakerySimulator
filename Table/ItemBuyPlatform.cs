using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public class ItemBuyPlatform : MonoBehaviour
{
    [Header("Item Settings")]
    public int itemPrice = 30;                   // 총 아이템 가격
    public float payInterval = 0.1f;             // 돈이 빠지는 간격
    public UnityEvent onPurchaseComplete;        // 구매 완료 시 실행할 이벤트

    private int currentPaid = 0;                 // 현재까지 지불한 돈
    private bool isPurchasing = false;           // 현재 구매 중인지 여부
    public TMP_Text costText; // 돈을 표시할 텍스트
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

    // 현재 진행 상황 리셋
    public void ResetPurchase()
    {
        currentPaid = 0;
    }
}
