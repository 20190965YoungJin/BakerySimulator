using System.Collections;
using UnityEngine;

public class BakeryGround : MonoBehaviour
{
    public BreadBox breadBox;
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
        }
    }

    private IEnumerator PlayerInGround()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (stackBread.MaxStackSize())
            {
                GameObject poppedBread = breadBox.PopBread();
                if (poppedBread != null)
                {
                    stackBread.PushBread(poppedBread);
                }
            }
            else
            {
                yield return null;
            }
            
        }
    }
}
