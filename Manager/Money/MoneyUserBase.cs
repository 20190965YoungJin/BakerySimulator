using UnityEngine;

public abstract class MoneyUserBase : MonoBehaviour
{
    public int Cost;

    // ���� ����� �� ������ ���� �� ��� ����
    public bool TryUseMoney()
    {
        if (MoneyManager.Instance.UseMoney(Cost))
        {
            OnMoneyUsed();
            return true;
        }

        return false;
    }

    protected abstract void OnMoneyUsed();
}
