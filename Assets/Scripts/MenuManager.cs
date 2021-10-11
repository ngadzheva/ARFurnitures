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
    private ControlsUI controlsUI;
    private PlaneManager planeManager;
    private FurniturePlacement furniturePlacement;
    private TouchHandler touchHandler;

    void Start()
    {
        this.controlsUI = FindObjectOfType<ControlsUI>();
        this.planeManager = FindObjectOfType<PlaneManager>();
        this.furniturePlacement = FindObjectOfType<FurniturePlacement>();
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
        furniturePlacement.GetFurniture().SetActive(false);
        furniturePlacement.DetachFurnitureFromAnchor();

        GameObject selectedFurniture = GetSelecteditem(allFurnitures[itemIndex].name);
        selectedFurniture.SetActive(true);

        planeManager.LoadPlacementAugmentation(selectedFurniture);
        furniturePlacement.LoadFurniture(selectedFurniture, selectedFurniture.name);
        touchHandler.LoadAugmentationObject(selectedFurniture);

        controlsUI.CloseFurnituresPopup();
    }

    GameObject GetSelecteditem(string name)
    {
        return GameObject.Find("Anchor_Placement").transform.Find(name).gameObject;
    }
}
