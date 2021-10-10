using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate() {
            OnClick(param);
        });
    }
}

public class MenuManager : MonoBehaviour
{
    [Serializable]
    public struct Furniture
    {
        public string name;
        public string description;
        public Sprite icon;
    }

    [SerializeField] Furniture[] allFurnitures;
    private CommonUI commonUI;
    private PlaneManager planeManager;
    private ProductPlacement productPlacement;
    private TouchHandler touchHandler;

    void Start()
    {
        this.commonUI = FindObjectOfType<CommonUI>();
        this.planeManager = FindObjectOfType<PlaneManager>();
        this.productPlacement = FindObjectOfType<ProductPlacement>();
        this.touchHandler = FindObjectOfType<TouchHandler>();

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject newItem;

        int n = allFurnitures.Length;

        for (int i = 0; i < n; ++i)
        {
            newItem = Instantiate(buttonTemplate, transform);
            newItem.transform.GetChild(0).GetComponent<Image>().sprite = allFurnitures[i].icon;
            newItem.transform.GetChild(1).GetComponent<Text>().text = allFurnitures[i].name;
            newItem.transform.GetChild(2).GetComponent<Text>().text = allFurnitures[i].description;

            newItem.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

        Destroy(buttonTemplate);
    }

    void ItemClicked(int itemIndex)
    {      
        productPlacement.GetProduct().SetActive(false);

        GameObject selectedProduct = GetSelecteditem(allFurnitures[itemIndex].name);
        selectedProduct.SetActive(true);

        planeManager.LoadPlacementAugmentation(selectedProduct);
        productPlacement.LoadProduct(selectedProduct, selectedProduct.name);
        touchHandler.LoadAugmentationObject(selectedProduct);

        commonUI.CloseFurnituresPopup();
    }

    GameObject GetSelecteditem(string name)
    {
        return GameObject.Find("Anchor_Placement").transform.Find(name).gameObject;
    }
}
