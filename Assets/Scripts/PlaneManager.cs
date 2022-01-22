using System.Timers;
using UnityEngine;
using Vuforia;
using UnityEngine.XR.ARFoundation;

public class PlaneManager : MonoBehaviour
{
    [SerializeField] PlaneFinderBehaviour planeFinder = null;

    [Header("Augmentation")]
    [SerializeField] GameObject placementAugmentation = null;

    StateManager stateManager;
    SmartTerrain smartTerrain;
    PositionalDeviceTracker positionalDeviceTracker;
    ContentPositioningBehaviour contentPositioningBehaviour;
    TouchHandler touchHandler;
    FurniturePlacement furniturePlacement;
    AnchorBehaviour placementAnchor;
    PlaneAreaManager planeAreaManager;
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

    Timer timer;
    bool timerFinished;

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        this.planeFinder.HitTestMode = HitTestMode.AUTOMATIC;

        this.furniturePlacement = FindObjectOfType<FurniturePlacement>();
        this.touchHandler = FindObjectOfType<TouchHandler>();
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

    void OnDestroy()
    {
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    public void LoadPlacementAugmentation(GameObject furniture)
    {
        this.placementAugmentation = furniture;
    }
    
    public void HandleInteractiveHitTest(HitTestResult result)
    {
        if (result == null)
        {
            return;
        }

        this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
        this.contentPositioningBehaviour.DuplicateStage = false;

        Vector3 furnitureSize = this.placementAugmentation.GetComponent<MeshCollider>().bounds.size;
        bool furnitureBiggerThanPlane = furnitureSize.x > planeAreaManager.planeWidth || furnitureSize.y > planeAreaManager.planeHeight;
        bool canPlaceFurniture = planeAreaManager.enableMeasurement ? !furnitureBiggerThanPlane : true;

        if (TrackingStatusIsTrackedAndNormal && canPlaceFurniture)
        {
            this.contentPositioningBehaviour.AnchorStage = this.placementAnchor;
            this.contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            UtilityHelper.EnableRendererColliderCanvas(placementAugmentation, true);
            
            if (!this.furniturePlacement.IsPlaced)
            {
                this.furniturePlacement.PlaceFurnitureAtAnchorFacingCamera(this.placementAnchor.transform);
                this.touchHandler.enableRotation = true;
            }
            else
            {
                this.furniturePlacement.PlaceFurnitureAtAnchor(this.placementAnchor.transform);
            }
        }
    }

    public void ResetScene()
    {
        this.furniturePlacement.Reset();
        UtilityHelper.EnableRendererColliderCanvas(this.placementAugmentation, false);

        this.furniturePlacement.DetachFurnitureFromAnchor();
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
