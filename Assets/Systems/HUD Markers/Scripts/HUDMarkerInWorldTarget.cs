using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDMarkerInWorldTarget : MonoBehaviour
{
    [SerializeField] string Name;
    [SerializeField] Sprite HUDSprite;

    // Start is called before the first frame update
    void Start()
    {
        HUDMarkerManager.Instance.AddMarker(this, Name, HUDSprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
