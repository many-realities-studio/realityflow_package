using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using RealityFlow.Plugin.Scripts;
//using RealityFlow.Plugin.Scripts.Events;
using RealityFlow.Plugin.Editor;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Managers;
using Packages.realityflow_package.Runtime.scripts.Messages;

[CustomEditor(typeof(FlowNetworkManager))]
public class FlowNetworkManagerEditor : EditorWindow
{
    // View parameters
    private Rect headerSection;
    private Rect bodySection;
    private Texture2D headerSectionTexture;
    private Texture2D bodySectionTexture;
    private GUISkin skin;
    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    Color bodySectionColor = new Color(150 / 255f, 150 / 255f, 150 / 255f, 1f);
    private string uName;
    private string pWord;
    private EWindowView window = EWindowView.LOGIN;
    private string projectName;
    private string userToInvite;

    private Dictionary<EWindowView, ChangeView> _ViewDictionary = new Dictionary<EWindowView, ChangeView>();
    private delegate void ChangeView();

    enum EWindowView
    {
        LOGIN = 0,
        USER_HUB = 1,
        PROJECT_HUB = 2,
        DELETE_OBJECT = 3,
        LOAD_PROJECT = 4,
        PROJECT_CREATION = 5,
        INVITE_USER = 6,
        PROJECT_IMPORT = 7
    }


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/FlowNetworkManagerEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FlowNetworkManagerEditor window = (FlowNetworkManagerEditor)EditorWindow.GetWindow(typeof(FlowNetworkManagerEditor));
        window.Show();
    }

    /// <summary>
    /// This function is called when this object becomes enabled and active
    /// </summary>
    private void OnEnable()
    {
        Operations.ConnectToServer("ws://echo.websocket.org");

        InitTextures();
        //InitData();
        skin = Resources.Load<GUISkin>("guiStyles/RFSkin");
        _ViewDictionary.Add(EWindowView.LOGIN, _CreateLoginView);
        _ViewDictionary.Add(EWindowView.USER_HUB, _CreateUserHubView);
        _ViewDictionary.Add(EWindowView.PROJECT_HUB, _CreateProjectHubView);
        _ViewDictionary.Add(EWindowView.DELETE_OBJECT, _CreateDeleteObjectView);
        _ViewDictionary.Add(EWindowView.LOAD_PROJECT, _CreateLoadProjectView);
        _ViewDictionary.Add(EWindowView.PROJECT_CREATION, _CreateProjectCreationView);
        _ViewDictionary.Add(EWindowView.INVITE_USER, _CreateInviteUserView); // not implementec
        _ViewDictionary.Add(EWindowView.PROJECT_IMPORT, _CreateProjectImportView);
    }


    public void OnGUI()
    {
        _ViewDictionary[window]();

        if (GUILayout.Button("Create", GUILayout.Height(20)))
        {
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            FlowTObject newObject = new FlowTObject();

            newObject.flowId = 2.ToString();
            NewObjectManager.CreateObjectInUnity(newObject);
        }

        if (GUILayout.Button("Edit", GUILayout.Height(20)))
        {
            if(NewObjectManager.Sphere == null)
            {
                NewObjectManager.Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            FlowTObject newObject = new FlowTObject(NewObjectManager.Sphere);
            newObject.flowId = 2.ToString();

            NewObjectManager.EditObject(newObject);
        }

        ////FlowNetworkManager myTarget = (FlowNetworkManager) target;
        //if (GUILayout.Button("Connect to server"))
        //{
        //    flowWebsocket.Connect("ws://echo.websocket.org");
        //}

        //if (GUILayout.Button("Send message to server"))
        //{
        //    flowWebsocket.SendMessage("Helloooo!");
        //}

        //if(GUILayout.Button("Start coroutine"))
        //{
        //    Debug.Log("Starting coroutine");

        //    Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(flowWebsocket.ReceiveMessage(), flowWebsocket);
        //}

        ////Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(flowWebsocket.ReceiveMessage(), flowWebsocket);

        //if (GUILayout.Button("Create primitive cube"))
        //{
        //    GameObject.CreatePrimitive(PrimitiveType.Cube);
        //}
    }

    public void Update()
    {
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine (Operations.FlowWebsocket.ReceiveMessage(), Operations.FlowWebsocket);
    }

    // Initializes the texture2d values
    private void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        bodySectionTexture = new Texture2D(1, 1);
        bodySectionTexture.SetPixel(0, 0, bodySectionColor);
        bodySectionTexture.Apply();
    }

    // Define the layout of the sections
    private void _DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        //Debug.Log(Screen.width);
        headerSection.height = 50;

        bodySection.x = 0;
        bodySection.y = 50;
        bodySection.width = Screen.width;
        bodySection.height = Screen.height - 50;

        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(bodySection, bodySectionTexture);

    }

    private void _DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Reality Flow", skin.GetStyle("Title"));

        GUILayout.EndArea();
    }

    public void loginCallback(object sender, ConfirmationMessageEventArgs eventArgs)
    {
        Debug.Log("login callback: " + eventArgs.message.WasSuccessful.ToString());
    }

    private void _CreateLoginView()
    {
        // Create UserName entry field
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("User: ");
        uName = EditorGUILayout.TextField(uName);
        EditorGUILayout.EndHorizontal();

        // Create Password entry field
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Password: ");
        pWord = EditorGUILayout.PasswordField(pWord);
        EditorGUILayout.EndHorizontal();

        // Create "Log in" Button and define onClick action
        if (GUILayout.Button("Log in", GUILayout.Height(40)))
        {
            // Send login event to the server
            Operations.Login(uName, pWord, loginCallback);
        }

        // Create "Register" Button and define onClick action

        if (GUILayout.Button("Register", GUILayout.Height(30)))
        {
            GameObject manager = GameObject.FindGameObjectWithTag("ObjManager");

           
        }

        // Create "Import" Button and define onClick action
        if (GUILayout.Button("Import", GUILayout.Height(20)))
        {
            // Send the user to the project import window
            window = EWindowView.PROJECT_IMPORT;
        }
    }

    private void _CreateUserHubView()
    {
        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("Logout", GUILayout.Height(20)))
        {
            // Send logout event to the server

            // Set logged (global) state to false
            //loggedIn = false;

            // Clear the user ID
            Config.userId = "-9999";

            // Send the user back to the login screen
            window = EWindowView.LOGIN;
        }

        // Create "New Project" Button and define onClick action
        if (GUILayout.Button("New Project", GUILayout.Height(40)))
        {
            // Send the user to the project creation screen
            window = EWindowView.PROJECT_CREATION;
        }

        // Create "Load Project" Button and define onClick action
        if (GUILayout.Button("Load Project", GUILayout.Height(40)))
        {
            // Send the user to the load project screen
            window = EWindowView.LOAD_PROJECT;
        }
    }

    private void _CreateProjectHubView()
    {
        // Create "Exit Project" Button and define onClick action
        if (GUILayout.Button("Exit Project", GUILayout.Height(20)))
        {
            ExitProject(); // Deletes all local instances of the objects and restores the projectID to default value

            // Send the user to the User Hub screen
            window = EWindowView.USER_HUB;
        }

        // Create "Create new object" Button and define onClick action
        if (GUILayout.Button("Create new Object", GUILayout.Height(40)))
        {
            // Opens a new window/unity utility tab to create an object
            ObjectSettings.OpenWindow();
        }

        // Create "Delete an object" Button and define onClick action
        if (GUILayout.Button("Delete an Object", GUILayout.Height(40)))
        {
            // Send user to Delete Object screen
            window = EWindowView.DELETE_OBJECT;
        }

        // Create "Export" Button and define onClick action
        if (GUILayout.Button("Export", GUILayout.Height(20)))
        {
            ConfirmationWindow.OpenWindow();
        }
    }

    private void _CreateDeleteObjectView()
    {
        GameObject objectManagerGameObject = GameObject.FindGameObjectWithTag("ObjManager");

        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the Project Hub screen
            window = EWindowView.PROJECT_HUB;
        }
        EditorGUILayout.EndHorizontal();


        if (objectManagerGameObject != null)
        {
            // TODO: Replace this with new code to get all flowObjects
            List<GameObject> gameObjectsList = objectManagerGameObject.GetComponent<ObjectManager>().GetFlowObjects();

            // List out all game objects in the scene
            foreach (GameObject gameObject in gameObjectsList)
            {
                // Add a button whose name is the name of the game object and define the onClick event
                if (GUILayout.Button(gameObject.name, GUILayout.Height(30)))
                {
                    DeleteObject(gameObject);
                }
            }
        }
    }

    private void _CreateLoadProjectView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the User Hub screen
            window = EWindowView.USER_HUB;
        }
        EditorGUILayout.EndHorizontal();

        // List all of the available projects
        if (Config.projectList != null)
        {
            foreach (FlowProject project in Config.projectList)
            {
                // Add a button whose name is the name of the project and define the onClick event
                if (GUILayout.Button(project.ProjectName, GUILayout.Height(30)))
                {
                    //// Set the current project to the selected project
                    //Config.projectId = project._id;
                    //FlowProject.activeProject.projectName = project.projectName;

                    //// Send a request to the server to fetch the desired project
                    //ProjectFetchEvent fetch = new ProjectFetchEvent();
                    //fetch.Send(); /// TODO: Refactor to be more explicit about which project is getting fetched

                    // Send the user to the Project Hub screen
                    window = EWindowView.PROJECT_HUB;
                }
            }
        }
    }

    private void _CreateProjectCreationView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user back to the User Hub
            window = EWindowView.USER_HUB;
        }
        EditorGUILayout.EndHorizontal();

        // Add "Project Name" label
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Project Name: ");
        projectName = EditorGUILayout.TextField(projectName);
        EditorGUILayout.EndHorizontal();

        // Make the user enter a project name if they did not enter one
        if (projectName == null || projectName.Equals(""))
        {
            EditorGUILayout.HelpBox("Enter a name for the project", MessageType.Warning);
        }
        else
        {
            // Create "Create Project" Button and define onClick action
            if (GUILayout.Button("Create Project", GUILayout.Height(40)))
            {
                //// Send ProjectCreate event to the server
                //ProjectCreateEvent create = new ProjectCreateEvent();
                ////create.Send(projectName);

                // Send the user to the Project Hub screen
                window = EWindowView.PROJECT_HUB;
                projectName = ""; // TODO: What does this do?
            }
        }
    }

    private void _CreateInviteUserView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send user to the Project Hub Screen
            window = EWindowView.PROJECT_HUB;
        }
        EditorGUILayout.EndHorizontal();

        // Add Username label to the current screen
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Username: ");
        userToInvite = EditorGUILayout.TextField(userToInvite);
        EditorGUILayout.EndHorizontal();

        // Ensure that there exists a user to invite
        if (userToInvite == null || userToInvite.Equals(""))
        {
            EditorGUILayout.HelpBox("Enter the username of the person to invite", MessageType.Warning);
        }
        else
        {
            // Create "Invite" Button and define onClick action
            if (GUILayout.Button("Invite", GUILayout.Height(40)))
            {
                //// Send Project Invite event to the server
                //ProjectInviteEvent invite = new ProjectInviteEvent();
                //invite.send(userToInvite);

                // Send the user to the Project Hub screen
                window = EWindowView.PROJECT_HUB;
            }
        }
    }

    private void ExitProject()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends a Delete event to the server to delete the desired game object
    /// </summary>
    /// <param name="gameObjectToBeDeleted"></param>
    public void DeleteObject(GameObject gameObjectToBeDeleted)
    {
        //ObjectDeleteEvent delete = new ObjectDeleteEvent();
        //delete.Send(gameObjectToBeDeleted.GetComponent<FlowObject>().ft._id);
    }

    private void _CreateProjectImportView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send user to the Login screen
            window = EWindowView.LOGIN;
        }
        EditorGUILayout.EndHorizontal();

        // TODO: Change the following from reading a text file to reading what is sent 
        //          from the server

        // Specify the path of where the project is saved
        string path = "Assets/resources/projects/test.txt";

        StreamReader reader = new StreamReader(path);
        List<string> jsonList = new List<string>();

        // Read each line of the file and add it to the json list
        while (reader.Peek() != -1)
        {
            jsonList.Add(reader.ReadLine());
        }
        reader.Close();

        // 
        //foreach (string json in jsonList)
        //{
        //    //TODO: What does this do?
        //    //ProjectFetchEvent log = new ProjectFetchEvent();
        //    JsonUtility.FromJsonOverwrite(json, log);
        //    string date = new System.DateTime(log.timestamp).ToString();

        //    // Create Project Button Button and define onClick action
        //    if (GUILayout.Button(log.project.projectName + "   " + date, GUILayout.Height(30)))
        //    {
        //        // gather the list of objects in the previous imported project
        //        GameObject[] gameObjectList = GameObject.FindGameObjectsWithTag("imported");

        //        // clear any previous imported objects
        //        foreach (GameObject gameObject in gameObjectList)
        //        {
        //            DestroyImmediate(gameObject);
        //        }

        //        // TODO: refactor name of log.objs to be more clear as to what that field is
        //        foreach (FlowTObject currentFlowTObject in log.objs)
        //        {
        //            // this piece of code doesn't have textures implemented
        //            // for an example on how to add the textures look at the commented
        //            // code in the SaveObjectData function

        //            Debug.Log("creating object: " + currentFlowTObject.name);
        //            GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //            // Create the mesh of the object
        //            Mesh objMesh = newObj.GetComponent<MeshFilter>().mesh;
        //            objMesh.vertices = currentFlowTObject.vertices;
        //            objMesh.uv = currentFlowTObject.uv;
        //            objMesh.triangles = currentFlowTObject.triangles;
        //            objMesh.RecalculateBounds();
        //            objMesh.RecalculateNormals();

        //            // Set the position and orientation of the object
        //            newObj.transform.localPosition = new Vector3(currentFlowTObject.x, currentFlowTObject.y, currentFlowTObject.z);
        //            newObj.transform.localRotation = Quaternion.Euler(new Vector4(currentFlowTObject.q_x, currentFlowTObject.q_y, currentFlowTObject.q_z, currentFlowTObject.q_w));
        //            newObj.transform.localScale = new Vector3(currentFlowTObject.s_x, currentFlowTObject.s_y, currentFlowTObject.s_z);

        //            // Destory the collider associated with the object (this is from the cube primitive
        //            MonoBehaviour.DestroyImmediate(newObj.GetComponent<Collider>(), true);

        //            newObj.AddComponent<BoxCollider>(); // Why do we destroy the collider just to add it back in?

        //            newObj.name = currentFlowTObject.name;
        //            newObj.tag = "imported";

        //            // Set the color of the object
        //            Material mat = newObj.GetComponent<MeshRenderer>().material;
        //            mat.color = currentFlowTObject.color;
        //        }

        //        // Send the user to the login screen
        //        window = EWindowView.LOGIN;
        //    }
        //}
    }
}
