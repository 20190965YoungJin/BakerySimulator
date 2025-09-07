using UnityEngine;
using System.Collections;

public class Oven : MonoBehaviour
{
    public BreadBox breadBox;

    public GameObject objectPrefab; // ������ ������Ʈ
    public Transform spawnPoint; // ���� ��ġ
    public float pushForce = 135f; // �о ��
    public float spawnDelay = 1.2f; // ���� ����
    public int maxCreateBread = 10; // �� �ִ� ���� ��
    public ParticleSystem smoke;

    void Start()
    {
        // �ݺ� ����
        StartCoroutine(SpawnLoop());

        if(breadBox == null)
            breadBox = FindObjectOfType<BreadBox>();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (breadBox != null && breadBox.BreadCount() >= maxCreateBread)
            {
                yield return new WaitForSeconds(spawnDelay);
                continue;
            }

            // ������Ʈ ����
            GameObject obj = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
            smoke.Play();

            yield return new WaitForSeconds(0.6f);

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            //Freeze ����
            rb.constraints = RigidbodyConstraints.None;

            if (rb != null)
            {
                rb.AddForce(spawnPoint.forward * pushForce);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
