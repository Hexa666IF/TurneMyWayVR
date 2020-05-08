using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureButton : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private GameObject furnitureManager;
    private GameObject furniture;
    [SerializeField]
    private Sprite furnitureSprite;
    protected static Dictionary<string, Sprite> spriteStore = new Dictionary<string, Sprite>();

    // Set Button Furniture
    public void SetFurniture(GameObject furniture)
    {
        this.label.text = furniture.name;
        this.furniture = furniture;
        this.SetImage();
    }

    // Get Button Furniture
    public GameObject GetFurniture()
    {
        return this.furniture;
    }

    // Click handler
    public void HandleClick()
    {
        if (this.furnitureManager != null)
        {
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript != null)
            {
                furnitureManagerScript.HandleFurnitureSelect(this.furniture);
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

    // Create and set sprite of furniture as button image
    public void SetImage()
    {
        string key = this.furniture.name;
        // If sprite in store, use it otherwise generate it
        if (FurnitureButton.spriteStore.ContainsKey(key))
        {
            this.furnitureSprite = FurnitureButton.spriteStore[key];
        }
        else
        {
            // Get FurnitureManagerScript instance
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();

            // Get instance of SnapshotCamera from FurnitureManager
            SnapshotCamera sc = furnitureManagerScript.GetSnapshotCamera();
            // Create a temporary furniture instance
            GameObject furnitureInstance = Instantiate(this.furniture) as GameObject;
            furnitureInstance.SetActive(true);

            // Generate a Texture2D from Furniture instance
            Texture2D tex = sc.TakeObjectSnapshot(furnitureInstance);

            // Generate sprite from texture, using whole texture and pivoting at centre
            this.furnitureSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // Save in store
            FurnitureButton.spriteStore.Add(key, this.furnitureSprite);

            // Delete temporary furniture instance
            Destroy(furnitureInstance.gameObject);
        }

        // Set sprite as button image
        Image image = gameObject.GetComponent<Image>();
        image.sprite = this.furnitureSprite; 
    }

    public void HandleHoverIn()
    {
        if (this.furnitureManager != null && this.furniture)
        {
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript != null)
            {
                furnitureManagerScript.HandleFurnitureHoverIn(this.furniture);
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

    public void HandleHoverOut()
    {
        if (this.furnitureManager != null && this.furniture)
        {
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript != null)
            {
                furnitureManagerScript.HandleFurnitureHoverOut(this.furniture);
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
}
