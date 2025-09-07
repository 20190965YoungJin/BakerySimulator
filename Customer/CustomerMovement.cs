using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(agent != null)
        {
            // ����ȭ
            float normalizedSpeed = agent.velocity.magnitude / agent.speed;

            // Clamp�� Ȥ�� �� 1 �ʰ� ����
            normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

            anim.SetFloat("Speed", normalizedSpeed);
        }
    }
}
