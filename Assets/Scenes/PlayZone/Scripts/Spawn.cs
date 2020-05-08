using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Spawn : MonoBehaviour
{
    public GameObject handController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "spawned_cube";
        cube.GetComponent<Renderer>().material.color = Color.white;
        cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        cube.transform.position = handController.transform.position;
        cube.AddComponent<BoxCollider>();
        cube.AddComponent<Rigidbody>();
        cube.AddComponent<XRGrabInteractable>();
    }
}
