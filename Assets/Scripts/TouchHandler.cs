using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    public Transform augmentationObject;

    [HideInInspector]
    public bool enableRotation;
    public bool enablePinchScaling;

    public static bool IsSingleFingerStationary => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Stationary);

    public static bool IsSingleFingerDragging => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Moved);

    const float ScaleRangeMin = 0.1f;
    const float ScaleRangeMax = 2.0f;

    Touch[] touches;
    static int lastTouchCount;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;

    void Start()
    {
        this.cachedAugmentationScale = this.augmentationObject.localScale.x;
        this.cachedAugmentationRotation = this.augmentationObject.localEulerAngles;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            var firstTouch = Input.GetTouch(0);
            var secondTouch = Input.GetTouch(1);
            
            float currentTouchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            float diff_y = firstTouch.position.y - secondTouch.position.y;
            float diff_x = firstTouch.position.x - secondTouch.position.x;
            float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

            if (this.isFirstFrameWithTwoTouches)
            {
                this.cachedTouchDistance = currentTouchDistance;
                this.cachedTouchAngle = currentTouchAngle;
                this.isFirstFrameWithTwoTouches = false;
            }

            float angleDelta = currentTouchAngle - this.cachedTouchAngle;
            float scaleMultiplier = (currentTouchDistance / this.cachedTouchDistance);
            float scaleAmount = this.cachedAugmentationScale * scaleMultiplier;
            float scaleAmountClamped = Mathf.Clamp(scaleAmount, ScaleRangeMin, ScaleRangeMax);

            if (this.enableRotation)
            {
                this.augmentationObject.localEulerAngles = this.cachedAugmentationRotation - new Vector3(0, angleDelta * 3f, 0);
            }
            if (this.enableRotation && this.enablePinchScaling)
            {
                this.augmentationObject.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
            }

        }
        else if (Input.touchCount < 2)
        {
            this.cachedAugmentationScale = this.augmentationObject.localScale.x;
            this.cachedAugmentationRotation = this.augmentationObject.localEulerAngles;
            this.isFirstFrameWithTwoTouches = true;
        }
        else if (Input.touchCount == 6)
        {
            this.enablePinchScaling = true;
        }
        else if (Input.touchCount == 5)
        {
            this.enablePinchScaling = false;
        }
    }

    static bool IsSingleFingerDown()
    {
        if (Input.touchCount == 0 || Input.touchCount >= 2)
            lastTouchCount = Input.touchCount;

        return (
            Input.touchCount == 1 &&
            Input.GetTouch(0).fingerId == 0 &&
            lastTouchCount == 0);
    }

    public void LoadAugmentationObject(GameObject furniture)
    {
        this.augmentationObject = furniture.transform;
    }
}
