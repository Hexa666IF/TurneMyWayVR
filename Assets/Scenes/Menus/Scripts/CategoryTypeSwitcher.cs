using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTypeSwitcher : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private GameObject furnitureManager;
    [SerializeField]
    private string defaultLabel = "By Room";

    // Set label 
    public void SetLabel(string label)
    {
        if (this.label != null)
        {
            this.label.text = label;
        }

    }

    // Click handler
    public void HandleClick()
    {
        if (this.furnitureManager != null)
        {
            FurnitureManagerScript furnitureManagerScript = furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript != null)
            {
                bool isType = furnitureManagerScript.ToggleCategoryType();
                this.SetLabel(isType ? "By Type" : "By Room");
            }
            else
            {
                Debug.Log("FurnitureManagerScript is not defined");
            }
        }
        else
        {
            Debug.Log("FurnitureManager is not defined");
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        this.SetLabel(this.defaultLabel);
    }

}
