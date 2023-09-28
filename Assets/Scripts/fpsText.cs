using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fpsText : MonoBehaviour
{
    [SerializeField] Text text;

    // Update is called once per frame
    void Update()
    {
        text.text = (1.0f / Time.deltaTime).ToString();
    }
}
