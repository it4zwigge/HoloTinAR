using System.Collections;
using UnityEditor;
using UnityEngine;

public class AutoCollider : MonoBehaviour
{
    [MenuItem("HoloTinAR/Tools/AutoSize Collider")]
    static void FitToChildren()
    {
        GameObject parentObject = Selection.gameObjects[0];

        BoxCollider[] colliders = parentObject.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
            DestroyImmediate(collider);

        BoxCollider bc = parentObject.AddComponent<BoxCollider>();

        if (parentObject.transform.childCount > 0)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool hasBounds = false;

            Renderer[] renderers = parentObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renderers)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(render.bounds);
                }
                else
                {
                    bounds = render.bounds;
                    hasBounds = true;
                }
            }
            if (hasBounds)
            {
                bc.center = bounds.center - parentObject.transform.position;
                bc.size = bounds.size;
            }
            else
            {
                bc.size = bc.center = Vector3.zero;
                bc.size = Vector3.zero;
            }
        }
    }
}
