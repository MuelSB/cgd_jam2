using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    public Map map;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MapCoordinate mapCoordinate = new MapCoordinate((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            map.IsValidCoordinate(mapCoordinate);
            player.Move(map.GetTile(mapCoordinate).getLocation());
            print("map coord:" + Input.mousePosition.x + Input.mousePosition.y);
        }
    }
}
