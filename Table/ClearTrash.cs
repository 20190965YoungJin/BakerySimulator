using UnityEngine;

public class ClearTrash : MonoBehaviour
{
    public ParticleSystem cleanVFX;

    private void OnDisable()
    {
        cleanVFX.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            MissionManager.Instance.NotifyClearTable();
        }
    }
}
