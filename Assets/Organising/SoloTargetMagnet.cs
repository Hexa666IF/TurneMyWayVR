using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This magnet will pull only the nearest of the correctly tagged GameObject to it.
// This is useful for desks for example. (Only one chair can be pulled to the desk)
public class SoloTargetMagnet : Magnet
{

    // This Init function looks for the nearest GameObject with according tag.
    // This object is inserted into the furniture list inherited from Magnet,
    // so that the rest of its behaviour is identical to Magnet.
    // => A SoloTargetMagnet is a magnet with only one GameObject into the
    //    furniture list.
    public override void Init()
    {
        GameObject [] tagged = GameObject.FindGameObjectsWithTag(tagPulled);
        GameObject nearestGameObject = null;
        double dist_min = double.MaxValue;
        double distance;
        Transform magnetTransform = GetComponent<Transform>();

        foreach(GameObject o in tagged)
        {
            Transform objTransform = o.transform;
            Vector3 vector = objTransform.position - magnetTransform.position;
            distance = Vector3.Dot(vector, vector);
            if(distance < dist_min)
            {
                dist_min = distance;
                nearestGameObject = o;
            }
        }

        // furniture = new List<GameObject>();
        if(nearestGameObject != null)
        {
            furniture.Add(nearestGameObject);
            Debug.Log(nearestGameObject.name + " is ready to be pulled by " + name);
        }

        SetConstraints();
        isActive = true;
    }
}
