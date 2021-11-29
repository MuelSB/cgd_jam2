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
    private float playerNoveInterpSpeed = 0.5f;
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
        // Check a player is controlled
        if (!controlledPlayer.is_some) return;

        // Raycast from the cursor world position down into the scene
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(
                Mouse.current.position.x.ReadValue(),
                Mouse.current.position.y.ReadValue(),
                0.0f));
        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction * selectRayLength, Color.red, 5.0f);

        if (Physics.Raycast(ray, out hit, selectRayLength, tileLayerMask))
        {
            // Get the player's current tile coordinate
            var playerHeight = controlledPlayer.value.GetComponent<CapsuleCollider>().height;
            RaycastHit playerTileRaycastHit;
            Ray playerTileRay = new Ray(controlledPlayer.value.transform.position, -Vector3.up);
            var playerTileCoord = new MapCoordinate(0, 0);
            if(Physics.Raycast(playerTileRay, out playerTileRaycastHit, (playerHeight / 2.0f) + 1.0f, tileLayerMask))
            {
                playerTileCoord = playerTileRaycastHit.collider.gameObject.GetComponent<MapTile>().getLocation();
                controlledPlayer.value.ChangeCurrentTile(playerTileCoord);
            }

            // Get the hit tile's map coordinate
            var hitCoordinate = hit.collider.gameObject.GetComponent<MapTile>().getLocation();

            // Do nothing if the selected tile is the tile the player is currently on
            if (hitCoordinate == playerTileCoord) return;

            // Get the distance between the hit tile and the controlled player's current tile
            int widthCount, depthCount;
            MapManager.GetTileCountTo(playerTileCoord, hitCoordinate, out widthCount, out depthCount);

            // If the tile is within the controlled player's movement range
            if (Mathf.Abs(widthCount) <= controlledPlayer.value.movementRange &&
                Mathf.Abs(depthCount) <= controlledPlayer.value.movementRange)
            {
                StartCoroutine(MovePlayerToTile(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator MovePlayerToTile(GameObject tileObject)
    {
        // Do nothing if the controller is not controlling a player
        if (!controlledPlayer.is_some) yield return null;

        float t = 0f;

        // Compute target position
        var playerCollisionHalfHeight = controlledPlayer.value.GetComponent<CapsuleCollider>().height * 0.5f;
        var tileCollisionHalfHeight = tileObject.GetComponent<BoxCollider>().size.y * 0.5f;

        var targetPosition = tileObject.transform.position;
        targetPosition.y += (playerCollisionHalfHeight * controlledPlayer.value.transform.localScale.y) + tileCollisionHalfHeight;

        // Move the player towards the target position
        while(Vector3.Distance(controlledPlayer.value.transform.position, targetPosition) > 0.1f)
        {
            controlledPlayer.value.transform.position = Vector3.Slerp(controlledPlayer.value.transform.position, targetPosition, t);
            t += playerNoveInterpSpeed * Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}
