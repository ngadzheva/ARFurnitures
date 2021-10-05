using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vuforia;
using UnityEngine.XR.ARFoundation;

public class GroundPlaneUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text instructions = null;
    [SerializeField] CanvasGroup screenReticle = null;
    [SerializeField] Text area = null;
    [SerializeField] GameObject product = null;
 
    GraphicRaycaster graphicRayCaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    ProductPlacement productPlacement;
    TouchHandler touchHandler;

    PlaneAreaBehaviour planeAreaBehaviour;

    void Start()
    {
        this.productPlacement = FindObjectOfType<ProductPlacement>();
        this.touchHandler = FindObjectOfType<TouchHandler>();
        this.graphicRayCaster = FindObjectOfType<GraphicRaycaster>();
        this.eventSystem = FindObjectOfType<EventSystem>();

        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    void LateUpdate()
    {
        this.area.transform.parent.gameObject.SetActive(true);
        this.area.enabled = true;
        this.area.text = product.GetComponent<Collider>().bounds.size.x.ToString();

        if (PlaneManager.TrackingStatusIsTrackedAndNormal) // && PlaneManager.GroundPlaneHitReceived
        {
            this.screenReticle.alpha = 0;

            this.instructions.transform.parent.gameObject.SetActive(true);
            this.instructions.enabled = true;

            if (this.productPlacement.IsPlaced)
            {
                Vector3 productSize = product.GetComponent<Collider>().bounds.size;

                if (productSize.x > planeAreaBehaviour.planeWidth || productSize.y > planeAreaBehaviour.planeHeight)
                {
                    this.instructions.text = "There is not enough space for the product";
                }
                else
                {
                    this.instructions.text = "Touch and drag to move the product.\nTwo fingers to rotate" +
                    ((this.touchHandler.enablePinchScaling) ? " or pinch to scale." : ".");
                }
            }
            else
            {
                this.instructions.text = "Tap to place the product.";
            }
        }
        else
        {
            // if (!PlaneManager.GroundPlaneHitReceived)
            // {
            //     this.screenReticle.alpha = 1;
            // }

            // this.screenReticle.alpha = 1;

            this.instructions.transform.parent.gameObject.SetActive(true);
            this.instructions.enabled = true;
            this.instructions.text = "Point device towards ground";

            // this.instructions.text = PlaneManager.GroundPlaneHitReceived ?
            //     "Move to get better tracking for placing an anchor" : "Point device towards ground";
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
