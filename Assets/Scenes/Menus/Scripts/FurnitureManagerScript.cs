using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class FurnitureManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject handController;
    [SerializeField]
    private GameObject categoryPicker;
    [SerializeField]
    private GameObject categoryContentList;
    [SerializeField]
    private GameObject furniturePicker;

    [SerializeField]
    private GameObject furnitureContentList;
    [SerializeField]
    private GameObject furnitureButtonTemplate;
    [SerializeField]
    private GameObject categoryButtonTemplate;
    [SerializeField]
    private GameObject spawnedFurniturePicker;
    [SerializeField]
    private GameObject spawnedFurnitureContentList;
    [SerializeField]
    private GameObject materialPicker;
    
    // private bool isShowingCategories = true;
    
    /** Category properties**/
    [SerializeField]
    private string defaultCategory = "Table";
    private string currentCategory = "";
    private bool isType = true;
    [SerializeField]
    private List<string> roomCategories = new List<string>();
    [SerializeField]
    private List<string> typeCategories = new List<string>();
    private List<GameObject> categoryButtonList = new List<GameObject>();

    /** Furniture properties**/
    private List<GameObject> furnitureList = new List<GameObject>();
    private List<GameObject> filteredFurnitureList = new List<GameObject>();
    private Dictionary<string, GameObject> spawnedFurnitureDict = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedFurnitureButtonList = new List<GameObject>();
    private GameObject currentFurniture;
    private List<GameObject> furnitureButtonList = new List<GameObject>();
    private int instanceCount = 0;

    private List<Magnet> magnetList = new List<Magnet>();

    [SerializeField]
    private GameObject cubeHighlight;

    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject instruction;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject inputManager;

    private SnapshotCamera snapshotCamera = null;

    [SerializeField]
    private Material newMaterialRef;
    private List<GameObject> walls = new List<GameObject>();

    [SerializeField]
    private GameObject Floor;
    [SerializeField]
    private PhysicMaterial noFriction;

    // Utility function to find child with a specific name
    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void ShowFurniturePicker()
    {
        this.furniturePicker.SetActive(true);
        this.categoryPicker.SetActive(false);
        this.spawnedFurniturePicker.SetActive(false);
        this.materialPicker.SetActive(false);
    }

    public void ShowCategoryPicker()
    {
        this.furniturePicker.SetActive(false);
        this.categoryPicker.SetActive(true);
        this.spawnedFurniturePicker.SetActive(false);
        this.materialPicker.SetActive(false);
    }

    public void ShowSpawnedFurniturePicker()
    {
        this.furniturePicker.SetActive(false);
        this.categoryPicker.SetActive(false);
        this.spawnedFurniturePicker.SetActive(true);
        this.materialPicker.SetActive(false);
    }

    public void ShowMaterialPicker()
    {
        this.furniturePicker.SetActive(false);
        this.categoryPicker.SetActive(false);
        this.spawnedFurniturePicker.SetActive(false);
        this.materialPicker.SetActive(true);
    }

    public void AddHighlights()
    {
        //Create Highlight cube around every furniture
        foreach(GameObject g in furnitureList)
        {
            if(g.GetComponent<BoxCollider>() != null)
            {
                GameObject furnitureHighlight = Instantiate(cubeHighlight);
                furnitureHighlight.transform.parent = g.transform;
                furnitureHighlight.transform.localScale = g.GetComponent<BoxCollider>().size;
                furnitureHighlight.transform.localPosition = g.GetComponent<BoxCollider>().center;
                //Debug.Log("name : "+g.name+" trans.rot x : "+ -g.transform.rotation.eulerAngles.x + " trans.rot y : "+ -g.transform.rotation.eulerAngles.y + " trans.rot z : "+ -g.transform.rotation.eulerAngles.z);
                furnitureHighlight.transform.localRotation = Quaternion.identity;
                furnitureHighlight.name = "HighlightCube";
                furnitureHighlight.SetActive(false);
            }
        }
    }
       
    /** Category methods **/

    // Toggle category type
    public bool ToggleCategoryType()
    {
        this.isType = !this.isType;
        this.GenerateCategoryButtons();
        return this.isType;
    }


    public void AddMagnet(Magnet m)
    {
        magnetList.Add(m);
        // Debug.Log("Added " + m.name + " to the magnet list.");
    }

    public void RemoveMagnet(Magnet m)
    {
        magnetList.Remove(m);
        // Debug.Log("Removed " + m.name + " from the magnet list");
    }

    public void NotifyMagnets(GameObject dispawned)
    {
        foreach(Magnet m in magnetList)
        {
            m.UpdateFurnitures(dispawned);
        }
    }

    // Method that stop Magnet from pulling GameObjects.
    // Call this method if you want to stop tidying the room before things go wild.
    public void StopMagnets()
    {
        foreach(Magnet m in magnetList)
        {
            m.Reset();
        } 
    }

    // Set the Magnets as Active and force them to get the GameObject they're going to pull.
    // Call this method if you want to tidy the room.
    public void StartMagnets()
    {
        foreach(Magnet m in magnetList)
        {
            m.Init();
        }
    }


    // Handle category selection
    public void HandleCategorySelect(string category)
    {
        this.currentCategory = category;
        this.FilterFurnitureWithCategory();
        this.GenerateFurnitureButtons();
        this.ShowFurniturePicker();
    }

    // Generate category list buttons 
    public void GenerateCategoryButtons()
    {
        if (this.categoryContentList != null && this.categoryButtonTemplate != null)
        {
            this.DeleteExistingCategoryButtons();
            List<string> categories = this.isType ? this.typeCategories : this.roomCategories;
            foreach (string category in categories)
            {
                GameObject categoryButton = Instantiate(this.categoryButtonTemplate) as GameObject;
                this.categoryButtonList.Add(categoryButton);
                categoryButton.SetActive(true);
                categoryButton.GetComponent<CategoryButton>().SetCategory(category);
                categoryButton.transform.SetParent(this.categoryContentList.transform, false);
            }
        }
    }

    // Delete existing category list buttons
    private void DeleteExistingCategoryButtons()
    {
        foreach (GameObject categoryButton in this.categoryButtonList)
        {
            Destroy(categoryButton.gameObject);
        }
        this.categoryButtonList.Clear();
    }

    /** Furniture methods **/

    public void setAllYFixed(bool yFixed)
    {
        foreach(var g in spawnedFurnitureDict)
        {
            if (g.Value.GetComponent<NotRotating>() != null)
            {
                g.Value.GetComponent<NotRotating>().SetYFixed(yFixed);
            }
        }
    }

    // Handle furniture selection
    public void HandleFurnitureSelect(GameObject furniture)
    {
        if (this.spawnedFurnitureDict.ContainsKey(furniture.name)) { //spawned furniture
            this.DespawnFurniture(furniture);
        } else { //not yet spawned furniture
            this.currentFurniture = furniture;
            this.SpawnFurniture(furniture);
        }
    }

    // Handle Furniture Button Hover
    public void HandleFurnitureHoverIn(GameObject furniture)
    {
        foreach (var f in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (f.name == furniture.name)
            {
                if (f.transform.childCount == 0)
                {
                    Debug.Log("This furniture doesn't have a highlight");
                }
                else
                {
                    GameObject highlightCube = GetChildWithName(f, "HighlightCube");
                    if (highlightCube) highlightCube.SetActive(true);
                }
            }
        }
    }

    public void HandleFurnitureHoverOut(GameObject furniture)
    {
        foreach (var f in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (f.name == furniture.name)
            {
                if (f.transform.childCount == 0)
                {
                    Debug.Log("This furniture doesn't have an highlight");
                }
                else
                {
                    GameObject highlightCube = GetChildWithName(f, "HighlightCube");
                    if (highlightCube) highlightCube.SetActive(false);
                }
            }
        }
    }

    // Populate the furniture list
    private void PopulateFurnitureList()
    {
        // Get children of furniture manager and to list
        foreach (Transform child in transform)
        {
            GameObject furniture = child.gameObject;
            this.furnitureList.Add(furniture);
        }
    }

    // Filter the furniture list according to a current category
    private void FilterFurnitureWithCategory()
    {
        this.filteredFurnitureList.Clear();
        foreach (GameObject furniture in this.furnitureList)
        {
            FurnitureTag furnitureTag = furniture.GetComponent<FurnitureTag>();
            if (furnitureTag)
            {
                if (furnitureTag.HasTag(this.currentCategory))
                    this.filteredFurnitureList.Add(furniture);
            }
            else
            {
                Debug.Log("Furniture tag is undefined");
            }
        }
    }

    // Generate furniture list buttons 
    public void GenerateFurnitureButtons()
    {
        if (this.furnitureContentList != null && this.furnitureButtonTemplate != null)
        {
            this.DeleteExistingFurnitureButtons();
            foreach (GameObject furniture in this.filteredFurnitureList)
            {
                GameObject furnitureButton = Instantiate(this.furnitureButtonTemplate) as GameObject;
                this.furnitureButtonList.Add(furnitureButton);
                furnitureButton.SetActive(true);
                furnitureButton.GetComponent<FurnitureButton>().SetFurniture(furniture);
                furnitureButton.transform.SetParent(this.furnitureContentList.transform, false);
            }
        }
    }

    // Delete existing furniture list buttons
    private void DeleteExistingFurnitureButtons()
    {
        foreach (GameObject furnitureButton in this.furnitureButtonList)
        {
            Destroy(furnitureButton.gameObject);
        }
        this.furnitureButtonList.Clear();
    }

    // Generate spawned furniture list buttons 
    public void GenerateSpawnedFurnitureButtons()
    {
        if (this.spawnedFurnitureContentList != null && this.furnitureButtonTemplate != null)
        {
            this.DeleteExistingSpawnedFurnitureButtons();
            foreach (GameObject furnitureInstance in this.spawnedFurnitureDict.Values)
            {
                GameObject furnitureInstanceButton = Instantiate(this.furnitureButtonTemplate) as GameObject;
                this.spawnedFurnitureButtonList.Add(furnitureInstanceButton);
                furnitureInstanceButton.SetActive(true);
                furnitureInstanceButton.GetComponent<FurnitureButton>().SetFurniture(furnitureInstance);
                furnitureInstanceButton.transform.SetParent(this.spawnedFurnitureContentList.transform, false);
            }
        }
    }

    // Delete existing spawned furniture list buttons
    private void DeleteExistingSpawnedFurnitureButtons()
    {
        foreach (GameObject spawnedFurnitureButton in this.spawnedFurnitureButtonList)
        {
            Destroy(spawnedFurnitureButton.gameObject);
        }
        this.spawnedFurnitureButtonList.Clear();
    }

    // Spawn selected furniture
    private void SpawnFurniture(GameObject furniture)
    {
        GameObject furnitureInstant = Instantiate(furniture) as GameObject;
        string instanceName = this.instanceCount++.ToString() + "-" + furniture.name;
        furnitureInstant.name = instanceName;
        furnitureInstant.SetActive(true);
        furnitureInstant.transform.position = handController.transform.position;
        if (furnitureInstant.GetComponent<BoxCollider>() == null)
        {
            furnitureInstant.AddComponent<BoxCollider>();
        }
        furnitureInstant.AddComponent<Rigidbody>();
        if (furnitureInstant.GetComponent<XRGrabInteractable>() == null)
        {
            furnitureInstant.AddComponent<XRGrabInteractable>();
            furnitureInstant.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
        furnitureInstant.layer = LayerMask.NameToLayer("Grabbables");

        // Add to spawned list
        this.spawnedFurnitureDict.Add(instanceName, furnitureInstant);
        this.GenerateSpawnedFurnitureButtons();
        MagnetizeFurniture(furnitureInstant);
    }

    // Set the correct magnet to the furniture according to its FurnitureTags,
    // and tags the GameObject so that it will be affected by other magnets.
    private void MagnetizeFurniture(GameObject furnitureInstant)
    {
        List<string> tags = furnitureInstant.GetComponent<FurnitureTag>().GetTags() as List<string>;
        
        

        foreach(string tag in tags)
        {
            if(tag == "Table")
            // A Table will pull chairs to it.
            {
                furnitureInstant.tag = "Table";

                Magnet mag = furnitureInstant.AddComponent<Magnet>() as Magnet;
                mag.force = 5;
                mag.isPolarised = true;
                mag.tagPulled = "Chair";
                AddMagnet(mag);

                break;
            }
            else if (tag == "Chair")
            // A chair tries to push away other chairs. (so a negative force is required.)
            {
                furnitureInstant.tag = "Chair";

                Magnet mag = furnitureInstant.AddComponent<Magnet>() as Magnet;
                mag.tagPulled = "Chair";
                mag.force = -5;
                mag.inverseFactor = InverseDenominator.dist_square;
                AddMagnet(mag);

                break;
            }
            else if (tag == "Desk")
            // TODO : this may not have to be the Desk tag.
            {
                furnitureInstant.tag = "Furniture";
                SoloTargetMagnet mag = furnitureInstant.AddComponent<SoloTargetMagnet>() as SoloTargetMagnet;
                mag.tagPulled = "Chair";
                mag.isPolarised = true;
                mag.force = 10;
                mag.torque = 10;
                AddMagnet(mag);

                break;
            }
            else
            // A basic furniture does nothing. It will just be pulled by a magnet.
            {
                furnitureInstant.tag = "Furniture";
            }
        }
    }

    // Put a DirectionMagnet on every wall tagged "Wall"
    // Also magnetize the floor. Yeah, it's kinda like a wall, you know...
    public void MagnetizeWalls()
    {
        foreach(GameObject wall in walls)
        {
            DirectionMagnet mag = wall.AddComponent<DirectionMagnet>() as DirectionMagnet;
            AddMagnet(mag);
        }

        Magnet floorMagnet = Floor.AddComponent<Magnet>() as Magnet;
        floorMagnet.tagPulled = "Table";
        floorMagnet.yToZero = false;
        floorMagnet.isForceConstant = true;
        floorMagnet.force = 25;
        AddMagnet(floorMagnet);
    }

    // Despawn selected furniture instance
    private void DespawnFurniture(GameObject furnitureInstance)
    {
        this.spawnedFurnitureDict.Remove(furnitureInstance.name);

        // Updating MagnetList and magnets' furnitures list.
        Magnet m = furnitureInstance.GetComponent<Magnet>();
        try
        {
            if (m != null)
                RemoveMagnet(m);
            NotifyMagnets(furnitureInstance);
        } 
        catch (NullReferenceException e)
        {
            Debug.Log("Got ya");
        }

        Destroy(furnitureInstance.gameObject);
        this.GenerateSpawnedFurnitureButtons();
    }

    // Move the camera, spawn an ui, listen to the "A" button and launch the script to organise the room.
    public void Organise()
    {
        //Move the camera
        camera.transform.position = new Vector3(0, 4, -1f);
        camera.transform.rotation = Quaternion.identity;

        //Show the instruction and hide the menu
        instruction.SetActive(true);
        canvas.SetActive(false);

        //Disable ui, movements and grab
        inputManager.GetComponent<ShowHideUI>().enabled = false;
        inputManager.GetComponent<EndOrganise>().enabled = true;
        camera.GetComponent<TeleportationProvider>().enabled = false;
        camera.GetComponent<LocomotionSystem>().enabled = false;
        camera.GetComponent<SnapTurnProvider>().enabled = false;
        XRRayInteractor[] rays = camera.GetComponentsInChildren<XRRayInteractor>();
        foreach(XRRayInteractor r in rays)
        {
            r.enabled = false;
        }
        foreach (var g in spawnedFurnitureDict)
        {
            if (g.Value.GetComponent<BoxCollider>() != null)
            {
                if(g.Value.GetComponent<FurnitureTag>() != null)
                {
                    List<string> tags = g.Value.GetComponent<FurnitureTag>().GetTags() as List<string>;
                    if (!tags.Contains("Table"))
                    {
                        g.Value.GetComponent<BoxCollider>().material = noFriction;
                    }
                }
            }
        }
        //Organise the room
        StartMagnets();
    }

    // End the scipt to organise the room, don't listen to A button, despawn the UI and move the camera
    public void EndOrganise()
    {
        //End the organisation of the room
        StopMagnets();
       
        foreach (var g in spawnedFurnitureDict)
        {
            if (g.Value.GetComponent<BoxCollider>() != null)
            {
                g.Value.GetComponent<BoxCollider>().material = null;
            }
        }

        //Hide the instruction
        instruction.SetActive(false);       
        
        //Move the camera
        camera.transform.position = new Vector3(0, 0, 0);
        //camera.transform.Rotate(0, 0, 0, Space.Self);

        //Enable ui, movements and grab
        inputManager.GetComponent<ShowHideUI>().enabled = true;
        inputManager.GetComponent<EndOrganise>().enabled = false;
        camera.GetComponent<TeleportationProvider>().enabled = true;
        camera.GetComponent<LocomotionSystem>().enabled = true;
        camera.GetComponent<SnapTurnProvider>().enabled = true;
        XRRayInteractor[] rays = camera.GetComponentsInChildren<XRRayInteractor>(true);
        foreach (XRRayInteractor r in rays)
        {
            r.enabled = true;
        }
    }

    public SnapshotCamera GetSnapshotCamera() {
        return this.snapshotCamera;
    }

    private void genererMursSol()
    {
        List<Vector3> points = GetBoudariesGuardian();
        if (points == null) return;

        List<Vector2> contours = new List<Vector2>();

        for (int i = 0; i < points.Count; i++)
        {
            contours.Add(new Vector2(points[i].x, points[i].z));
        }

        int min_angle = 10;
        double min_distance = 0.1;
        List<Vector2> corners = findCorners(contours, min_angle, min_distance);

        genererSol(corners);
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

    List<Vector2> findCorners(List<Vector2> contours, int min_angle, double min_distance)
    {
        List<Vector2> ret = new List<Vector2>();
        Vector2 p1, p2, p3;
        double prevAngle = 0;
        double distance = 0;

        int cc = 0;

        for (int i = 0; i < (contours.Count + 3); i++)
        {
            if (i > 2)
            {
                if (i - contours.Count < 0) //Handle the overlapping steps of the loop
                {
                    p1 = contours[i - 2];
                    p2 = contours[i - 1];
                    p3 = contours[i];
                }
                else if (i - contours.Count == 0)
                {
                    p1 = contours[i - 2];
                    p2 = contours[i - 1];
                    p3 = contours[0];
                }
                else if (i - contours.Count == 1)
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
                double currAngle = Math.Atan2(p1.y - p3.y, p1.x - p3.x) * 180 / Math.PI;
                if (currAngle < 0)
                {
                    currAngle = (currAngle * -1);
                }

                if (i > 3)
                {
                    //calculate the difference between this angle and the previous one
                    double diffAngle = Math.Abs(prevAngle - currAngle);

                    //Add point to return array if angle diff is above threshold
                    if (diffAngle > min_angle)
                    {
                        //Ignore points that are closer than "min_distance pixels" to the previous point
                        if (cc > 0)
                        {
                            double dx = ret[cc - 1].x - p1.x;  // no need for if-else or fabs
                            double dy = ret[cc - 1].y - p1.y;
                            distance = Math.Sqrt((dx * dx) + (dy * dy));  // dx*dx will always be positive

                            if (distance >= min_distance)
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
        Debug.Log("Find corners: found " + cc + " corners");

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
                GameObject middleObj = new GameObject();
                Vector3 start = new Vector3(points[i].x, 1.5f, points[i].y);
                Vector3 end = new Vector3(points[i + 1].x, 1.5f, points[i + 1].y);
                Vector3 middle = new Vector3((points[i].x+points[i + 1].x)/2, 1.5f, (points[i].y+points[i + 1].y)/2);
                startObj.transform.localPosition = start;
                endObj.transform.localPosition = end;
                middleObj.transform.localPosition = middle;
                startObj.transform.LookAt(endObj.transform.position);
                endObj.transform.LookAt(startObj.transform.position);

                middleObj.transform.LookAt(endObj.transform.position);
                middleObj.transform.Rotate(0, 90, 0);

                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.tag = "Wall";

                wall.GetComponent<BoxCollider>().material = noFriction;

                // Set material
                Renderer wallRender = wall.GetComponentsInChildren<Renderer>()[0];
                wallRender.material = newMaterialRef;

                // Get distance
                float distance = Vector3.Distance(start, end);

                // Set position
                wall.name = "Wall" + start.x + "," + start.y + "," + end.x + "," + end.y;
                wall.transform.localScale = new Vector3(distance, 3f, 0.01f);
                wall.transform.position = middleObj.transform.position;
                wall.transform.rotation = middleObj.transform.rotation;

                //wall.transform.LookAt(new Vector3(0,0,0));

                Destroy(startObj);
                Destroy(endObj);
                Destroy(middleObj);

                walls.Add(wall);
            }
            else
            {
                GameObject startObj = new GameObject();
                GameObject endObj = new GameObject();
                GameObject middleObj = new GameObject();
                Vector3 start = new Vector3(points[i].x, 1.5f, points[i].y);
                Vector3 end = new Vector3(points[0].x, 1.5f, points[0].y);
                Vector3 middle = new Vector3((points[i].x + points[0].x) / 2, 1.5f, (points[i].y + points[0].y) / 2);
                startObj.transform.localPosition = start;
                endObj.transform.localPosition = end;
                middleObj.transform.localPosition = middle;
                startObj.transform.LookAt(endObj.transform.position);
                endObj.transform.LookAt(startObj.transform.position);

                middleObj.transform.LookAt(endObj.transform.position);
                middleObj.transform.Rotate(0, 90, 0);

                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.tag = "Wall";

                wall.GetComponent<BoxCollider>().material = noFriction;

                // Set color
                Renderer wallRender = wall.GetComponentsInChildren<Renderer>()[0];
                wallRender.material = newMaterialRef;

                // Get distance

                float distance = Vector3.Distance(start, end);
                // Set position
                wall.name = "Wall" + start.x + "," + start.y + "," + end.x + "," + end.y;
                wall.transform.localScale = new Vector3(distance, 3f, 0.01f);
                wall.transform.position = middleObj.transform.position;
                wall.transform.rotation = middleObj.transform.rotation;

                Destroy(startObj);
                Destroy(endObj);
                Destroy(middleObj);

                walls.Add(wall);
            }
        }
    }

    public List<GameObject> GetWalls() {
        return this.walls;
    }

    private void genererSol(List<Vector2> points)
    {
        float XMax = 0;
        float XMin = 0;
        float ZMax = 0;
        float ZMin = 0;

        foreach (Vector2 point in points)
        {
            if(point.x > XMax)
            {
                XMax = point.x;
            }
            if (point.x < XMin)
            {
                XMin = point.x;
            }
            if (point.y > ZMax)
            {
                ZMax = point.y;
            }
            if (point.y < ZMin)
            {
                ZMin = point.y;
            }
        }

        Floor.transform.position = new Vector3((XMax+XMin) / 2, 0, (ZMax + ZMin) / 2);
        Floor.transform.localScale = new Vector3((XMax - XMin) / 10, 1, (ZMax - ZMin) / 10);
    }

    // Start is called before the first frame update
    void Start()
    {
        genererMursSol();
        this.currentCategory = this.defaultCategory;
        this.snapshotCamera = SnapshotCamera.MakeSnapshotCamera("UI");
        this.PopulateFurnitureList();
        this.GenerateCategoryButtons();
        this.AddHighlights();
        MagnetizeWalls();
    }
}