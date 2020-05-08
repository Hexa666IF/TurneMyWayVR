using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowHideUI : MonoBehaviour
{
    public ButtonHandler menuClickHandler = null;
    public GameObject canvas;
    public GameObject camera;

    public void OnEnable()
    {
        menuClickHandler.OnButtonDown += ChangeUIStatus;
    }

    public void OnDisable()
    {
        menuClickHandler.OnButtonDown -= ChangeUIStatus;
    }

    private void ChangeUIStatus(XRController controller)
    {
        if (canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
            //TODO : set position to direction of headset
        }
    }
}
