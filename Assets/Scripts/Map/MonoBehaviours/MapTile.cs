using UnityEngine;

public class MapTile : MonoBehaviour
{
    // Class properties
    [SerializeField] private MapTileProperties properties = new MapTileProperties();

    // Component references
    private Rigidbody tileRigidbody;
    private Renderer objectRenderer;

    public GameObject SetProperties(MapTileProperties properties) 
    {
        this.properties = properties;
        return gameObject; 
        // TODO: Should integrity be tied to tile transparency? Update fade here if so with SetFade()
    }

    public MapTileProperties GetProperties() { return properties; }

    public GameObject GetObject() { return gameObject; }

    //currently used for debug colouring based on type :)
    public void setColour(Color _new_colour) {
        objectRenderer.material.color = new Color(_new_colour.r, _new_colour.g, _new_colour.b, objectRenderer.material.color.a);
    }

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
