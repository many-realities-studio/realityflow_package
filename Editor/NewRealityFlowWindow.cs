using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using RealityFlow.Plugin.Scripts;
using RealityFlow.Plugin.Editor;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using System.Collections;
using Packages.realityflow_package.Runtime.scripts.Structures;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using Behaviours;

[CustomEditor(typeof(FlowWebsocket))]
public class FlowNetworkManagerEditor : EditorWindow
{
    //private const string Url = "ws://localhost:8999/";
    private string _Url = "ws://plato.mrl.ai:8999";
    //private const string Url = "ws://68e6e63b.ngrok.io";

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

    private IList<FlowProject> _ProjectList = null;

    //FlowTObject newObject = null;// = new FlowTObject(new Color(0, 0, 0, 0), "TestFlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "TestObject");
    //FlowUser newUser = null;// = new FlowUser("testUsername", "TestPassword");

    public string[] objectOptions;
    public ArrayList objectNames = new ArrayList();
    public ArrayList objectIds = new ArrayList();
    public Boolean addingChain = false;
    public int selectedTrigger = 0;
    public int selectedTarget = 0;
    public FlowBehaviour headBehaviour = null;

   // public string previousBehaviourId = null;
    public Boolean showAllOptions = false;


    enum EWindowView
    {
        LOGIN = 0,
        USER_HUB = 1,
        PROJECT_HUB = 2,
        DELETE_OBJECT = 3,
        LOAD_PROJECT = 4,
        PROJECT_CREATION = 5,
        INVITE_USER = 6,
        PROJECT_IMPORT = 7,
        CREATE_BEHAVIOUR = 8,
        CREATE_TELEPORT = 9,
        CREATE_CLICK = 10,
        CREATE_SNAPZONE = 11,
        CREATE_ENABLE = 12,
        CREATE_DISABLE = 13
    }


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/FlowNetworkManagerEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FlowNetworkManagerEditor window = (FlowNetworkManagerEditor)EditorWindow.GetWindow(typeof(FlowNetworkManagerEditor));
        window.Show();
    }
    //FlowTObject testObject;
    //FlowUser testUser;
    //FlowProject testProject;

    /// <summary>
    /// This function is called when this object becomes enabled and active
    /// </summary>
    private void OnEnable()
    {
        //if(testObject == null)
        //{
        //    testObject = new FlowTObject(GUID.Generate().ToString(), 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, "name");
        //}
        //if (testUser == null)
        //{
        //     testUser = new FlowUser("user", "pass"); ;
        //}
        //if(testProject == null)
        //{
        //    testProject = new FlowProject("flowId", "description", 0, "projectName"/*, new FlowTObject[]
        //    {
        //         testObject
        //    }*/);
        //}

        //if(newObject == null)
        //    newObject = new FlowTObject(GUID.Generate().ToString(), 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, "name");
        //if (newUser == null)
        //    newUser = new FlowUser("testUsername", "TestPassword");

        //Operations.ConnectToServer("ws://echo.websocket.org");
        //Operations.ConnectToServer(Url);

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
        _ViewDictionary.Add(EWindowView.CREATE_BEHAVIOUR, _CreateBehaviourView);
        _ViewDictionary.Add(EWindowView.CREATE_TELEPORT, _CreateTeleportView);
        _ViewDictionary.Add(EWindowView.CREATE_CLICK, _CreateClickView);
        _ViewDictionary.Add(EWindowView.CREATE_ENABLE, _CreateEnableView);
        _ViewDictionary.Add(EWindowView.CREATE_DISABLE, _CreateDisableView);


    }

    public void OnGUI()
    {
        _ViewDictionary[window]();

        //if (GUILayout.Button("Fetch Projects"))
        //{
        //    Operations.GetAllUserProjects(ConfigurationSingleton.CurrentUser, (_, e) =>
        //    {
        //        _ProjectList = e.message.Projects;
        //        Debug.Log("Project list from fetch projects button: " + (_ProjectList == null ? "null" : _ProjectList.ToString()));

        //        foreach (FlowProject f in _ProjectList)
        //        {
        //            Debug.Log(f.ProjectName);
        //        }
        //    });
        //}

        //if (GUILayout.Button("Create", GUILayout.Height(20)))
        //{
        //    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        //    //if (newUser == null)
        //    //{
        //    //    newUser = new FlowUser("user", "pass");
        //    //}

        //    #region User messages

        //    // Login
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(Login_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new Login_SendToServer(testUser)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(LoginUser_Received).ToString() + ".json", MessageSerializer.ConvertToString(new LoginUser_Received(true)));

        //    //// Register / CreateUser
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(RegisterUser_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new RegisterUser_SendToServer(testUser)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(RegisterUser_Received).ToString() + ".json", MessageSerializer.ConvertToString(new RegisterUser_Received(true)));

        //    //// Logout
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(Logout_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new Logout_SendToServer(testUser)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(LogoutUser_Received).ToString() + ".json", MessageSerializer.ConvertToString(new LogoutUser_Received(true)));

        //    #endregion User messages

        //    #region Object messages

        //    //// Create Object
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(CreateObject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new CreateObject_SendToServer(testObject, /*testUser,*/ testProject.Id)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(CreateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new CreateObject_Received(testObject)));

        //    //// Delete Object
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(DeleteObject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteObject_SendToServer(testProject.Id, testObject.Id)));

        //    ////System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(DeleteObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteObject_Received()));

        //    //// Update Object
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(UpdateObject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new UpdateObject_SendToServer(testObject, /*testUser,*/ testProject.Id)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(UpdateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new UpdateObject_Received(testObject)));

        //    //// Finalized update object
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(FinalizedUpdateObject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new FinalizedUpdateObject_SendToServer(testObject, testProject.Id)));

        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(FinalizedUpdateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new FinalizedUpdateObject_Received(testObject)));

        //    #endregion Object messages

        //    #region Project messages
        //    //// Create project
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(CreateProject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new CreateProject_SendToServer(testProject, testUser)));
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(CreateProject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new CreateProject_Received(testProject, true)));

        //    //// Delete project
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(DeleteProject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteProject_SendToServer(testProject, testUser)));

        //    ////System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(DeleteObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteObject_Received()));

        //    //// Fetch project list
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(GetAllUserProjects_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new GetAllUserProjects_SendToServer(testUser)));
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(GetAllUserProjects_Received).ToString() + ".json", MessageSerializer.ConvertToString(new GetAllUserProjects_Received(new List<FlowProject>() { testProject })));

        //    //// Open project
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(OpenProject_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new OpenProject_SendToServer(testProject.Id, testUser)));
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(OpenProject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new OpenProject_Received(testProject)));

        //    #endregion Project messages

        //    //// Room messages
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\RoomMessages\" + typeof(JoinRoom_SendToServer).ToString() + ".json", MessageSerializer.ConvertToString(new JoinRoom_SendToServer(testProject.Id, testUser)));
        //    //System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\RoomMessages\" + typeof(JoinRoom_Received).ToString() + ".json", MessageSerializer.ConvertToString(new JoinRoom_Received(testProject)));

        //    //    //Operations.CreateObject(newObject, newUser, "TestProjectId", CreateObjectCallbackTest);

        //    //    //UpdateObject_SendToServer updateObject = new UpdateObject_SendToServer(newObject, newUser, "ProjectId");
        //    //    //Operations.FlowWebsocket.SendMessage(updateObject);

        //    //    //Operations.DeleteObject("TestProjectId", DeleteObjectCallback);


        //    //    //FinalizedUpdateObject_SendToServer finalizedUpdateObject = new FinalizedUpdateObject_SendToServer(newObject, newUser, "ProjectId");
        //    //    //Operations.FlowWebsocket.SendMessage(finalizedUpdateObject);
        //    //}

        //    // if (GUILayout.Button("Edit", GUILayout.Height(20)))
        //    // {
        //    //     if(NewObjectManager.Sphere == null)
        //    //     {
        //    //         NewObjectManager.Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    //     }
        //    //     FlowTObject newObject = new FlowTObject(NewObjectManager.Sphere);
        //    //     newObject.flowId = 2.ToString();

        //    //     NewObjectManager.EditObject(newObject);
        //    // }

        //    ////FlowNetworkManager myTarget = (FlowNetworkManager) target;
        //    //if (GUILayout.Button("Connect to server"))
        //    //{
        //    //    flowWebsocket.Connect("ws://echo.websocket.org");
        //    //}

        //    //if (GUILayout.Button("Send message to server"))
        //    //{
        //    //    flowWebsocket.SendMessage("Helloooo!");
        //    //}

        //    //if(GUILayout.Button("Start coroutine"))
        //    //{
        //    //    Debug.Log("Starting coroutine");

        //    //    Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(flowWebsocket.ReceiveMessage(), flowWebsocket);
        //    //}

        //    ////Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(flowWebsocket.ReceiveMessage(), flowWebsocket);

        //    //if (GUILayout.Button("Create primitive cube"))
        //    //{
        //    //    GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //}
        //}
    }

    private void DeleteObjectCallback(object sender, DeleteObjectMessageEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    private void CreateObjectCallbackTest(object sender, CreateObjectMessageEventArgs eventArgs)
    {
        Debug.Log("Final: " + eventArgs.message.ToString());
    }

    private void OnDestroy()
    {
        Operations.Logout(ConfigurationSingleton.CurrentUser);

        FlowTObject.RemoveAllObjectsFromScene();
        ConfigurationSingleton.CurrentProject = null;
        ConfigurationSingleton.CurrentUser = null;

    }

    public void Update()
    {
        if (FlowWebsocket.websocket != null)
        {
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(Operations._FlowWebsocket.ReceiveMessage(), Operations._FlowWebsocket);
        }

        //foreach(string flowObjectId in FlowTObject.idToGameObjectMapping.Keys)
        //{
        //    FlowTObject currentObject = FlowTObject.idToGameObjectMapping[flowObjectId];

        //    // If the current object is selected
        //    if (Selection.Contains(currentObject.AttachedGameObject))
        //    {
        //        FlowObject_Monobehaviour currentObjectMonobehaviour = currentObject.AttachedGameObject.GetComponent(typeof(FlowObject_Monobehaviour)) as FlowObject_Monobehaviour;

        //        bool currentIsCheckedOut = currentObjectMonobehaviour.checkedOut;

        //        if(currentIsCheckedOut == true)
        //        {

        //        }
        //    }
        //}
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

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Server URL: ");
        _Url = EditorGUILayout.TextField(_Url);
        EditorGUILayout.EndHorizontal();

        // Create "Log in" Button and define onClick action
        if (GUILayout.Button("Log in", GUILayout.Height(40)))
        {
            // Send login event to the server
            ConfigurationSingleton.CurrentUser = new FlowUser(uName, pWord);
            Operations.Login(ConfigurationSingleton.CurrentUser, _Url, (_, e) =>
            {
                Debug.Log("login callback: " + e.message.WasSuccessful.ToString());
                if (e.message.WasSuccessful == true)
                {
                    Operations.GetAllUserProjects(ConfigurationSingleton.CurrentUser, (__, _e) =>
                    {
                        _ProjectList = _e.message.Projects;
                        Debug.Log("Project list = " + _ProjectList);
                        window = EWindowView.USER_HUB;
                    });
                }
                else
                {
                    ConfigurationSingleton.CurrentUser = null;
                    // TODO: Display that login failed
                }
            });
        }

        // Create "Register" Button and define onClick action

        if (GUILayout.Button("Register", GUILayout.Height(30)))
        {
            Operations.Register(uName, pWord, _Url, (sender, e) => { Debug.Log(e.message); });
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

        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("Logout", GUILayout.Height(20)))
        {
            // Send logout event to the server
            if (ConfigurationSingleton.CurrentUser != null)
            {
                Operations.Logout(ConfigurationSingleton.CurrentUser);
                window = EWindowView.LOGIN;
            }

            // Send the user back to the login screen
            //window = EWindowView.LOGIN;
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

        if (GUILayout.Button("Add Interaction", GUILayout.Height(40)))
        {
            BehaviourEventManager.PreviousBehaviourId = null;

            objectNames.Clear();
            objectIds.Clear();

            Dictionary<string, FlowTObject>.ValueCollection  values = FlowTObject.idToGameObjectMapping.Values;

            foreach(FlowTObject obj in values)
            {
                objectNames.Add(obj.Name);
                objectIds.Add(obj.Id);
            }

            objectOptions = (string[])objectNames.ToArray(typeof(string));


            // Send user to Create Behaviour Screen
            addingChain = false;
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        // Create "Export" Button and define onClick action
        if (GUILayout.Button("Export", GUILayout.Height(20)))
        {
            ConfirmationWindow.OpenWindow();
        }
    }

    private void _CreateDeleteObjectView()
    {
        //GameObject objectManagerGameObject = GameObject.FindGameObjectWithTag("ObjManager");

        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the Project Hub screen
            window = EWindowView.PROJECT_HUB;
        }
        EditorGUILayout.EndHorizontal();

        foreach(FlowTObject currentFlowObject in FlowTObject.idToGameObjectMapping.Values)
        {
            if(GUILayout.Button(currentFlowObject.Name, GUILayout.Height(30)))
            {
                //TODO: make sure this deletes the object
                Operations.DeleteObject(currentFlowObject.Id, ConfigurationSingleton.CurrentProject.Id, (_, e) => { Debug.Log("Deleted Object " + e.message); });
            }
        }

        //if (objectManagerGameObject != null)
        //{
        //    // TODO: Replace this with new code to get all flowObjects
        //    List<GameObject> gameObjectsList = objectManagerGameObject.GetComponent<ObjectManager>().GetFlowObjects();

        //    // List out all game objects in the scene
        //    foreach (GameObject gameObject in gameObjectsList)
        //    {
        //        // Add a button whose name is the name of the game object and define the onClick event
        //        if (GUILayout.Button(gameObject.name, GUILayout.Height(30)))
        //        {
        //            DeleteObject(gameObject);
        //        }
        //    }
        //}
    }

    private bool _RefreshProjectList = true;

    private void _CreateLoadProjectView()
    {
        //if(_RefreshProjectList == true)
        //{
        //    Operations.GetAllUserProjects(ConfigurationSingleton.CurrentUser, (_, e) =>
        //    {
        //        _ProjectList = e.message.ProjectList;
        //        Debug.Log(e.message);
        //    });

        //    _RefreshProjectList = false;
        //}

        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the User Hub screen
            window = EWindowView.USER_HUB;
        }
        EditorGUILayout.EndHorizontal();

        if (_ProjectList != null)
        {
            foreach (FlowProject project in _ProjectList)
            {
                if (GUILayout.Button(project.ProjectName, GUILayout.Height(30)))
                {
                    Operations.OpenProject(project.Id, ConfigurationSingleton.CurrentUser, (_, e) =>
                    {
                        Debug.Log(e.message);
                        if(e.message.WasSuccessful == true)
                        {
                            ConfigurationSingleton.CurrentProject = e.message.flowProject;
                            window = EWindowView.PROJECT_HUB;
                        }
                    });
                }
            }
        }

        // List all of the available projects
        //if (Config.projectList != null)
        //{
        //    foreach (FlowProject project in Config.projectList)
        //    {
        //        // Add a button whose name is the name of the project and define the onClick event
        //        if (GUILayout.Button(project.ProjectName, GUILayout.Height(30)))
        //        {
        //            //// Set the current project to the selected project
        //            //Config.projectId = project._id;
        //            //FlowProject.activeProject.projectName = project.projectName;

        //            //// Send a request to the server to fetch the desired project
        //            //ProjectFetchEvent fetch = new ProjectFetchEvent();
        //            //fetch.Send(); /// TODO: Refactor to be more explicit about which project is getting fetched

        //            // Send the user to the Project Hub screen
        //            window = EWindowView.PROJECT_HUB;
        //        }
        //    }
        //}
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
                ConfigurationSingleton.CurrentProject = new FlowProject("GUID", "description", 0, projectName/*, new List<FlowTObject>()*/);
                // TODO: Generate guid
                Operations.CreateProject(ConfigurationSingleton.CurrentProject, ConfigurationSingleton.CurrentUser, (_, e) =>
                {
                    if (e.message.WasSuccessful == true)
                    {
                        // TODO: overwrite current project with new received project info
                        window = EWindowView.PROJECT_HUB;
                        Debug.Log(e.message);
                    }
                    else
                    {
                        ConfigurationSingleton.CurrentProject = null;
                        // TODO: display to the user that create project failed
                    }
                });
                //// Send ProjectCreate event to the server
                //ProjectCreateEvent create = new ProjectCreateEvent();
                ////create.Send(projectName);

                // Send the user to the Project Hub screen
                //window = EWindowView.PROJECT_HUB;
                //projectName = ""; // TODO: What does this do?
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
        Operations.LeaveProject(ConfigurationSingleton.CurrentProject.Id, ConfigurationSingleton.CurrentUser, (_, e) =>
        {
            if(e.message.WasSuccessful == true)
            {
                ConfigurationSingleton.CurrentProject = null;
                FlowTObject.RemoveAllObjectsFromScene();
            }

            else
            {
                Debug.LogWarning("Error leaving project.");
            }
        });
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
    }


    private void _CreateBehaviourView()
    {
        Debug.Log("All the current Behaviours: ");

        foreach(FlowBehaviour fb in BehaviourEventManager.BehaviourList.Values)
        {
            Debug.Log(fb.BehaviourName + " " + fb.TargetObjectId + " " + fb.TriggerObjectId + " " + fb.Id + "\n");
        }

        Debug.Log("All the objects in the mapping: ");
        foreach(FlowTObject t in FlowTObject.idToGameObjectMapping.Values)
        {
            Debug.Log(t.Name + "\n");
        }



        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
           // headBehaviour = null;
            addingChain = false;
            showAllOptions = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Choose one of the triggers below.");
        EditorGUILayout.BeginHorizontal();

        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("Click", GUILayout.Height(75), GUILayout.Width(92)))
        {
            window = EWindowView.CREATE_CLICK;
        }

        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("On Collision", GUILayout.Height(75), GUILayout.Width(92)))
        {
            //window = EWindowView.CREATE_CLICK;
        }

        
        EditorGUILayout.EndHorizontal();

        if (addingChain)
        {


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Teleport", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_TELEPORT;
            }
     
            if (GUILayout.Button("Snapzone", GUILayout.Height(75), GUILayout.Width(92)))
            {

            }
            if (GUILayout.Button("Enable", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_ENABLE;
            }

            if (GUILayout.Button("Disable", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_DISABLE;
            }

            EditorGUILayout.EndHorizontal();
        }
    }


    private void _CreateTeleportView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Teleport");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "TriggerObject:", selectedTrigger, objectOptions);
        selectedTarget = EditorGUI.Popup(new Rect(0, 150, position.width, 30), "TargetObject:", selectedTarget, objectOptions);

        GUILayout.Space(120f);
        GUILayout.Label("Add another interaction?");
        GUILayout.BeginHorizontal();
   
        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObjectId = objectIds[selectedTrigger].ToString();
            string secondObjectId = objectIds[selectedTarget].ToString();

            GameObject firstObject = FlowTObject.idToGameObjectMapping[firstObjectId].AttachedGameObject;

            addingChain = true;

            TeleportAction teleportAction = new TeleportAction(new TeleportCoordinates(firstObject, false));

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Teleport";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObjectId, secondObjectId, flowAction);
            AddBehaviour(fb);

            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();
            string secondObject = objectIds[selectedTarget].ToString();
            
            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Teleport";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, secondObject, flowAction);
            AddBehaviour(fb);

            addingChain = false;
            showAllOptions = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void _CreateClickView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("On Click");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "Object trigger:", selectedTrigger, objectOptions);

        GUILayout.Space(120f);

        GUILayout.Label("Press OK to setup the event that will take place when the above object is clicked.");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("OK", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Click", id, firstObject, firstObject, null);
            AddBehaviour(fb);

            addingChain = true;
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void _CreateEnableView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Enable Object");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "Object to Enable:", selectedTrigger, objectOptions);

        GUILayout.Space(120f);

        GUILayout.Label("Add another Interaction?");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Enable";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);

            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Enable";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);

            addingChain = false;
            showAllOptions = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void _CreateDisableView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Disable Object");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "Object to Disable:", selectedTrigger, objectOptions);

        GUILayout.Space(120f);
        GUILayout.Label("Add another Interaction?");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();
            addingChain = true;

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Disable";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);

            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();
            addingChain = false;

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Disable";

            string id = Guid.NewGuid().ToString();
            
            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);
            
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }



    /// <summary>
    /// If not adding on to the behaviour chain, then sends a CreateBehaviour request to server.
    /// If adding on to the behaviour chain, then adds the flowBehaviour to the end of the current chain
    /// </summary>
    /// <param name="flowbehaviour"></param>
    private void AddBehaviour(FlowBehaviour newFlowBehaviour)
    {
        // Create the list of behaviours that need to add newFlowBehaviour to their chain 
        List<string> behavioursToLinkTo = new List<string>();
        if(BehaviourEventManager.PreviousBehaviourId != null)
        {
            behavioursToLinkTo.Add(BehaviourEventManager.PreviousBehaviourId);
        }

        // Create the new behaviour
        Operations.CreateBehaviour(newFlowBehaviour, ConfigurationSingleton.CurrentProject.Id, behavioursToLinkTo, (sender, e) =>
        {
            if(e.message.WasSuccessful == true)
            {
                // Update the Previous behaviour Id
                BehaviourEventManager.PreviousBehaviourId = e.message.flowBehaviour.Id;
            }

            else
            {
                Debug.LogWarning("Failed to create behaviour");
            }
        });
    }
}

