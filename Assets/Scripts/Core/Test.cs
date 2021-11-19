using Core;
using UnityEngine;

public class Test : MonoBehaviour
{

    private void OnEnable() => EventSystem.Subscribe("Brrr", Do );


    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            EventSystem.Invoke("Brrr");
        }
    }

    private void Do()
    {
        Debug.Log("BRRR");
    }

    private void DoMore()
    {
        Debug.Log("BRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRrrrrrrrrrrrrrrrrr");
    }
}
