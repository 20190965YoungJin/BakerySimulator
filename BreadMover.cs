using System.Collections;
using UnityEngine;

public class BreadMover : MonoBehaviour
{
    // 로컬 이동 (로컬 좌표 기준)
    public static IEnumerator LocalMoveBreadCurve(GameObject bread, Vector3 start, Vector3 end, float duration, float height = 2f)
    {
        Vector3 control = (start + end) / 2 + Vector3.up * height;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;

            Vector3 curvedPos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            bread.transform.localPosition = curvedPos;

            time += Time.deltaTime;
            yield return null;
        }

        bread.transform.localPosition = end;
    }

    // 월드 이동 (월드 좌표 기준)
    public static IEnumerator WorldMoveBreadCurve(GameObject bread, Vector3 start, Vector3 end, float duration, float height = 2f)
    {
        Vector3 control = (start + end) / 2 + Vector3.up * height;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;

            Vector3 curvedPos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            bread.transform.position = curvedPos;

            time += Time.deltaTime;
            yield return null;
        }

        bread.transform.position = end;
    }
}
