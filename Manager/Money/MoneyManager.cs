using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public int CurrentMoney { get; private set; }

    public event System.Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        OnMoneyChanged?.Invoke(CurrentMoney); // UI 등 외부에 알림

        CurrentMoney += amount;
    }

    public bool UseMoney(int amount)
    {
        if (CurrentMoney < amount) return false;

        CurrentMoney -= amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
        return true;
    }

    // 현재 돈이 특정 값 이상인지 확인하는 판정 함수
    public bool HasMoney(int amount = 1)
    {
        return CurrentMoney >= amount;
    }

}
