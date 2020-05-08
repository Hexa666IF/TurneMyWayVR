using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    public List<ButtonHandler> allButtonHandlers = new List<ButtonHandler>();

    private XRController controller = null;

    private void Awake()
    {
        controller = GetComponent<XRController>();
    }

    private void Update()
    {
        HandleButtonEvents();
    }

    private void HandleButtonEvents()
    {
        foreach (ButtonHandler handler in allButtonHandlers)
        {
            handler.HandleState(controller);
        }
    }

    private void HandleAxis2DEvents()
    {

    }

    public void HandleAxisEvents()
    {

    }
}

