using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public enum InverseDenominator {dist = 1, dist_square = 2, dist_cube = 3};

// Magnet : pulls furnitures.

// This Behaviour script pulls every GameObjects according to their tag toward its position (transform.position).

// If the Magnet has been attached to a GameObject that is a Trigger, it will pull GameObject with the correct
// tag only when they are in its trigger zone.

// 7 properties can be tweaked in the Unity IDE :

//      tagPulled : The tag used to get GameObjects that need to be pulled. 
//                  By default : "furniture", but you can set any tag you want. Just be sure that the tag has been
//                  created in the scene, or the project.

//      force : intensity of the force applied to the furniture.

//      isForceConstant : define if the force is the same everywhere in space, or depends on the distance
//                        between the magnet and the furniture.

//      inverseFactor : The power of the inverse of the distance that need to be used to calculate the applied force
//                      to a gameObject. Note : It will not be used if the force is constant !

//      isPolarised : If true, the magnet will also apply torque to the pulled GameObjects, in order to align the
//                    forward vector of the GameObject and the opposite of the forward vector of the Magnet.  

//      torque : The maximum torque intensity that can be applied to a GameObject to allign its forward vector.
//               Note that this property is useless if isPolarised hasn't been checked.

//      isActive : Used to activate and deactivate the magnet while in the scene. An isActive equals to true will let
//                 the magnet apply forces to GameObjects.
//                 Beware, if you uncheck this box, you'll need a way to toggle the isActive attribute while in the
//                 scene to get the Magnet pulling the GameObjects.
public class Magnet : MonoBehaviour
{

    // The tag that is going to be used to pull the GameObject.
    public string tagPulled = "Furniture";

    public float force = 5;    

    // Define if the force applied to the furnitures is constant, or depends on the distance between
    // the furniture and this current GameObject.
    // If constantForce == true, then the same force will be applied everywhere in space.
    // Otherwise, it will decrease with the distance (1/d).
    public bool isForceConstant = false;
    public InverseDenominator inverseFactor = InverseDenominator.dist;

    // Set the Y coordinate of the pullVector to zero.
    public bool yToZero = true;

    // Define if the GameObject applies torque or not.
    public bool isPolarised = false;
    public float torque = 5;
    
    // Define if the magnet is pulling GameObjects toward it or not.
    public bool isActive = true;

    protected List<GameObject> furniture = new List<GameObject>();


// =============== Unity's GameObject overriden methods ======================

    // Start is called before the first frame update
    void Start()
    {
        // Init();
    }

    // Update is called once per frame
    // Applying force to furniture tagged GameObjects.
    void Update()
    {
        if(isActive && furniture != null)
            ApplyForces();
    }

    // Trigger Functions. 
    // They are used only if the Magnet is a trigger.

    void OnTriggerEnter(Collider other)
    {
        GameObject thing = other.gameObject;
        if(thing.tag == tagPulled)
        {
            furniture.Add(thing);
            Debug.Log(thing.name + " is now under effect of the magnet : " + name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject thing = other.gameObject;
        if(thing.tag == tagPulled)
        {
            furniture.Remove(thing);
            Debug.Log(thing.name + " is out of the area of effect of the magnet : " + name);
        }
    }


// ===================== Custom methods ============================


    // Fills the furniture list with the GameObject with the tag tagPulled.
    public virtual void Init()
    {
        if(GetComponent<Collider>().isTrigger == false)
        {
            furniture = new List<GameObject>(GameObject.FindGameObjectsWithTag(tagPulled));            
            foreach(GameObject obj in furniture)
            {
                Debug.Log(obj.name + " is ready to be pulled by " + name);
            }
        }
        else
        {
            // furniture = new List<GameObject>();
            Debug.Log(name + " is a trigger, its GameObject list starts empty.");
        }
        SetConstraints();
        isActive = true;
    }

    // Prevent magnet's gameObject & furnitures' gameobject from
    // rotating around X and Z axis, and translating by Y axis.
    public void SetConstraints()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if(rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotationZ 
                            | RigidbodyConstraints.FreezeRotationX
                            | RigidbodyConstraints.FreezePositionY;

        foreach(GameObject obj in furniture)
        {
            Rigidbody body = obj.GetComponent<Rigidbody>();
            if(body != null)
                body.constraints = RigidbodyConstraints.FreezeRotationZ 
                                    | RigidbodyConstraints.FreezeRotationX
                                    | RigidbodyConstraints.FreezePositionY;
        }
        
    }

    public void UpdateFurnitures(GameObject dispawned)
    {
        furniture.Remove(dispawned);
    }
    public void Reset()
    {
        isActive = false;
        UnsetConstraints();
        ClearFurniture();
    }

    protected void ClearFurniture()
    {
        furniture = new List<GameObject>();
    }

    // Remove every constraints applied to the magnet's GameObject
    // and the furnitures.
    public void UnsetConstraints()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
            rb.constraints = RigidbodyConstraints.None;
        
        foreach(GameObject obj in furniture)
        {
            Rigidbody body = obj.GetComponent<Rigidbody>();
            if(body != null)
                body.constraints = RigidbodyConstraints.None;
        }
    }

    protected float CalculateTorque(Vector3 direction, Vector3 objectRight)
    {
        float cosa = Vector3.Dot(direction, objectRight) / (direction.magnitude * objectRight.magnitude);
        return (1 - cosa)*torque;
    }

    // Apply a force to every GameObject in the furniture list to pull them toward the magnet. 
    protected virtual void ApplyForces()
    {
        Transform magnetTransform = GetComponent<Transform>();
        
        foreach(GameObject obj in furniture)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Transform objTransform = obj.GetComponent<Transform>();
            Vector3 pullVector = (magnetTransform.position - objTransform.position);

            if(yToZero)
                pullVector.y = 0;

            if(isForceConstant)
            {
                rb.AddForce( (pullVector.normalized) * force );
            }
            else
            {
                double distance = Math.Sqrt( Vector3.Dot(pullVector, pullVector) );
                distance = Math.Abs(distance);
                distance = Math.Pow(distance, (int) (inverseFactor));
                if(distance != 0)
                    rb.AddForce( pullVector.normalized * (float)(force*(1/distance)) );
            }

            if(isPolarised)
                rb.AddTorque(objTransform.up * CalculateTorque(pullVector, objTransform.forward));
                
        }
    }

    
}