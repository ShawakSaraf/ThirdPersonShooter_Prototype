using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace RPG.Control
{
    public class EnemyPatrolPath : MonoBehaviour
    {
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            for (int i = 0; i < GetChildCount(); i++)
            {
                int j = GetNextIndex(i);
                Handles.color = Color.grey;
                Handles.DrawAAPolyLine(0.5f, GetWaypoint(i), GetWaypoint(j));
                Handles.color = new Vector4(0.5f,0.5f,0.5f,0.3f);
                Handles.DrawAAPolyLine(0.5f, GetWaypoint(i), (-Vector3.up * GetWaypoint(i).y) + GetWaypoint(i) );
            }
        }
    #endif

        public int GetChildCount() => transform.childCount;

        public int GetNextIndex(int i) => (i + 1) % transform.childCount;
        
        public Vector3 GetWaypoint(int i) => transform.GetChild(i).position;

    }
}