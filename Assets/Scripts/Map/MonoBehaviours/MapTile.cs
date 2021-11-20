using UnityEngine;

public class MapTile : MonoBehaviour
{
    // Class properties
    private MapTileProperties properties = new MapTileProperties();

    // Component references
    private Rigidbody tileRigidbody;
    private Renderer objectRenderer;

    public void SetProperties(MapTileProperties properties) 
    {
        this.properties = properties;
        // TODO: Should integrity be tied to tile transparency? Update fade here if so with SetFade()
    }

    public MapTileProperties GetProperties() { return properties; }

    public GameObject GetObject() { return gameObject; }

    public void SetFade(float fade)
    {
        var newColor = objectRenderer.material.color;
        newColor.a = 1.0f - fade;
        objectRenderer.material.color = newColor;
    }

    public void UseGravity(bool use)
    {
        tileRigidbody.useGravity = use;
        tileRigidbody.isKinematic = !use;
    }

    private void Awake()
    {
        tileRigidbody = GetComponent<Rigidbody>();
        objectRenderer = GetComponent<Renderer>();
    }
}
