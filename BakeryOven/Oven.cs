using UnityEngine;
using System.Collections;

public class Oven : MonoBehaviour
{
    public BreadBox breadBox;

    public GameObject objectPrefab; // 생성할 오브젝트
    public Transform spawnPoint; // 생성 위치
    public float pushForce = 135f; // 밀어낼 힘
    public float spawnDelay = 1.2f; // 생성 간격
    public int maxCreateBread = 10; // 빵 최대 생성 수
    public ParticleSystem smoke;

    void Start()
    {
        // 반복 시작
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

            // 오브젝트 생성
            GameObject obj = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
            smoke.Play();

            yield return new WaitForSeconds(0.6f);

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            //Freeze 해제
            rb.constraints = RigidbodyConstraints.None;

            if (rb != null)
            {
                rb.AddForce(spawnPoint.forward * pushForce);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
