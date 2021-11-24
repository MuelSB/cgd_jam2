using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Class variables
    PlayerInputActions playerInputActions;
    private Maybe<Player> controlledPlayer = new Maybe<Player>();
    private float selectRayLength = 10000.0f;
    private float playerMovementSmoothing = 0.01f;
    private int tileLayerMask = 1 << 6;

    public void ControlPlayer(Player targetPlayer)
    {
        // Set the player that this controller is controlling
        controlledPlayer = new Maybe<Player>(targetPlayer);

        // Call the player's on controlled event
        controlledPlayer.value.OnControlled();
    }

    public void UnControlPlayer()
    {
        // If this controller is not controlling a player, just return
        if (!controlledPlayer.is_some) return;

        // Call the controlled player's on uncontrolled event
        controlledPlayer.value.OnUnControlled();
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
        // Raycast from the cursor world position down into the scene
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(
                Mouse.current.position.x.ReadValue(),
                Mouse.current.position.y.ReadValue(),
                0.0f));
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction * selectRayLength, Color.red, 5.0f);

        if (Physics.Raycast(ray, out hit, selectRayLength, tileLayerMask))
        {
            StartCoroutine(MovePlayerToTile(hit.collider.gameObject));
        }
    }

    private IEnumerator MovePlayerToTile(GameObject tileObject)
    {
        // Do nothing if the controller is not controlling a player
        if (!controlledPlayer.is_some) yield return null;

        // Compute target position
        var playerCollisionHalfHeight = controlledPlayer.value.GetComponent<CapsuleCollider>().height * 0.5f;
        var tileCollisionHalfHeight = tileObject.GetComponent<BoxCollider>().size.y * 0.5f;

        var targetPosition = tileObject.transform.position;
        targetPosition.y += playerCollisionHalfHeight + tileCollisionHalfHeight;

        // Move the player towards the target position
        while(Vector3.Distance(controlledPlayer.value.transform.position, targetPosition) > 0.005f)
        {
            controlledPlayer.value.transform.position = Vector3.Lerp(controlledPlayer.value.transform.position, targetPosition, playerMovementSmoothing * Time.time);

            yield return null;
        }

        yield return null;
    }
}
