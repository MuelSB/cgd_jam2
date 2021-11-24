using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Class variables
    PlayerInputActions playerInputActions;
    private Maybe<Player> ControlledPlayer = new Maybe<Player>();

    public void ControlPlayer(Player targetPlayer)
    {
        // Set the player that this controller is controlling
        ControlledPlayer = new Maybe<Player>(targetPlayer);

        // Call the player's on controlled event
        ControlledPlayer.value.OnControlled();
    }

    public void UnControlPlayer()
    {
        // If this controller is not controlling a player, just return
        if (!ControlledPlayer.is_some) return;

        // Call the controlled player's on uncontrolled event
        ControlledPlayer.value.OnUnControlled();
    }

    private void Awake()
    {
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Select.performed += Select;
    }

    private void Select(InputAction.CallbackContext context)
    {

    }

    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit, 1000))
    //        {
    //            Vector3 position = transform.position;



    //            position.x = Mathf.Floor(hit.point.x) + 0.5f;
    //            position.y = Mathf.Floor(hit.point.y) + 1.0f; // Change this to be based on height
    //            position.z = Mathf.Floor(hit.point.z) + 0.5f;

    //            transform.position = position;

    //            print(position);
    //        }

    //    }
    //}
}
