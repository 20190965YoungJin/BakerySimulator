using System.Collections.Generic;
using UnityEngine;

public class BreadBox : MonoBehaviour
{
    private Queue<GameObject> objectQueue = new Queue<GameObject>();
    public Transform boxTransform;
    public int maxCapacity = 20;
    public Collider boxCollider;

    private void Start()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<Collider>();

        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if (!objectQueue.Contains(obj) && objectQueue.Count < maxCapacity && obj.CompareTag("Bread"))
        {
            objectQueue.Enqueue(obj);
            obj.transform.SetParent(boxTransform);
            //Debug.Log($"{obj.name} added to box.");
        }
    }

    // PopBread이 제거된 오브젝트를 반환
    public GameObject PopBread()
    {
        if (objectQueue.Count > 0)
        {
            GameObject obj = objectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.GetComponent<MeshCollider>().enabled = false;
            return obj;
        }

        return null;
    }

    public int BreadCount()
    {
        return objectQueue.Count;
    }
}
