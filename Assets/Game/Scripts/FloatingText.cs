using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] Vector3 randomePoz = new Vector3(0, 0, 0);
    [SerializeField] float txtPopUp = 3f;
    [SerializeField] Vector3 txtOffset = new Vector3(0, 2, 0);

    void Start()
    {
        RandomizePos();
        RandomizeColor();

        // offset text
        transform.localPosition += txtOffset;

        // look at camera
        transform.LookAt(Camera.main.transform);
        Destroy(gameObject, txtPopUp);
    }

    private void RandomizePos()
    {
        transform.localPosition += new Vector3(Random.Range(-randomePoz.x, randomePoz.x),
        Random.Range(-randomePoz.y, randomePoz.y),
        Random.Range(-randomePoz.z, randomePoz.z));
    }

    private void RandomizeColor()
    {
        var popUpTxt = GetComponent<TextMeshPro>();
        popUpTxt.color = Random.ColorHSV(0.5f, 1f, 0.7f, 1f, 1f, 1f);
    }

    void Update()
    {
        
    }
}
