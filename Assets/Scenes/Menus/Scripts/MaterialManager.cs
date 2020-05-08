using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialManager : MonoBehaviour
{
    [SerializeField]
    private Text switcherLabel;
    [SerializeField]
    private GameObject furnitureManager;
    [SerializeField]
    private string materialType = "Walls";
    [SerializeField]
    private List<Material> floorMaterials = new List<Material>();
    [SerializeField]
    private List<Material> wallMaterials = new List<Material>();
    [SerializeField]
    private GameObject materialPicker;
    [SerializeField]
    private GameObject materialContentList;
    [SerializeField]
    private GameObject materialButtonTemplate;
    private List<GameObject> materialButtonList = new List<GameObject>();

    // Set label 
    public void SetSwitcherLabel(string label)
    {
        if (this.switcherLabel != null)
        {
            this.switcherLabel.text = label == "Walls" ? "Floor" : "Walls";
        }

    }

    // Toggle handler
    public void HandleToggleClick()
    {
        this.materialType = this.materialType == "Walls" ? "Floor" : "Walls";
        this.SetSwitcherLabel(this.materialType);
        this.GenerateMaterialButtons();

    }

    // Handle material selection
    public void HandleMaterialSelect(Material material)
    {
        switch (this.materialType)
        {
            case "Walls":
                // Set wall material
                Debug.Log("Setting wall material to " + material.name);
                // GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
                FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
                List<GameObject> walls = furnitureManagerScript.GetWalls();
                if (walls != null)
                {
                    foreach (GameObject wall in walls)
                    {
                        Renderer wallRender = wall.GetComponentsInChildren<Renderer>()[0];
                        wallRender.material = material;
                    }
                }
                break;
            case "Floor":
                // Set floor material
                Debug.Log("Setting floor material to " + material.name);
                GameObject floor = GameObject.Find("Floor");
                if (floor != null)
                {
                    Renderer floorRender = floor.GetComponentsInChildren<Renderer>()[0];
                    floorRender.material = material;
                }
                break;
            default:
                break;
        }
    }

    // Generate category list buttons 
    public void GenerateMaterialButtons()
    {
        if (this.materialContentList != null && this.materialButtonTemplate != null)
        {
            this.DeleteExistingMaterialButtons();
            List<Material> materials = new List<Material>();
            switch (this.materialType)
            {
                case "Walls":
                    materials = this.wallMaterials;
                    break;
                case "Floor":
                    materials = this.floorMaterials;
                    break;
                default:
                    break;
            }

            foreach (Material material in materials)
            {
                GameObject materialButton = Instantiate(this.materialButtonTemplate) as GameObject;
                this.materialButtonList.Add(materialButton);
                materialButton.SetActive(true);
                materialButton.GetComponent<MaterialButton>().SetMaterial(material);
                materialButton.transform.SetParent(this.materialContentList.transform, false);
            }
        }
    }

    // Delete existing material list buttons
    private void DeleteExistingMaterialButtons()
    {
        foreach (GameObject materialButton in this.materialButtonList)
        {
            Destroy(materialButton.gameObject);
        }
        this.materialButtonList.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.SetSwitcherLabel(this.materialType);
        this.GenerateMaterialButtons();
    }

}
