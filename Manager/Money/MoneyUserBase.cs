using UnityEngine;

public abstract class MoneyUserBase : MonoBehaviour
{
    public int Cost;

    // 돈을 사용할 수 있으면 차감 후 기능 실행
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
