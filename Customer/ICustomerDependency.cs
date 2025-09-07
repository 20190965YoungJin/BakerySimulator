using UnityEngine;

public interface ICustomerDependency
{
    void Initialize(Stall stall, StackBread stackBread, PaymentManager paymentManager, TableSeatController tableSeat, Transform shelfPos, Transform takeoutPos, Transform counterPos);
}
