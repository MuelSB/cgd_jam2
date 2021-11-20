using Core;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            EventSystem.Invoke("StartFade");
        }
    }
}
