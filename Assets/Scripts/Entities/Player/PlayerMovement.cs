using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed = 0.5f;

    public bool IsMoving => _movingCoroutine != null;

    private Coroutine _movingCoroutine = null;
    public UnityEvent _moveComplete = new UnityEvent();
    
    public void MovePlayer(Player player, GameObject tileObject)
    {
        _movingCoroutine = StartCoroutine(MovePlayerToTile(player, tileObject));
    }
    
    private IEnumerator MovePlayerToTile(Player player, GameObject tileObject)
    {
        // Compute target position
        var t = 0f;
        var playerCollisionHalfHeight = player.GetComponent<CapsuleCollider>().height * 0.5f;
        var tileCollisionHalfHeight = tileObject.GetComponent<BoxCollider>().size.y * 0.5f;

        var playerTransform = player.transform;
        var targetPosition = tileObject.transform.position;
        targetPosition.y += (playerCollisionHalfHeight * playerTransform.localScale.y) + tileCollisionHalfHeight;

        // Move the player towards the target position
        while(Vector3.Distance(playerTransform.position, targetPosition) > 0.1f)
        {
            player.transform.position = Vector3.Slerp(playerTransform.position, targetPosition, t);
            t += playerMoveSpeed * Time.deltaTime;

            yield return null;
        }

        // Tile reached
        // Set the tile's entity to player
        var mapTile = tileObject.GetComponent<MapTile>();
        player.ChangeCurrentTile(mapTile.getLocation());
        
        // set coroutine to null
        _movingCoroutine = null;
        _moveComplete.Invoke();
        yield return null;
    }
}
