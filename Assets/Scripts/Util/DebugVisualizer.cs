using UnityEngine;

public class DebugVisualizer
{

    public static void DrawBoxCast(Vector3 start, Vector3 end, Vector3 size, Quaternion rotation, float maxDistance)
    {
        Gizmos.color = Color.green;

        // Cache the Gizmos matrix.
        Matrix4x4 currentMatrix = Gizmos.matrix;

        // Draw Cubes
        Gizmos.matrix = Matrix4x4.TRS(start, rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.TRS(end, rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Draw Connecting Lines
        Vector3 x = Vector3.right * size.x * 0.5f;
        Vector3 y = Vector3.up * size.y * 0.5f;
        Vector3 z = Vector3.forward * size.z * 0.5f;

        Gizmos.matrix = Matrix4x4.TRS(start, rotation, Vector3.one);
        Vector3 rayDirection = (end - start).normalized;
        Vector3 localDir = Quaternion.Inverse(rotation) * rayDirection;

        Gizmos.DrawRay(Vector3.zero - x - y - z, localDir * maxDistance);
        Gizmos.DrawRay(Vector3.zero + x - y - z, localDir * maxDistance);
        Gizmos.DrawRay(Vector3.zero + x + y - z, localDir * maxDistance);
        Gizmos.DrawRay(Vector3.zero - x + y - z, localDir * maxDistance);

        // Reset the Gizmos matrix.
        Gizmos.matrix = currentMatrix;
    }

    public static void VisualizeHitPoint(MonoBehaviour monoBehaviour, Vector3 hitPoint, float sphereVisualizeScale, float duration = -1f)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.GetComponent<Collider>().enabled = false;
        obj.transform.localScale = new Vector3(sphereVisualizeScale, sphereVisualizeScale, sphereVisualizeScale);
        obj.transform.position = hitPoint;

        if (duration > 0)
        {
            Util.WaitForSeconds(monoBehaviour, () => Object.Destroy(obj), duration);
        }
    }

    /// <summary>
    /// Never tested before. Leaving this here for future use case
    /// </summary>
    /// <param name="pos0"></param>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="time"></param>
    /// <param name="resolution"></param>
    public static void VisualizeBezier(Vector3 pos0,Vector3 pos1, Vector3 pos2, float time, int resolution = 20)
    {
        Gizmos.color = Color.yellow;

        Vector3 prevPoint = pos0;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = GetPoint(t);

            Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetPoint(time), 0.1f);

        Vector3 GetPoint(float t)
        {
            float u = 1 - t;
            return u * u * pos0+
                   2 * u * t * pos1 +
                   t * t * pos2;
        }
    }
}