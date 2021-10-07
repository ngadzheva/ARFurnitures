using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUI : MonoBehaviour
{
    [SerializeField] GameObject furnituresPopup;
    [SerializeField] GameObject addButton;
    [SerializeField] GameObject groundPlaneUI;
    
    public void OpenFurnituresPopup()
    {
        furnituresPopup.SetActive(true);
        addButton.SetActive(false);
        groundPlaneUI.SetActive(false);
    }

    public void CloseFurnituresPopup()
    {
        furnituresPopup.SetActive(false);
        addButton.SetActive(true);
        groundPlaneUI.SetActive(true);
    }
}
