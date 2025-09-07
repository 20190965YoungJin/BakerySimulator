using UnityEngine;
using TMPro;

public class MoneyUIUpdater : MonoBehaviour
{
    public TMP_Text moneyText; // 돈을 표시할 텍스트

    private void OnEnable()
    {
        // 이벤트에 구독하여 돈이 바뀔 때마다 UI 갱신
        MoneyManager.Instance.OnMoneyChanged += UpdateMoneyText;

        // 초기화
        UpdateMoneyText(MoneyManager.Instance.CurrentMoney);
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        MoneyManager.Instance.OnMoneyChanged -= UpdateMoneyText;
    }

    // 텍스트 갱신
    private void UpdateMoneyText(int currentMoney)
    {
        moneyText.text = $"{currentMoney:N0}"; // 천 단위 콤마 포함
    }
}
