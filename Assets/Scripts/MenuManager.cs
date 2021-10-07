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
        commonUI = FindObjectOfType<CommonUI>();
        planeManager = FindObjectOfType<PlaneManager>();
        productPlacement = FindObjectOfType<ProductPlacement>();
        touchHandler = FindObjectOfType<TouchHandler>();

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

        Debug.Log("Current product --------------------------------------");
        Debug.Log(productPlacement.GetProduct().name);
        
        GameObject product = GameObject.Find(allFurnitures[itemIndex].name);
        Debug.Log("New product --------------------------------------");
        Debug.Log(product.name);
        // product.SetActive(true);

        // planeManager.LoadPlacementAugmentation(product);
        // productPlacement.LoadProduct(product, allFurnitures[itemIndex].name);
        // touchHandler.LoadAugmentationObject(product);

        commonUI.CloseFurnituresPopup();
    }
}
