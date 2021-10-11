using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using Vuforia;

public class InstructionsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text instructions = null;
    [SerializeField] CanvasGroup screenReticle = null;
    [SerializeField] GameObject furniture = null;
    GraphicRaycaster graphicRayCaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    FurniturePlacement furniturePlacement;
    TouchHandler touchHandler;
    PlaneAreaManager planeAreaManager;
    ARPlaneManager planeManager;

    void Start()
    {
        
        this.furniturePlacement = FindObjectOfType<FurniturePlacement>();
        this.touchHandler = FindObjectOfType<TouchHandler>();
        this.graphicRayCaster = FindObjectOfType<GraphicRaycaster>();
        this.eventSystem = FindObjectOfType<EventSystem>();
        this.planeManager = FindObjectOfType<ARPlaneManager>();
        this.planeAreaManager = FindObjectOfType<PlaneManager>().GetComponent<PlaneAreaManager>();

        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    void LateUpdate()
    {
        this.instructions.transform.parent.gameObject.SetActive(true);
        this.instructions.enabled = true;

        if (PlaneManager.TrackingStatusIsTrackedAndNormal && planeManager.trackables.count > 0)
        {
            this.screenReticle.alpha = 0;

            if (this.furniturePlacement.IsPlaced)
            {
                Vector3 furnitureSize = this.furniture.GetComponent<MeshCollider>().bounds.size;

                if (furnitureSize.x > planeAreaManager.planeWidth || furnitureSize.y > planeAreaManager.planeHeight)
                {
                    this.instructions.text = "There is not enough space for the furniture";
                }
                else
                {
                    this.instructions.text = "Touch and drag to move the furniture.\nTwo fingers to rotate" +
                    ((this.touchHandler.enablePinchScaling) ? " or pinch to scale." : ".");
                }
            }
            else
            {
                this.instructions.text = "Tap to place the furniture.";
            }
        }
        else
        {
            this.screenReticle.alpha = 1;

            this.instructions.text = "Point device towards ground.";
        }
    }

    void OnDestroy()
    {
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    public bool IsCanvasButtonPressed()
    {
        pointerEventData = new PointerEventData(this.eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        this.graphicRayCaster.Raycast(pointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Toggle>() ||
                result.gameObject.GetComponent<Button>())
            {
                resultIsButton = true;
                break;
            }
        }
        return resultIsButton;
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        string statusMessage = "";

        switch (statusInfo)
        {
            case TrackableBehaviour.StatusInfo.UNKNOWN:
                statusMessage = "Limited Status";
                break;
            case TrackableBehaviour.StatusInfo.INITIALIZING:
                statusMessage = "Point your device to the floor and move to scan";
                break;
            case TrackableBehaviour.StatusInfo.EXCESSIVE_MOTION:
                statusMessage = "Move slower";
                break;
            case TrackableBehaviour.StatusInfo.INSUFFICIENT_FEATURES:
                statusMessage = "Not enough visual features in the scene";
                break;
            case TrackableBehaviour.StatusInfo.INSUFFICIENT_LIGHT:
                statusMessage = "Not enough light in the scene";
                break;
            default:
                statusMessage = "";
                break;
        }

        this.instructions.text = statusMessage;
    }
}
