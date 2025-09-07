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
            // 정규화
            float normalizedSpeed = agent.velocity.magnitude / agent.speed;

            // Clamp로 혹시 모를 1 초과 방지
            normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

            anim.SetFloat("Speed", normalizedSpeed);
        }
    }
}
