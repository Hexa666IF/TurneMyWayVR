using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


// DirectionMagnet : for pulling furnitures according to a direction.

// This Magnet Behaviour script pulls every GameObjects according to their tag toward the opposite of its forward vector.
// It inherits from Magnet Class Behaviour. For related propreties, please refer to the Magnet.cs file.
public class DirectionMagnet : Magnet
{

    // Update is called once per frame
    protected override void ApplyForces()
    {
        // Here we assume that the forward vector is pointing inside the room !
        Transform magnetTransform = GetComponent<Transform>();
        Vector3 pullDirection = -magnetTransform.forward;
        
        foreach(GameObject obj in furniture)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Transform objTransform = obj.GetComponent<Transform>();

            if(isForceConstant)
            {
                rb.AddForce(pullDirection*force);
            }
            else
            {
                float distance = Vector3.Dot((objTransform.position - magnetTransform.position), pullDirection);
                distance = Math.Abs(distance);
                rb.AddForce(pullDirection*force*(1/distance));
            }

            if(isPolarised)
                rb.AddTorque(objTransform.up * CalculateTorque(pullDirection, objTransform.forward));
            
        }
    }

}
