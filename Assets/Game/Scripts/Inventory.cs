using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using Freya;
using System;

public class Inventory : MonoBehaviour
{
    const float TAU = Mathfs.TAU;

    [Range(0.01f, 1f)] public float arcRadius = 0.1f;
    [Range(0.0001f,0.1f)] public float[] iRadii = new float[3];

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        using( new Handles.DrawingScope(transform.localToWorldMatrix) )
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            int iCount = iRadii.Length;

            Vector3 forward = Vector3.forward;
            Vector3 up = Vector3.up;


            float[] angBetweenRad = new float[iCount-1];
            float totalAngBetweenRad = 0;
            for (int i = 0; i < angBetweenRad.Length; i++)
            {
                float a = iRadii[i];
                float b = iRadii[i+1];
                float abLenght = a + b;

                float ang = Mathf.Acos(1 - abLenght * abLenght / (2 * arcRadius * arcRadius));
                angBetweenRad[i] = ang;
                totalAngBetweenRad += angBetweenRad[i];
            }

            float angRad = - totalAngBetweenRad * 0.5f;
            for (int i = 0; i < iCount; i++)
            {
                float radius = iRadii[i];
                float t = i / (float)iCount;

                Vector3 pt = AngToDir(angRad) * arcRadius;
                Vector3 ptXZplane = new Vector3 ( pt.y, pt.x, 0);
                Quaternion ptOrientation = Quaternion.LookRotation(transform.forward, ptXZplane);

                angRad += i < iCount-1 ? angBetweenRad[i] : 0;

                Handles.DrawWireDisc(ptXZplane, forward, radius);
            }

            DrawArc(forward, up, totalAngBetweenRad* Mathf.Rad2Deg, arcRadius);
        }
    }

    private Vector3 AngToDir(float angRad) => new Vector3( Mathf.Cos(angRad), Mathf.Sin(angRad));

    void DrawArc(Vector3 n, Vector3 from, float to, float radius)
    {
        Handles.color = Color.cyan;
        Handles.DrawWireArc(default, n, from, to, radius);
        Handles.DrawWireArc(default, n, from, -to, radius);
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(Vector3.zero, from * arcRadius);
    }
#endif
}
