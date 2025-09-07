using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperbagDisabled : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
