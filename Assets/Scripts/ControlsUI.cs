using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject instructionsUI;
    
    public void OpenPopup(string popupType)
    {
        GameObject popup = transform.Find(popupType).gameObject;

        popup.SetActive(true);
        buttons.SetActive(false);
        instructionsUI.SetActive(false);
    }

    public void ClosePopup(string popupType)
    {
        GameObject popup = transform.Find(popupType).gameObject;
        
        popup.SetActive(false);
        buttons.SetActive(true);
        instructionsUI.SetActive(true);
    }
}
