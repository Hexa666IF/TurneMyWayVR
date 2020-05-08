using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class MoveGrabbedObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var interactor = GetComponent<XRBaseInteractor>();
        var controller = GetComponent<XRController>();

        XRGrabInteractable grabbable = interactor.selectTarget as XRGrabInteractable;

        if (grabbable != null && grabbable.attachTransform != null)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(controller.controllerNode);

            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbDirection);

            float moveAwayAmount = -0.1f*thumbDirection.y;

            Vector3 vectorToMoveObjectAlong = grabbable.attachTransform.parent.InverseTransformVector(controller.transform.forward);

            if ((grabbable.attachTransform.localPosition + moveAwayAmount * vectorToMoveObjectAlong).z <= 0)
            {
                grabbable.attachTransform.localPosition += moveAwayAmount * vectorToMoveObjectAlong;
            }

            MethodInfo unity_UpdateInteractorLocalPose = typeof(XRGrabInteractable).GetMethod("UpdateInteractorLocalPose", BindingFlags.NonPublic | BindingFlags.Instance);
            unity_UpdateInteractorLocalPose.Invoke(grabbable, new object[] { interactor });
        }
    }
}
