using UnityEngine;


[RequireComponent(typeof(Canvas))]
public class MissionNavi : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer cursorSprite;

    private void Start()
    {
        if (target == null)
            target = GameObject.Find("MissionPointerMesh").transform;
        if ( cursorSprite == null )
            cursorSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (!target.gameObject.activeSelf)
        {
            cursorSprite.enabled = false;
            return;
        }
        cursorSprite.enabled = true;

        Vector3 direction = target.position - transform.position;
        direction.y = 0; // ���� ���⸸ ȸ��

        if (direction.sqrMagnitude < 0.02f)
            return;

        // ȸ�� ����
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }
}
