﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class RaycastTest : MonoBehaviour {
    /*
     * Declaring all variables to collect and store stats for each object touched. 
     */
    Vector3 touchPosWorld;
	public Text myText;
	//Change me to change the touch phase used.
	TouchPhase touchPhase = TouchPhase.Began;
    public GameObject[] cubes;
    private int j;
    private int count;
    private float timetillNow;
    public int[] countperCube;
    public bool[] shadeperCube;
    public float[] timeTaken;
    bool is_fill;
    bool right_failed;
    public bool is_shader_changed = false;
    Shader[] temp_shader;
    int which_j;
    public int[] displaySide;
    public Vector3[] cubesize;
    public Vector3[] whereiscube;
    bool is_count_changed;
    GameObject particle;
    void Start()
    {
        j =0;
        is_count_changed = false;
        is_fill = false;
        right_failed = false;
        particle = GameObject.Find("point");
        particle.SetActive(false);
	/*
	 * Cubes were the game objects chosen for the task, cubes appear in random order at strategically chosen locations.
	 * All the cubes in unity are set with tag "cube" for grouping them into a Game Object array.
	 */
        cubes =  GameObject.FindGameObjectsWithTag("cube");
        countperCube = new int[cubes.Length];
        shadeperCube = new bool[cubes.Length];
        timeTaken = new float[cubes.Length];
        displaySide = new int[cubes.Length];
        cubesize = new Vector3[cubes.Length];
        whereiscube = new Vector3[cubes.Length];
        displaySide[0] = cubes[0].layer;
        whereiscube[0] = cubes[0].transform.position;
        countperCube[0] = 0;
        temp_shader = new Shader[cubes.Length];
        MeshRenderer cuberend1 = cubes[0].GetComponent<MeshRenderer>();
        temp_shader[0] = cuberend1.material.shader;
        cubesize[0] = cuberend1.bounds.size;
        shadeperCube[0] = false;
	/*
	 * At any time only one cube is shown on the screen, to prevent confusion that increases selection time.
	 * At the start, all cube objects except the first one are set to false, and all counters are reset to 0.
	 */
        for (int i = 1; i < cubes.Length; i++)
        {
            MeshRenderer cuberend = cubes[i].GetComponent<MeshRenderer>();
            temp_shader[i] = cuberend.material.shader;
            cubesize[i] = cuberend.bounds.size;
            displaySide[i] = cubes[i].layer;
            whereiscube[i] = cubes[i].transform.position;
            cubes[i].SetActive(false);
            countperCube[i] = 0;
            shadeperCube[i] = false;
        }
        timetillNow = 0;
        count = 0;
    }
    void Update() {
	    for (var i = 0; i < Input.touchCount; ++i) {
		    is_fill = false;
		    right_failed = false;
		    /*
		     * The idea is to provide feedback by changing color of the cube when user's finger is mvoving over the cube.
		     * So we do the operation when touch phase is Moved. 
		     */
		    if (Input.GetTouch(i).phase == TouchPhase.Moved) {
			    if (!is_count_changed)
			    {
				    count += 1;
				    is_count_changed = true;
			    }
			    timetillNow += Time.deltaTime;
			    // Construct a ray from the current touch coordinates
			    Camera camLeft = GameObject.Find("LeftEye").GetComponent<Camera>();
			    Ray rayLeft = camLeft.ScreenPointToRay(Input.GetTouch(i).position);
			    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
			    // Create a particle if hit
			    RaycastHit hit;
			    if (Physics.Raycast (ray, out hit)) { 
				    //You can print this to understand how raycast from Camera works in VR
				    //myText.text = " moved Hit Right";
			    }
			    else
			    {
				    right_failed = true;
			    }
			    if (right_failed)
			    {
				    if (Physics.Raycast(rayLeft, out hit))
				    {
					    //                      myText.text = " moved Hit left";
				    }
			    }
			    if (hit.collider != null){
				    /*
				     * When touch is in moving phase, if object is hit, then change object's color.
				     * Keep track of whose color changed. In touch-ended phase, if this object is not selected,
				     * we change the color back to cube's color.
				     */
				    GameObject touchedObject = hit.transform.gameObject;
				    if (touchedObject.transform.name == cubes[j].name){
					    MeshRenderer cuberend = cubes[j].GetComponent<MeshRenderer>();
					    Shader cubemat = Shader.Find("high");
					    cuberend.material.shader = cubemat;
					    shadeperCube[j] = true;
					    cubes[j].layer = 0;
					    //count = count + 1;
					    //myText.text = "I changed color";
				    }
				    else
				    {
					    particle.transform.position = hit.point;
					    particle.SetActive(true);
				    }
			    }
		    }
		    if (Input.GetTouch(i).phase == TouchPhase.Ended)
		    {
			    /*
			     * Once touch has ended, i.e. finger is lifted, we see if cube is hit.
			     */
				    
			    count += 1;
			    timetillNow += Time.deltaTime;
			    // Construct a ray from the current touch coordinates
			    Camera camLeft = GameObject.Find("LeftEye").GetComponent<Camera>();
			    Ray rayLeft = camLeft.ScreenPointToRay(Input.GetTouch(i).position);
			    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
			    // Create a particle if hit
			    RaycastHit hit;
			    if (Physics.Raycast(ray, out hit))
			    {
				    //myText.text = "Hit Right";
			    }
			    else
			    {
				    right_failed = true;
			    }
			    if (right_failed)
			    {
				    if (Physics.Raycast(rayLeft, out hit))
				    {
					    //myText.text = "Hit left";
				    }
			    }
			    if (hit.collider != null)
			    {
				    /*
				     * Object is hit, check if it is the cube / reference plain.
				     */
				    GameObject touchedObject = hit.transform.gameObject;
				    //myText.text += ": " + touchedObject.transform.name;
				    if (touchedObject.transform.name == cubes[j].name)
				    {
					    /*
					     * If correct cube is hit, we set visibility to false, and turn the next cube on.
					     * Keep doing this till last cube.
					     * On last cube, save the stats. 
					     */
					    touchedObject.SetActive(false);
					    if ((j >= cubes.Length - 1) && (!cubes[cubes.Length - 1].activeSelf))
					    {
						    myText.text = "Thanks!";
						    countperCube[j] = count - 1;
						    timeTaken[j] = timetillNow;
						    SaveInfo();
					    }
					    else
					    {
						    countperCube[j] = count - 1;
						    timeTaken[j] = timetillNow;
						    cubes[j + 1].SetActive(true);
						    j++;
						    count = 0;
						    timetillNow = 0;
						    is_count_changed = false;
						    particle.SetActive(false);
					    }
				    }
				    else
				    {
					    if (shadeperCube[j])
					    {
						    if (cubes[j].activeSelf)
						    {
							    /*
							     * Reset the Render for cube, from previous highlight, 
							     * as selection wasn't successful.
							     */
							    MeshRenderer cuberend = cubes[j].GetComponent<MeshRenderer>();
							    cuberend.material.shader = temp_shader[j];
						    }
					    }
					    particle.transform.position = hit.point;
					    particle.SetActive(true);
				    }
			    }
			    else
			    {
				    if (shadeperCube[j])
				    {
					    if (cubes[j].activeSelf)
					    {
						    /*
						     * Reset the Render for cube, from previous highlight, 
						     * as selection wasn't successful.
						     */
						    MeshRenderer cuberend = cubes[j].GetComponent<MeshRenderer>();
						    cuberend.material.shader = temp_shader[j];
					    }
				    }
			    }
		    }
	    }
    }
    void SaveInfo()
    {
        // Users array will be created in future.
        string user = "homido";
        string filePath = getPath(user);

        //This is the writer, it writes to the filepath
        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine("no. of touches, time taken, cube size,,,display side,where is cube");
        //This loops through everything in the inventory and sets the file to these.
        for (int i = 0; i < cubes.Length; i++)
        {
            writer.WriteLine(countperCube[i].ToString() +
                "," + timeTaken[i].ToString()+ ","+cubesize[i].ToString()+
                ","+displaySide[i].ToString()+","+whereiscube[i]);
        }
        writer.Flush();
        //This closes the file
        writer.Close();
    }

    private string getPath(string user)
    {
        return Application.persistentDataPath+"Saved_Info_"+ user+".csv";
    }
}
