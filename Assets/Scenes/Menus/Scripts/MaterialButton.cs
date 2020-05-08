using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialButton : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private GameObject materialPicker;
    private Material material;

    // Set Button Material
    public void SetMaterial(Material material)
    {
        this.label.text = material.name;
        this.material = material;
    }

    // Get Button Material
    public Material GetMaterial()
    {
        return this.material;
    }

    // Click handler
    public void HandleClick()
    {
        if (this.materialPicker != null)
        {
            MaterialManager materialManager = this.materialPicker.GetComponent<MaterialManager>();
            if (materialManager != null && this.material != null)
            {
                materialManager.HandleMaterialSelect(this.material);
            }
        }
    }
}
