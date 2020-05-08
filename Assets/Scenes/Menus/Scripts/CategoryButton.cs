using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private GameObject furnitureManager;
    private string category = "";

    // Set Button Category
    public void SetCategory(string category)
    {
        this.label.text = category;
        this.category = category;
    }

    // Get Button Category
    public string GetCategory()
    {
        return this.category;
    }

    // Click handler
    public void HandleClick()
    {
        if (this.furnitureManager!=null) {
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript!=null) {
                furnitureManagerScript.HandleCategorySelect(this.category);
            } else {
                Debug.Log("FurnitureManagerScript is not defined");
            }
        } else {
            Debug.Log("FurnitureManager is not defined");
        }
    }
}
