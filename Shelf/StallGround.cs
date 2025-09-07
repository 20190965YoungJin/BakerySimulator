using System.Collections;
using UnityEngine;

public class StallGround : MonoBehaviour
{
    public Stall stall;
    public StackBread stackBread;
    private Coroutine activeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCoroutine = StartCoroutine(PlayerInGround());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
            stall.isSupply = false;
        }
    }

    private IEnumerator PlayerInGround()
    {
        stall.isSupply = true;

        while (stall.HasEmptySlot())
        {
            yield return new WaitForSeconds(0.2f);

            GameObject poppedBread = stackBread.PopBread();
            if (poppedBread != null)
            {
                stall.PushBread(poppedBread);
            }
            else
            {
                break; // 빵 없으면 반복 종료
            }
        }

        stall.isSupply = false;
    }

}
