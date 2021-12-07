using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapTile : MonoBehaviour
{
    // Class properties
    [SerializeField] private MapTileProperties properties = new MapTileProperties();

    // Component references
    private Rigidbody tileRigidbody;
    private Renderer objectRenderer;

    private Maybe<GameObject> decorChild = new Maybe<GameObject>();
    private List<Renderer> decorChildRenderers = new List<Renderer>();

    [SerializeField] private MapCoordinate mapLocation;

    public MapTile()
    {
        properties.tile = this;
    }

    public GameObject setLocation(MapCoordinate _coord) {
        mapLocation = _coord;
        return gameObject;
    }

    public MapCoordinate getLocation() => mapLocation;

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

    IEnumerator updatePostionFromInegrity()
    {
        float target_y = GetProperties().getHeightFromIntegrity();
        float current_y = gameObject.transform.position.y;
        if (current_y == target_y) {yield break;}
        float e_time = 0f;
        float duration = 3f;
        while (e_time < duration)
        {
            float new_y = Mathf.Lerp(current_y,target_y,(e_time/duration));
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,new_y,gameObject.transform.position.z);
            e_time+=Time.deltaTime;
            yield return null;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,target_y,gameObject.transform.position.z);
        yield return null;
    }

    public IEnumerator TileDeath()
    {
        Color grey = MetaGeneratorHelper.makeColour(40,40,40);
        Color currentColor = objectRenderer.material.color;
        float e_time = 0f;
        float duration = 1f;
        while (e_time < duration)
        {
            Color new_c = Color.Lerp(currentColor,grey,(e_time/duration));
            objectRenderer.material.color = new_c;
            if (decorChildRenderers.Count > 0)
            {
                decorChildRenderers.ForEach(_r => {_r.materials.ToList().ForEach(_m => {_m.color = new_c;});}); 
            }
            e_time+=Time.deltaTime;
            yield return null;
        }
        objectRenderer.material.color = grey;
        decorChildRenderers.ForEach(_r => {_r.material.color = grey;});
        UseGravity(true);
        yield return new WaitForSeconds(10);
        // objectRenderer.material.color = new Color(0,0,0,0);
        gameObject.SetActive(false);
        yield return null;
    }

    //will set the tiles integrity to zero and kill it.
    public void Kill() {
        if (GetProperties().Alive()) {
            MapTileProperties properties = GetProperties();
            properties.Integrity = 0;
            SetProperties(properties);
            StartCoroutine(TileDeath());
        }
    }

    //will erode the tiles integrity rate by its internal integrity erosion rate and return true if it dies.
    public bool Decay(System.Random _random) {
        if (GetProperties().Alive()) {
            MapTileProperties properties = GetProperties();

            properties.Integrity -= _random.Next(properties.IntegrityErosionRange.y,properties.IntegrityErosionRange.x+1);


            float current_height = gameObject.transform.position.y;
            float lerp_value = Mathf.Lerp(0.0f, current_height, 0.5f);
            float bias = 0.6f;

            // remaps the original lerp value between 0 and 1 with a bias.
            lerp_value = lerp_value / 10.0f + bias;

            // Make a new color
            Color grey = MetaGeneratorHelper.makeColour(40, 40, 40);
            Color currentColor = objectRenderer.material.color;

            // Set the color
            Color new_c = Color.Lerp(grey, currentColor, lerp_value);
            objectRenderer.material.color = new_c;

            //if (decorChildRenderers.Count > 0)
            //{
            //    decorChildRenderers.ForEach(_r => { _r.materials.ToList().ForEach(_m => { _m.color = new_c; }); });
            //}

            SetProperties(properties);
            if (properties.Integrity <= 0) {
                StartCoroutine(TileDeath());
                return true;
            }
            StartCoroutine(updatePostionFromInegrity());
            return false;
        }
        return false;
    }

    public void removeDecor() {
        if (decorChild.is_some) {
            Destroy(decorChild.value);
        }
    }
    public void spawnDecor(GameObject _decor) {
        removeDecor();
        decorChild = new Maybe<GameObject>(Instantiate(_decor,gameObject.transform));
        decorChildRenderers = decorChild.value.GetComponentsInChildren<Renderer>().ToList();
    }
}