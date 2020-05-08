using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndOrganise : MonoBehaviour
{
    public ButtonHandler aClickHandler = null;
    public GameObject furnitureManager;

    public void OnEnable()
    {
        aClickHandler.OnButtonDown += EndOrganiseClick;
    }

    public void OnDisable()
    {
        aClickHandler.OnButtonDown -= EndOrganiseClick;
    }

    private void EndOrganiseClick(XRController controller)
    {
        if (this.furnitureManager != null)
        {
            FurnitureManagerScript furnitureManagerScript = this.furnitureManager.GetComponent<FurnitureManagerScript>();
            if (furnitureManagerScript != null)
            {
                furnitureManagerScript.EndOrganise();
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
