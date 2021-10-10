using System.Timers;
using UnityEngine;
using Vuforia;
using UnityEngine.XR.ARFoundation;

public class PlaneManager : MonoBehaviour
{
    public static bool GroundPlaneHitReceived { get; private set; }
    [SerializeField] PlaneFinderBehaviour planeFinder = null;

    [Header("Augmentation")]
    [SerializeField] GameObject placementAugmentation = null;

    StateManager stateManager;
    SmartTerrain smartTerrain;
    PositionalDeviceTracker positionalDeviceTracker;
    ContentPositioningBehaviour contentPositioningBehaviour;
    TouchHandler touchHandler;
    ProductPlacement productPlacement;
    AnchorBehaviour placementAnchor;
    ARPlaneManager planeManager;
    PlaneAreaManager planeAreaManager;
    int automaticHitTestFrameCount;
    static TrackableBehaviour.Status StatusCached = TrackableBehaviour.Status.NO_POSE;
    static TrackableBehaviour.StatusInfo StatusInfoCached = TrackableBehaviour.StatusInfo.UNKNOWN;

    public static bool TrackingStatusIsTrackedAndNormal
    {
        get
        {
            return
                (StatusCached == TrackableBehaviour.Status.TRACKED ||
                 StatusCached == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
                StatusInfoCached == TrackableBehaviour.StatusInfo.NORMAL;
        }
    }

    public static bool TrackingStatusIsTrackedOrLimited
    {
        get
        {
            return TrackingStatusIsTrackedAndNormal ||
                (StatusCached == TrackableBehaviour.Status.LIMITED &&
                 StatusInfoCached == TrackableBehaviour.StatusInfo.UNKNOWN);
        }
    }

    bool SurfaceIndicatorVisibilityConditionsMet
    {
        get
        {
            return TrackingStatusIsTrackedOrLimited && Input.touchCount == 0; // && GroundPlaneHitReceived
        }
    }

    Timer timer;
    bool timerFinished;

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        this.planeFinder.HitTestMode = HitTestMode.AUTOMATIC;

        this.productPlacement = FindObjectOfType<ProductPlacement>();
        this.touchHandler = FindObjectOfType<TouchHandler>();
        this.planeManager = FindObjectOfType<ARPlaneManager>();
        this.planeAreaManager = GetComponent<PlaneAreaManager>();

        this.placementAnchor = this.placementAugmentation.GetComponentInParent<AnchorBehaviour>();

        UtilityHelper.EnableRendererColliderCanvas(this.placementAugmentation, false);

        this.timer = new Timer(10000);
        this.timer.Elapsed += TimerFinished;
        this.timer.AutoReset = false;
    }

    void Update()
    {
        if (this.timerFinished)
        {
            ResetTrackers();
            ResetScene();
            this.timerFinished = false;
        }
    }

    void LateUpdate()
    {
        // GroundPlaneHitReceived = (this.automaticHitTestFrameCount == Time.frameCount);

        // SetSurfaceIndicatorVisible(SurfaceIndicatorVisibilityConditionsMet);
    }

    void OnDestroy()
    {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    // public void HandleAutomaticHitTest(HitTestResult result)
    // {
    //     this.automaticHitTestFrameCount = Time.frameCount;

    //     if (!productPlacement.IsPlaced)
    //     {
    //         this.productPlacement.DetachProductFromAnchor();
    //         this.placementAugmentation.PositionAt(result.Position);
    //     }
    // }

    public void LoadPlacementAugmentation(GameObject product)
    {
        this.placementAugmentation = product;
    }
    
    public void HandleInteractiveHitTest(HitTestResult result)
    {
        if (result == null)
        {
            return;
        }

        this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
        this.contentPositioningBehaviour.DuplicateStage = false;

        Vector3 productSize = this.placementAugmentation.GetComponent<MeshCollider>().bounds.size;
        bool productBiggerThanPlane = productSize.x > planeAreaManager.planeWidth || productSize.y > planeAreaManager.planeHeight;

        if (TrackingStatusIsTrackedAndNormal && !productBiggerThanPlane)
        {
            this.contentPositioningBehaviour.AnchorStage = this.placementAnchor;
            this.contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            UtilityHelper.EnableRendererColliderCanvas(placementAugmentation, true);
            
            if (!this.productPlacement.IsPlaced)
            {
                this.productPlacement.PlaceProductAtAnchorFacingCamera(this.placementAnchor.transform);
                this.touchHandler.enableRotation = true;
            }
            else
            {
                this.productPlacement.PlaceProductAtAnchor(this.placementAnchor.transform);
            }
        }
    }

    public void ResetScene()
    {
        this.productPlacement.Reset();
        UtilityHelper.EnableRendererColliderCanvas(this.placementAugmentation, false);

        this.productPlacement.DetachProductFromAnchor();
        this.touchHandler.enableRotation = false;
    }

    public void ResetTrackers()
    {
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        this.smartTerrain.Stop();
        this.positionalDeviceTracker.Reset();
        this.smartTerrain.Start();
    }

    void SetSurfaceIndicatorVisible(bool isVisible)
    {
        Renderer[] renderers = this.planeFinder.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
        Canvas[] canvas = this.planeFinder.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

        Transform transform = this.planeFinder.PlaneIndicator.GetComponent<Transform>();
        Debug.Log("Point x coordinate: " + transform.position.x);

        foreach (Canvas c in canvas)
            c.enabled = isVisible;

        foreach (Renderer renderer in renderers)
            renderer.enabled = isVisible;
    }

    void TimerFinished(System.Object source, ElapsedEventArgs e)
    {
        this.timerFinished = true;
    }

    void OnVuforiaStarted()
    {
        stateManager = TrackerManager.Instance.GetStateManager();

        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
            {
                return;
            }

            if (this.positionalDeviceTracker.IsActive && !this.smartTerrain.IsActive)
                this.smartTerrain.Start();
        }
    }

    void OnTrackerStarted()
    {
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
            {
                return;
            }

            if (!this.smartTerrain.IsActive)
                this.smartTerrain.Start();
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        StatusCached = status;
        StatusInfoCached = statusInfo;
    }
}
