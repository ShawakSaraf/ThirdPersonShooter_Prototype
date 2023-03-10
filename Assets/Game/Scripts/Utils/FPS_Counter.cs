using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Counter : MonoBehaviour
{
    [Range(0, 0.3f)][SerializeField] float m_UpdateRate = 0.2f;
    [SerializeField] Vector2 m_TextPos = new Vector2(5, 40);
    [SerializeField] Vector2 m_WidthHeight = new Vector2(200, 50);

    string label = "";
	float count;
	
	IEnumerator Start ()
	{
		GUI.depth = 2;
		while (true) {
			if (Time.timeScale == 1) {
				yield return new WaitForSeconds (m_UpdateRate);
				count = (1 / Time.deltaTime);
				label = (Mathf.Round (count)).ToString();
			} else {
				label = "Pause";
			}
			yield return new WaitForSeconds (0.5f);
		}
	}
	
	void OnGUI ()
	{
		GUI.Label (new Rect (m_TextPos.x, m_TextPos.y, m_WidthHeight.x, m_WidthHeight.y), label);
	}
}
