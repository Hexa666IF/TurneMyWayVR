using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;

public class PlayZoneSetup : MonoBehaviour
{
    public Material newMaterialRef;


    // Start is called before the first frame update
    void Start()
    {
        genererMurs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void genererMurs()
    {
        List<Vector3> points = GetBoudariesGuardian();

        List<Vector2> contours = new List<Vector2>();

        for(int i = 0; i<points.Count; i++)
        {
            contours.Add(new Vector2(points[i].x,points[i].z));
        }

        int min_angle = 12;
        double min_distance = 0.1;

        List<Vector2> corners = findCorners(contours, min_angle, min_distance);

        CreateWalls(corners);

    }

    private List<Vector3> GetBoudariesGuardian()
    {
        List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(inputSubsystems);
        if (inputSubsystems.Count > 0)
        {
            List<Vector3> boundary = new List<Vector3>();
            if (inputSubsystems[0].TryGetBoundaryPoints(boundary))
            {
                return boundary;
            }
        }
        return null;
    }

    List<Vector2> findCorners( List<Vector2> contours, int min_angle, double min_distance )
    {
        List<Vector2> ret = new List<Vector2>();
        Vector2 p1, p2, p3;
        double prevAngle = 0;
        double distance = 0;
    
        int cc = 0;
    
        for( int i = 0; i < (contours.Count + 3); i++ )
        {
            if( i > 2 )
            {
                if( i - contours.Count < 0 ) //Handle the overlapping steps of the loop
                {
                    p1 = contours[i - 2];
                    p2 = contours[i - 1];
                    p3 = contours[i];
                }
                else if( i - contours.Count == 0 )
                {
                    p1 = contours[i - 2];
                    p2 = contours[i - 1];
                    p3 = contours[0];
                }
                else if( i - contours.Count == 1 )
                {
                    p1 = contours[i - 2];
                    p2 = contours[0];
                    p3 = contours[1];
                }
                else
                {
                    p1 = contours[i - contours.Count - 2];
                    p2 = contours[i - contours.Count - 1];
                    p3 = contours[i - contours.Count];
                }
    
                //Calculate angle between points 1 and 3
                double currAngle = Math.Atan2( p1.y - p3.y, p1.x - p3.x ) * 180 / Math.PI;
                if( currAngle < 0 )
                {
                    currAngle = (currAngle * -1);
                }
    
                if( i > 3 )
                {
                    //calculate the difference between this angle and the previous one
                    double diffAngle = Math.Abs(prevAngle - currAngle);
    
                    //Add point to return array if angle diff is above threshold
                    if( diffAngle > min_angle )
                    {
                        //Ignore points that are closer than "min_distance pixels" to the previous point
                        if( cc > 0 )
                        {
                            double dx = ret[cc - 1].x - p1.x;  // no need for if-else or fabs
                            double dy = ret[cc - 1].y - p1.y;
                            distance = Math.Sqrt( (dx * dx) + (dy * dy) );  // dx*dx will always be positive
                            
                            if( distance >= min_distance )
                            {
                                ret.Add(p1);
                                cc++;
                            }
                        }
                        else
                        {
                            ret.Add(p1);
                            cc++;
                        }
    
                    }
                }
    
                prevAngle = currAngle;
            }
    
        }
        
        Debug.Log("Find corners: found "+cc+" corners");

        return ret;
    }

    private void CreateWalls(List<Vector2> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (i != points.Count - 1)
            {
                GameObject startObj = new GameObject();
                GameObject endObj = new GameObject();
                Vector3 start = new Vector3(points[i].x, 1.5f, points[i].y);
                Vector3 end = new Vector3(points[i + 1].x, 1.5f, points[i + 1].y);
                startObj.transform.localPosition = start;
                endObj.transform.localPosition = end;
                startObj.transform.LookAt(endObj.transform.position);
                endObj.transform.LookAt(startObj.transform.position);
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.tag = "Wall";

                wall.AddComponent<MeshCollider>(); // Add the rigidbody.

                // Set material
                Renderer wallRender = wall.GetComponentsInChildren<Renderer>()[0];
                wallRender.material = newMaterialRef;

                // Get distance

                float distance = Vector3.Distance(start, end);
                // Set position
                wall.name = "Wall" + start.x + "," + start.y + "," + end.x + "," + end.y;
                wall.transform.localScale = new Vector3(0.0001f, 3f, distance);
                wall.transform.position = startObj.transform.position + distance / 2 * startObj.transform.forward;
                wall.transform.rotation = startObj.transform.rotation;

                Destroy(startObj);
                Destroy(endObj);
            }
            else
            {
                GameObject startObj = new GameObject();
                GameObject endObj = new GameObject();
                Vector3 start = new Vector3(points[i].x, 1.5f, points[i].y);
                Vector3 end = new Vector3(points[0].x, 1.5f, points[0].y);
                startObj.transform.localPosition = start;
                endObj.transform.localPosition = end;
                startObj.transform.LookAt(endObj.transform.position);
                endObj.transform.LookAt(startObj.transform.position);

                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.tag = "Wall";
                
                // Set color
                Renderer wallRender = wall.GetComponentsInChildren<Renderer>()[0];
                wallRender.material = newMaterialRef;

                // Get distance

                float distance = Vector3.Distance(start, end);
                // Set position
                wall.name = "Wall" + start.x + "," + start.y + "," + end.x + "," + end.y;
                wall.transform.localScale = new Vector3(0.0001f, 3f, distance);
                wall.transform.position = startObj.transform.position + distance / 2 * startObj.transform.forward;
                wall.transform.rotation = startObj.transform.rotation;

                Destroy(startObj);
                Destroy(endObj);
            }
        }


    }
}
