using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] GameObject furnituresPopup;
    [SerializeField] GameObject addButton;
    [SerializeField] GameObject instructionsUI;
    
    public void OpenFurnituresPopup()
    {
        furnituresPopup.SetActive(true);
        addButton.SetActive(false);
        instructionsUI.SetActive(false);
    }

    public void CloseFurnituresPopup()
    {
        furnituresPopup.SetActive(false);
        addButton.SetActive(true);
        instructionsUI.SetActive(true);
    }
}
