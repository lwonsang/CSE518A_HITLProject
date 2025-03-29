using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public GameObject mainMenu;
    public GameObject controlsMenu;

    void Awake()
    {
        instance = this;
    }

    void SwitchMenu(GameObject menu)
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(false);
        menu.SetActive(true);
        
    }

    public void ShowMainMenu()
    {
        SwitchMenu(mainMenu);
    }

    public void ShowControlsMenu()
    {
        SwitchMenu(controlsMenu);
    }
}
