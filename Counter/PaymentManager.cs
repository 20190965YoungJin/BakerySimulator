using System.Collections;
using UnityEngine;

public class PaymentManager : MonoBehaviour
{
    private bool isPaying = false;
    private bool isPlayerIn = false;

    public Transform paperbagPos;
    public GameObject paperbagPrefab;

    public MoneyStacker moneyStacker;

    private int breadPrice = 0;

    public bool CanPay()
    {
        return !isPaying && isPlayerIn;
    }

    public void StartPayment(CustomerAi customer)
    {
        StartCoroutine(PaymentRoutine(customer));
        MissionManager.Instance.NotifyGoToCounter();
    }

    IEnumerator PaymentRoutine(CustomerAi customer)
    {
        isPaying = true;

        // Step 1. ���� ����
        GameObject bag = Instantiate(paperbagPrefab, paperbagPos.position, Quaternion.identity);

        // Step 2. �� �ֱ� (�ִϸ��̼� � �̵� + ����)
        for (int i = 0; i < customer.stackBread.maxStackSize; i++)
        {
            var bread = customer.stackBread.PopBread();
            if (bread == null) break;

            // ���� ������ ����� �̵�
            yield return BreadMover.WorldMoveBreadCurve(bread, bread.transform.position, paperbagPos.position, 0.1f);

            // �̵� �� ����
            Destroy(bread);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.7f);

        // Step 3. ������ �մԿ��� ���� (������ � �̵�)
        Vector3 targetPos = customer.transform.position + new Vector3(0, 0.813f, 0.368f);
        yield return BreadMover.LocalMoveBreadCurve(bag, bag.transform.position, targetPos, 0.2f);

        bag.transform.SetParent(customer.transform);
        bag.transform.localPosition = new Vector3(0, 0.813f, 0.368f);
        bag.transform.localEulerAngles = Vector3.zero;

        customer.GetComponent<Animator>().SetBool("isStack", true);

        // Step 4. �� ���
        SoundManager.Instance.Play("cash");
        customer.isBuy = true;
        isPaying = false;

        breadPrice = Random.Range(15, 20);
        moneyStacker.SetMoney(breadPrice);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerIn = false;
    }
}
