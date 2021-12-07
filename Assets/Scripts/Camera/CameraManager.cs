using UnityEngine;
using Core;

static public class CameraManager
{
    // Class variables
    static private Maybe<Transform> lookTargetTransform = new Maybe<Transform>();
    static private bool lookingAtPlayer = true;
    static private CameraManagerSettings settings = new CameraManagerSettings();

    static public void Initialize(CameraManagerSettings managerSettings)
    {
        // Store the camera manager settings
        settings = managerSettings;

        // Set the main camera's rotation
        Camera.main.transform.rotation = Quaternion.Euler(settings.CameraEulerRotation);

        // Subscribe to turn started events
        EventSystem.Subscribe<Entity>(Events.TurnStarted, OnTurnStarted);
    }

    static public void UpdateMainCamera()
    {
        // Move the main camera's position to look at the target transform
        if (lookTargetTransform.is_some && lookTargetTransform.value != null && Camera.main.transform != null)
        {
            var newCameraPosition = Vector3.MoveTowards(
                    Camera.main.transform.position,
                    new Vector3(
                            lookTargetTransform.value.position.x,
                            lookTargetTransform.value.position.y,
                            lookTargetTransform.value.position.z + (lookingAtPlayer ? settings.cameraZOffset : settings.enemyTurnZOffset)
                        ),
                    Time.deltaTime * settings.cameraLerpSpeed
                );

            newCameraPosition.y = (lookingAtPlayer ? settings.cameraYPosition : settings.enemyTurnYPosition);

            Camera.main.transform.position = newCameraPosition;
        }
    }

    static public void SetMainCameraPosition(Vector3 newPosition)
    {
        newPosition.y = settings.cameraYPosition;
        newPosition.z += settings.cameraZOffset;
        Camera.main.transform.position = newPosition;
    }

    static private void OnTurnStarted(Entity entity)
    {
        // need a null check here
        if (entity == null) return;
        
        // Call the player's turn started event if the turn's entity is the player
        if (entity.entityType == Entity.EntityType.PLAYER)
        {
            OnPlayerTurnStarted(entity as Player);
        }
        else
        {
            lookingAtPlayer = false;

            // Set the entity's transform as the main camera's target transform
            lookTargetTransform = new Maybe<Transform>(entity.transform);
        }
    }

    static private void OnPlayerTurnStarted(Player player)
    {
        lookingAtPlayer = true;

        // Set the main camera's target transform as the player's transform
        lookTargetTransform = new Maybe<Transform>(player.transform);
    }
}
