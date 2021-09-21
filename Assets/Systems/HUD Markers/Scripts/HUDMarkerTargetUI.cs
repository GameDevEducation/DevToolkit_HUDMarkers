using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDMarkerTargetUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LinkedText;
    [SerializeField] Image LinkedImage;

    HUDMarkerInWorldTarget LinkedTarget;
    string MarkerText;

    RectTransform HUDMarkerRect;

    public void Bind(HUDMarkerInWorldTarget _Target, string _Text, Sprite _Image)
    {
        LinkedTarget = _Target;
        LinkedText.text = MarkerText = _Text;
        LinkedImage.sprite = _Image;
    }

    // Start is called before the first frame update
    void Start()
    {
        HUDMarkerRect = (RectTransform) transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // if our target has been removed then remove the marker
        if (LinkedTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        var viewportPos = Camera.main.WorldToViewportPoint(LinkedTarget.transform.position);

        var distance = Mathf.FloorToInt(Vector3.Distance(LinkedTarget.transform.position, Camera.main.transform.position));
        LinkedText.text = MarkerText + System.Environment.NewLine + "(" + distance + "m)";                        

        // is the target on screen
        if (viewportPos.x >= 0f && viewportPos.x <= 1 &&
            viewportPos.y >= 0f && viewportPos.y <= 1 &&
            viewportPos.z > 0)
        {
            LinkedText.gameObject.SetActive(true);

            OnScreenRepositionMarker();
        }
        else
        {
            LinkedText.gameObject.SetActive(false);

            OffScreenRepositionMarker();
        }
    }

    void OnScreenRepositionMarker()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(LinkedTarget.transform.position);

        float markerX = ((screenPosition.x / Screen.width) - 0.5f) * HUDMarkerRect.rect.width;
        float markerY = ((screenPosition.y / Screen.height) - 0.5f) * HUDMarkerRect.rect.height;

        ((RectTransform)transform).anchoredPosition = new Vector2(markerX, markerY);
    }

    float CachedFOV_Horizontal = -1f;
    float CachedFOV_Vertical = -1;
    float CachedAspectRatio = -1f;

    void OffScreenRepositionMarker()
    {
        Vector3 vecToTarget = (LinkedTarget.transform.position - Camera.main.transform.position).normalized;

        // refresh cached data if changed
        if (Camera.main.aspect != CachedAspectRatio || Camera.main.fieldOfView != CachedFOV_Vertical)
        {
            CachedFOV_Vertical = Camera.main.fieldOfView;
            CachedAspectRatio = Camera.main.aspect;

            CachedFOV_Horizontal = Camera.VerticalToHorizontalFieldOfView(CachedFOV_Vertical, CachedAspectRatio);
        }

        // calculate normalised angles to target
        float normalisedX = Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.right)) * Mathf.Rad2Deg / (CachedFOV_Horizontal * 0.5f);
        float normalisedY = Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.up)) * Mathf.Rad2Deg / (CachedFOV_Vertical * 0.5f);

        // clamp to a 0 to 1 range
        normalisedX = Mathf.Clamp01(0.5f * (normalisedX + 1f));
        normalisedY = Mathf.Clamp01(0.5f * (normalisedY + 1f));

        // find the closest edge of the screen
        Vector2 distanceToTopRight = new Vector2(1f - normalisedX, 1f - normalisedY);
        Vector2 distanceToBottomLeft = new Vector2(normalisedX, normalisedY);
        float smallestX = Mathf.Min(distanceToBottomLeft.x, distanceToTopRight.x);
        float smallestY = Mathf.Min(distanceToBottomLeft.y, distanceToTopRight.y);
        float smallestDistance = Mathf.Min(smallestX, smallestY);

        // clamp to edge of screen
        if (smallestDistance == distanceToTopRight.x)
            normalisedX = 1f;
        else if (smallestDistance == distanceToBottomLeft.x)
            normalisedX = 0f;
        else if (smallestDistance == distanceToTopRight.y)
            normalisedY = 1f;
        else if (smallestDistance == distanceToBottomLeft.y)
            normalisedY = 0f;

        // position the marker
        ((RectTransform)transform).anchoredPosition = new Vector2((normalisedX - 0.5f) * HUDMarkerRect.rect.width, 
                                                                  (normalisedY - 0.5f) * HUDMarkerRect.rect.height);
    }
}
