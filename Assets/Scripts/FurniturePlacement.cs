using UnityEngine;
using Vuforia;

public class FurniturePlacement : MonoBehaviour
{
   public bool IsPlaced { get; private set; }

    [Header("Augmentation Objects")]
    [SerializeField] GameObject furniture = null;
    [SerializeField] GameObject furnitureShadow = null;
    [SerializeField] string furnitureName = null;

    [Header("Control Indicators")]
    [SerializeField] GameObject translationIndicator = null;
    [SerializeField] GameObject rotationIndicator = null;

    [Header("Augmentation Size")]
    [Range(0.1f, 2.0f)]
    [SerializeField] float furnitureSize = 0.65f;

    MeshRenderer furnitureRenderer;
    MeshRenderer furnitureShadowRenderer;
    Material[] furnitureMaterials;
    Material furnitureShadowMaterial;
    InstructionsUI instructionsUI;
    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;
    float augmentationScale;
    Vector3 furnitureScale;
    string floorName;

    void Start()
    {
        this.mainCamera = Camera.main;
        this.instructionsUI = FindObjectOfType<InstructionsUI>();
        
        SetupFurniture();

        Reset();
    }


    void Update()
    {
        if (this.IsPlaced)
        {
            this.rotationIndicator.SetActive(Input.touchCount == 2);

            this.translationIndicator.SetActive(
                (TouchHandler.IsSingleFingerDragging || TouchHandler.IsSingleFingerStationary) && !this.instructionsUI.IsCanvasButtonPressed());

            if (TouchHandler.IsSingleFingerDragging)
            {
                if (!this.instructionsUI.IsCanvasButtonPressed())
                {
                    this.cameraToPlaneRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(this.cameraToPlaneRay, out this.cameraToPlaneHit))
                    {
                        if (this.cameraToPlaneHit.collider.gameObject.name == floorName)
                        {
                            this.furniture.PositionAt(this.cameraToPlaneHit.point);
                        }
                    }
                }
            }
        }
        else
        {
            UtilityHelper.RotateTowardCamera(this.furniture);
            this.rotationIndicator.SetActive(false);
            this.translationIndicator.SetActive(false);
        }
    }

    public void Reset()
    {
        this.furniture.transform.position = Vector3.zero;
        this.furniture.transform.localEulerAngles = Vector3.zero;
        this.furniture.transform.localScale = this.furnitureScale;

        this.IsPlaced = false;
    }

    public void LoadFurniture(GameObject furnitureObject, string name)
    {
        this.furniture = furnitureObject;
        // this.furnitureShadow = GameObject.Find($"{name}Shadow");
        this.furnitureName = name;

        SetupFurniture();
    }

    public GameObject GetFurniture()
    {
        return this.furniture;
    }

    public void PlaceFurnitureAtAnchor(Transform anchor)
    {
        this.furniture.transform.SetParent(anchor, true);
        this.furniture.transform.localPosition = Vector3.zero;
        this.IsPlaced = true;
    }

    public void PlaceFurnitureAtAnchorFacingCamera(Transform anchor)
    {
        PlaceFurnitureAtAnchor(anchor);

        UtilityHelper.RotateTowardCamera(this.furniture);
    }

    public void DetachFurnitureFromAnchor()
    {
        this.furniture.transform.SetParent(null);
        this.IsPlaced = false;
    }

    void SetupFurniture()
    {
        this.furnitureRenderer = this.furniture.GetComponent<MeshRenderer>();
        // this.furnitureShadowRenderer = this.furnitureShadow.GetComponent<MeshRenderer>();

        SetupMaterials();
        SetupFloor();


        this.augmentationScale = this.furnitureSize;

        this.furnitureScale =
            new Vector3(this.augmentationScale,
                        this.augmentationScale,
                        this.augmentationScale);

        this.furniture.transform.localScale = this.furnitureScale;
    }

    void SetupMaterials()
    {
        this.furnitureMaterials = new Material[]
        {
            Resources.Load<Material>($"{this.furnitureName}Body"),
            Resources.Load<Material>($"{this.furnitureName}Frame")
        };

        // this.furnitureShadowMaterial = Resources.Load<Material>($"{this.furnitureName}Shadow");
    }

    void SetupFloor()
    {
        this.floorName = "Floor";
        GameObject floor = new GameObject(this.floorName, typeof(BoxCollider));
        floor.transform.SetParent(this.furniture.transform.parent);
        floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        floor.transform.localScale = Vector3.one;
        floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
    }

    void SetVisible(bool visible)
    {
        this.furnitureRenderer.enabled = visible; //this.furnitureShadowRenderer.enabled = visible;
    }
}
