using UnityEngine;
using Vuforia;

public class ProductPlacement : MonoBehaviour
{
   public bool IsPlaced { get; private set; }

    [Header("Augmentation Objects")]
    [SerializeField] GameObject product = null;
    [SerializeField] GameObject productShadow = null;
    [SerializeField] string productName = null;

    [Header("Control Indicators")]
    [SerializeField] GameObject translationIndicator = null;
    [SerializeField] GameObject rotationIndicator = null;

    [Header("Augmentation Size")]
    [Range(0.1f, 2.0f)]
    [SerializeField] float productSize = 0.65f;

    MeshRenderer productRenderer;
    MeshRenderer productShadowRenderer;
    Material[] productMaterials;
    Material productShadowMaterial;
    GroundPlaneUI groundPlaneUI;
    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;
    float augmentationScale;
    Vector3 productScale;
    string floorName;

    void Start()
    {
        this.mainCamera = Camera.main;
        this.groundPlaneUI = FindObjectOfType<GroundPlaneUI>();
        
        SetupProduct();

        Reset();
    }


    void Update()
    {
        if (this.IsPlaced)
        {
            this.rotationIndicator.SetActive(Input.touchCount == 2);

            this.translationIndicator.SetActive(
                (TouchHandler.IsSingleFingerDragging || TouchHandler.IsSingleFingerStationary) && !this.groundPlaneUI.IsCanvasButtonPressed());

            if (TouchHandler.IsSingleFingerDragging)
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

    public void Reset()
    {
        this.product.transform.position = Vector3.zero;
        this.product.transform.localEulerAngles = Vector3.zero;
        this.product.transform.localScale = this.productScale;

        this.IsPlaced = false;
    }

    public void LoadProduct(GameObject productObject, string name)
    {
        this.product = productObject;
        // this.productShadow = GameObject.Find($"{name}Shadow");
        this.productName = name;

        SetupProduct();
    }

    public GameObject GetProduct()
    {
        return this.product;
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

    void SetupProduct()
    {
        this.productRenderer = this.product.GetComponent<MeshRenderer>();
        // this.productShadowRenderer = this.productShadow.GetComponent<MeshRenderer>();

        SetupMaterials();
        SetupFloor();


        this.augmentationScale = this.productSize;

        this.productScale =
            new Vector3(this.augmentationScale,
                        this.augmentationScale,
                        this.augmentationScale);

        this.product.transform.localScale = this.productScale;
    }

    void SetupMaterials()
    {
        this.productMaterials = new Material[]
        {
            Resources.Load<Material>($"{this.productName}Body"),
            Resources.Load<Material>($"{this.productName}Frame")
        };

        // this.productShadowMaterial = Resources.Load<Material>($"{this.productName}Shadow");
    }

    void SetupFloor()
    {
        this.floorName = "Floor";
        GameObject floor = new GameObject(this.floorName, typeof(BoxCollider));
        floor.transform.SetParent(this.product.transform.parent);
        floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        floor.transform.localScale = Vector3.one;
        floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
    }

    void SetVisible(bool visible)
    {
        this.productRenderer.enabled = visible; //this.productShadowRenderer.enabled = visible;
    }
}
