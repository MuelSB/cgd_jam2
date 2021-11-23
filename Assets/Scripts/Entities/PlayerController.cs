using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector3 position = transform.position;



                position.x = Mathf.Floor(hit.point.x) + 0.5f;
                position.y = Mathf.Floor(hit.point.y) + 1.0f; // Change this to be based on height
                position.z = Mathf.Floor(hit.point.z) + 0.5f;

                transform.position = position;

                print(position);
            }

        }
    }
}
