﻿using UnityEngine;
using Vuforia;

public class ProductPlacement : MonoBehaviour
{
   public bool IsPlaced { get; private set; }

    [Header("Augmentation Objects")]
    [SerializeField] GameObject product = null;
    [SerializeField] GameObject productShadow = null;

    [Header("Control Indicators")]
    [SerializeField] GameObject translationIndicator = null;
    [SerializeField] GameObject rotationIndicator = null;

    [Header("Augmentation Size")]
    [Range(0.1f, 2.0f)]
    [SerializeField] float productSize = 0.65f;

    MeshRenderer productRenderer;
    MeshRenderer productShadowRenderer;
    Material[] productMaterials, productMaterialsTransparent;
    Material productShadowMaterial, productShadowMaterialTransparent;

    GroundPlaneUI groundPlaneUI;
    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;

    float augmentationScale;
    Vector3 productScale;
    string floorName;

    bool productVisibilityConditionsMet
    {
        get
        {
            return PlaneManager.TrackingStatusIsTrackedOrLimited && PlaneManager.GroundPlaneHitReceived;
        }
    }
    void Start()
    {
        this.mainCamera = Camera.main;
        this.groundPlaneUI = FindObjectOfType<GroundPlaneUI>();
        this.productRenderer = this.product.GetComponent<MeshRenderer>();
        this.productShadowRenderer = this.productShadow.GetComponent<MeshRenderer>();

        SetupMaterials();
        SetupFloor();


        this.augmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : this.productSize;

        this.productScale =
            new Vector3(this.augmentationScale,
                        this.augmentationScale,
                        this.augmentationScale);

        this.product.transform.localScale = this.productScale;
    }


    void Update()
    {
        EnablePreviewModeTransparency(!this.IsPlaced);

        if (this.IsPlaced)
        {
            this.rotationIndicator.SetActive(Input.touchCount == 2);

            this.translationIndicator.SetActive(
                (TouchHandler.IsSingleFingerDragging || TouchHandler.IsSingleFingerStationary) && !this.groundPlaneUI.IsCanvasButtonPressed());

            if (TouchHandler.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {
                if (!this.groundPlaneUI.IsCanvasButtonPressed())
                {
                    this.cameraToPlaneRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(this.cameraToPlaneRay, out this.cameraToPlaneHit))
                    {
                        if (this.cameraToPlaneHit.collider.gameObject.name == floorName)
                        {
                            this.product.PositionAt(this.cameraToPlaneHit.point);
                        }
                    }
                }
            }
        }
        else
        {
            UtilityHelper.RotateTowardCamera(this.product);
            this.rotationIndicator.SetActive(false);
            this.translationIndicator.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (!this.IsPlaced)
        {
            SetVisible(this.productVisibilityConditionsMet);
        }
    }

    public void Reset()
    {
        this.product.transform.position = Vector3.zero;
        this.product.transform.localEulerAngles = Vector3.zero;
        this.product.transform.localScale = this.productScale;
    }

    public void PlaceProductAtAnchor(Transform anchor)
    {
        this.product.transform.SetParent(anchor, true);
        this.product.transform.localPosition = Vector3.zero;
        this.IsPlaced = true;
    }

    public void PlaceProductAtAnchorFacingCamera(Transform anchor)
    {
        PlaceProductAtAnchor(anchor);

        UtilityHelper.RotateTowardCamera(this.product);
    }

    public void DetachProductFromAnchor()
    {
        this.product.transform.SetParent(null);
        this.IsPlaced = false;
    }

    void SetupMaterials()
    {
        this.productMaterials = new Material[]
        {
            Resources.Load<Material>("ChairBody"),
            Resources.Load<Material>("ChairFrame")
        };

        this.productMaterialsTransparent = new Material[]
        {
            Resources.Load<Material>("ChairBodyTransparent"),
            Resources.Load<Material>("ChairFrameTransparent")
        };

        this.productShadowMaterial = Resources.Load<Material>("ChairShadow");
        this.productShadowMaterialTransparent = Resources.Load<Material>("ChairShadowTransparent");
    }

    void SetupFloor()
    {
        if (VuforiaRuntimeUtilities.IsPlayMode())
        {
            this.floorName = "Emulator Ground Plane";
        }
        else
        {
            this.floorName = "Floor";
            GameObject floor = new GameObject(this.floorName, typeof(BoxCollider));
            floor.transform.SetParent(this.product.transform.parent);
            floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            floor.transform.localScale = Vector3.one;
            floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        }
    }

    void SetVisible(bool visible)
    {
        this.productRenderer.enabled = this.productShadowRenderer.enabled = visible;
    }

    void EnablePreviewModeTransparency(bool previewEnabled)
    {
        this.productRenderer.materials = previewEnabled ? this.productMaterialsTransparent : this.productMaterials;
        this.productShadowRenderer.material = previewEnabled ? this.productShadowMaterialTransparent : this.productShadowMaterial;
    }
}