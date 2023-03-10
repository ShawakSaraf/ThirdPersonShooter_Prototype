using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehavior;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class EnemyManager : MonoBehaviour
{
    public static List<EnemyAI> allEnemies = new List<EnemyAI>();

    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            foreach (var enemy in allEnemies)
            {
                Vector3 managePos   = transform.position;
                Vector3 enemyPos    = enemy.transform.position;
                float halfHeaight   = (managePos.y - enemyPos.y) * 0.5f;
                Vector3 offset      = Vector3.up * halfHeaight;
                Handles.DrawBezier(
                    managePos,
                    enemyPos,
                    managePos - offset *1.5f,
                    enemyPos + offset,
                    Color.grey,
                    EditorGUIUtility.whiteTexture,
                    1
                );
                
            }
        }
    #endif
}
