using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMarkerManager : MonoBehaviour
{
    [SerializeField] GameObject MarkerPrefab;
    [SerializeField] Transform MarkerRoot;

    public static HUDMarkerManager Instance { get; private set; } = null;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found a duplicate HUDMarkerManager on " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMarker(HUDMarkerInWorldTarget target, string text, Sprite image)
    {
        var newMarker = Instantiate(MarkerPrefab, Vector3.zero, Quaternion.identity, MarkerRoot);

        newMarker.GetComponent<HUDMarkerTargetUI>().Bind(target, text, image);
    }
}
