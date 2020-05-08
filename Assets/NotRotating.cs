using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotRotating : MonoBehaviour
{
    Vector3 startEulerAngles;
    float yPos;
    bool yFixed = false;
    // Start is called before the first frame update
    void Start()
    {
        startEulerAngles = GetComponent<Transform>().localEulerAngles;
        Debug.Log("WHEN DO YOU START ?!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetYFixed(bool yFixed)
    {
        this.yFixed = yFixed;
    }

    protected void LateUpdate()
    {
        Transform transform = GetComponent<Transform>();
        transform.localEulerAngles = new Vector3(startEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        //transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);

        if (yFixed)
        {
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
        else
        {
            yPos = transform.position.y;
        }
    }

}
