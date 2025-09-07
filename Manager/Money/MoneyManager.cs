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
        OnMoneyChanged?.Invoke(CurrentMoney); // UI �� �ܺο� �˸�

        CurrentMoney += amount;
    }

    public bool UseMoney(int amount)
    {
        if (CurrentMoney < amount) return false;

        CurrentMoney -= amount;
        OnMoneyChanged?.Invoke(CurrentMoney);
        return true;
    }

    // ���� ���� Ư�� �� �̻����� Ȯ���ϴ� ���� �Լ�
    public bool HasMoney(int amount = 1)
    {
        return CurrentMoney >= amount;
    }

}
