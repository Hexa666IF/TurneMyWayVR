using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToScanMenu()
    {
        SceneManager.LoadScene("ScanMenu");
    }

    public void GoToFurnitureMenu()
    {
        SceneManager.LoadScene("FurnitureMenu");
    }

    public void GoToPlayZone()
    {
        SceneManager.LoadScene("PlayZone");
    }
}
