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
using System.Globalization;

[CustomEditor(typeof(FlowWebsocket))]
public class FlowNetworkManagerEditor : EditorWindow
{
    //private const string Url = "ws://localhost:8999/";
    private string _Url = "ws://plato.mrl.ai:8999";
    private const string Url = "ws://a73c9fa8.ngrok.io";

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

    private string defaultZero = "0";
    private string positionX;
    private string positionY;
    private string positionZ;

    private string rotationX;
    private string rotationY;
    private string rotationZ;

    private string scaleX;
    private string scaleY;
    private string scaleZ;



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
        CREATE_DISABLE = 13,
        DELETE_BEHAVIOUR = 14
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
        EditorApplication.playModeStateChanged += _PlayModeStateHasChanged;
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
        _ViewDictionary.Add(EWindowView.CREATE_SNAPZONE, _CreateSnapZoneView);
        _ViewDictionary.Add(EWindowView.DELETE_BEHAVIOUR, _DeleteBehaviourView);


    }

    public void OnGUI()
    {
        _ViewDictionary[window]();

        if (GUILayout.Button("Check BEM"))
        {
            //Debug.Log(BehaviourEventManager.BehaviourList);
        }

        //if (GUILayout.Button("Fetch Projects"))
        //{
        //    Operations.GetAllUserProjects(ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
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

    private void OnDestroy()
    {
        Operations.Logout(ConfigurationSingleton.SingleInstance.CurrentUser);

        FlowTObject.RemoveAllObjectsFromScene();
        ConfigurationSingleton.SingleInstance.CurrentProject = null;
        ConfigurationSingleton.SingleInstance.CurrentUser = null;

    }

    private void _PlayModeStateHasChanged(PlayModeStateChange state)
    {
        ConfigurationSingleton.SingleInstance = AssetDatabase.LoadAssetAtPath<ConfigurationSingleton>("Assets/RealityFlowConfiguration.asset");

        FlowUser currentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
        FlowProject currentProject = ConfigurationSingleton.SingleInstance.CurrentProject;

        if(state == PlayModeStateChange.EnteredEditMode 
            || state == PlayModeStateChange.EnteredPlayMode)
        {
            Operations.Login(currentUser, _Url, (_, e) =>
            {
                if (e.message.WasSuccessful)
                {
                    Operations.OpenProject(currentProject.Id, currentUser, (__, ee) =>
                    {
                        if (ee.message.WasSuccessful)
                        {
                            Debug.Log("Successfully logged in user after switching play mode state!");
                        }
                    });
                }
            });
        }
        else if (state == PlayModeStateChange.ExitingEditMode
            || state == PlayModeStateChange.ExitingPlayMode)
        {
            // Check in all objects
            foreach (FlowTObject obj in FlowTObject.idToGameObjectMapping.Values)
            {
                if (obj.CanBeModified == true)
                {
                    Operations.CheckinObject(obj.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
                }
            }

            Operations.Logout(ConfigurationSingleton.SingleInstance.CurrentUser);
        }
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
            ConfigurationSingleton.SingleInstance.CurrentUser = new FlowUser(uName, pWord);
            Operations.Login(ConfigurationSingleton.SingleInstance.CurrentUser, _Url, (_, e) =>
            {
                Debug.Log("login callback: " + e.message.WasSuccessful.ToString());
                if (e.message.WasSuccessful == true)
                {
                    Operations.GetAllUserProjects(ConfigurationSingleton.SingleInstance.CurrentUser, (__, _e) =>
                    {
                        _ProjectList = _e.message.Projects;
                        Debug.Log("Project list = " + _ProjectList);
                        window = EWindowView.USER_HUB;
                    });
                }
                else
                {
                    ConfigurationSingleton.SingleInstance.CurrentUser = null;
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
            if (ConfigurationSingleton.SingleInstance.CurrentUser != null)
            {
                Operations.Logout(ConfigurationSingleton.SingleInstance.CurrentUser);
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

        EditorGUILayout.BeginHorizontal();

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


        if (GUILayout.Button("Delete Interaction", GUILayout.Height(40)))
        {
            window = EWindowView.DELETE_BEHAVIOUR;
               
        }

        EditorGUILayout.EndHorizontal();

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
                Operations.DeleteObject(currentFlowObject.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { Debug.Log("Deleted Object " + e.message); });
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
        //    Operations.GetAllUserProjects(ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
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
                    Operations.OpenProject(project.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
                    {
                        Debug.Log(e.message);
                        if(e.message.WasSuccessful == true)
                        {
                            ConfigurationSingleton.SingleInstance.CurrentProject = e.message.flowProject;
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
                ConfigurationSingleton.SingleInstance.CurrentProject = new FlowProject("GUID", "description", 0, projectName/*, new List<FlowTObject>()*/);
                // TODO: Generate guid
                Operations.CreateProject(ConfigurationSingleton.SingleInstance.CurrentProject, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
                {
                    if (e.message.WasSuccessful == true)
                    {
                        // TODO: overwrite current project with new received project info
                        window = EWindowView.PROJECT_HUB;
                        Debug.Log(e.message);
                    }
                    else
                    {
                        ConfigurationSingleton.SingleInstance.CurrentProject = null;
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
        Operations.LeaveProject(ConfigurationSingleton.SingleInstance.CurrentProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
        {
            if(e.message.WasSuccessful == true)
            {
                ConfigurationSingleton.SingleInstance.CurrentProject = null;
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


    public string CreateBehaviourTitle(FlowBehaviour fb)
    {
        // Find the object names and trigger types to display nicely on the Delete Interaction UI
        string triggerObjectIdName = FlowTObject.idToGameObjectMapping[fb.TriggerObjectId].Name;
        Boolean hasNextBehaviours = (fb.NextBehaviour.Count > 0);
        string chainBehaviour = (hasNextBehaviours ? BehaviourEventManager.BehaviourList[fb.NextBehaviour[0]].BehaviourName : "no chain");
        string nextBehaviourObjectName;

        // Determine what the object name for the chained behaviour is
        if (hasNextBehaviours)
        {
            string nextBehaviourId = fb.NextBehaviour[0];
            string nextBehaviourTriggerObjectId = BehaviourEventManager.BehaviourList[nextBehaviourId].TriggerObjectId;
            nextBehaviourObjectName = FlowTObject.idToGameObjectMapping[nextBehaviourTriggerObjectId].Name;
        }
        else
        {
            nextBehaviourObjectName = "no object";
        }
        // Create the string to display on the button detailing the behaviour
        string behaviourTitle = "Type of Trigger: " + fb.TypeOfTrigger +
                                "\t\tChain behaviour: " + chainBehaviour +
                                "\nOn Object: " + triggerObjectIdName +
                                "\t\tOn Object: " + nextBehaviourObjectName;

        return behaviourTitle;

    }

    private void _DeleteBehaviourView()
    {
        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.Space(10f);

        EditorGUILayout.LabelField("Delete Interaction");
        EditorGUILayout.LabelField("Choose an interaction to delete by pressing its button.");

        GUILayout.Space(20f);

        // Grab all the current flowBehaviours in the behaviour list and create buttons 
        foreach (FlowBehaviour currentFlowBehaviour in BehaviourEventManager.BehaviourList.Values)
        {
            // only grab the click behaviours and delete its chain since it's the only trigger we have setup
            if (currentFlowBehaviour.TypeOfTrigger.Equals("Click"))
            {

                string behaviourTitle = CreateBehaviourTitle(currentFlowBehaviour);


                // Create the button to delete the interaction
                if (GUILayout.Button(behaviourTitle, GUILayout.Height(70)))
                {
                    // Make the List that will keep all the behaviour ids in the chain so we can send to server to delete
                    List<string> deleteBehaviourIds = new List<string>();

                    // Make a reference so we can traverse down the chain
                    FlowBehaviour head = currentFlowBehaviour;
                    deleteBehaviourIds.Add(currentFlowBehaviour.Id);

                    // Add the behaviour's chain Id's
                    while (head.NextBehaviour != null && head.NextBehaviour.Count > 0)
                    {
                        // this should be doing BFS and adding each id into the delete
                        // but for now ui is setup to only have one behaviour in a behaviour's 
                        // nextbehaviour array, so for now just add the first one
                        deleteBehaviourIds.Add(head.NextBehaviour[0]);

                        // make the nextbeheviour the new head and iterate through its chain
                        head = BehaviourEventManager.BehaviourList[head.NextBehaviour[0]];
                    }

                    Operations.DeleteBehaviour(deleteBehaviourIds, ConfigurationSingleton.CurrentProject.Id, (_, e) =>
                    {
                        if (e.message.WasSuccessful)
                        {
                            Debug.Log("Successfully deleted behaviours ");
                            foreach (string s in e.message.BehaviourIds)
                            {
                                Debug.Log(s);
                            }
                        }
                        else
                        {
                            Debug.Log("Unable to delete the behaviours");
                        }

                    });

                    // Change window back to Project hub
                    window = EWindowView.PROJECT_HUB;

                }
            }
        }
    }



    private void _CreateBehaviourView()
    {
        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
           // headBehaviour = null;
            addingChain = false;
            showAllOptions = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.Space(10f);
        string optionType = (addingChain == true ? "actions" : "triggers");
        EditorGUILayout.LabelField("Choose one of the " + optionType + " below.");
        EditorGUILayout.BeginHorizontal();

        if (!addingChain)
        {
            // Create "Logout" Button and define onClick action
            if (GUILayout.Button("Click", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_CLICK;
            }

            //// Create "Logout" Button and define onClick action
            //if (GUILayout.Button("On Collision", GUILayout.Height(75), GUILayout.Width(92)))
            //{
            //    //window = EWindowView.CREATE_CLICK;
            //}
        }
        
        EditorGUILayout.EndHorizontal();

        //if (/*addingChain*/)
        if(addingChain)
        {


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Teleport", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_TELEPORT;
            }
     
            if (GUILayout.Button("Snapzone", GUILayout.Height(75), GUILayout.Width(92)))
            {
                window = EWindowView.CREATE_SNAPZONE;
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

    private void CreatePositionScaleRotation()
    {

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
        //GUILayout.Space(30f);

        EditorGUILayout.LabelField("Select the object that will teleport.");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "TriggerObject:", selectedTrigger, objectOptions);
       // selectedTarget = EditorGUI.Popup(new Rect(0, 150, position.width, 30), "TargetObject:", selectedTarget, objectOptions);
        GUILayout.Space(30f);

        CreateTransformUI("teleport");


        GUILayout.Space(120f);
        GUILayout.Label("Add another interaction?");
        GUILayout.BeginHorizontal();
   
        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObjectId = objectIds[selectedTrigger].ToString();
            //string secondObjectId = objectIds[selectedTarget].ToString();

           // GameObject firstObject = FlowTObject.idToGameObjectMapping[firstObjectId].AttachedGameObject;

            addingChain = true;

            CreateTeleportCoordinates(firstObjectId, false, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ);

            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            CreateTeleportCoordinates(firstObject, false, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ);

            addingChain = false;
            showAllOptions = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Converts each 
    /// </summary>
    /// <param name="firstObjectId"></param>
    /// <param name="isSnapZone"></param>
    /// <param name="posX">X position</param>
    /// <param name="posY">Y position</param>
    /// <param name="posZ">Z position</param>
    /// <param name="rotX">X rotation</param>
    /// <param name="rotY">Y rotation</param>
    /// <param name="rotZ">Z rotation</param>
    /// <param name="scaleX">X scale</param>
    /// <param name="scaleY">Y Scale</param>
    /// <param name="scaleZ">Z Scale</param>
    public void CreateTeleportCoordinates(string firstObjectId, Boolean isSnapZone, string posX, string posY, string posZ, string rotX, string rotY, string rotZ, string scaleX, string scaleY, string scaleZ)
    {
        // Convert position coordinates into a Vector3
        Vector3 positionCoords = new Vector3(float.Parse(posX, CultureInfo.InvariantCulture.NumberFormat),
                                                 float.Parse(posY, CultureInfo.InvariantCulture.NumberFormat),
                                                 float.Parse(posZ, CultureInfo.InvariantCulture.NumberFormat));

        // Convert rotation coordinates into a Quaternion
        Quaternion rotationCoords = new Quaternion(float.Parse(rotX, CultureInfo.InvariantCulture.NumberFormat),
                                                   float.Parse(rotY, CultureInfo.InvariantCulture.NumberFormat),
                                                   float.Parse(rotZ, CultureInfo.InvariantCulture.NumberFormat),
                                                   1f);

        // Convert scale coordinates into a Vector3
        Vector3 scaleCoords = new Vector3(float.Parse(scaleX, CultureInfo.InvariantCulture.NumberFormat),
                                          float.Parse(scaleY, CultureInfo.InvariantCulture.NumberFormat),
                                          float.Parse(scaleZ, CultureInfo.InvariantCulture.NumberFormat));


        // Create the TeleportCoordinates and pass it into the constructor for TeleportAction
        TeleportCoordinates teleportCoordinates = new TeleportCoordinates(positionCoords, rotationCoords, scaleCoords, isSnapZone);
        TeleportAction teleportAction = new TeleportAction(teleportCoordinates);

        string id = Guid.NewGuid().ToString();

        FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObjectId, firstObjectId, teleportAction);
        AddBehaviour(fb);
    }


    public void CreateTransformUI(string behaviourType)
    {

        // Setup the coordinates that the object will teleport to
        if (behaviourType.Equals("teleport"))
        {
            EditorGUILayout.LabelField("Enter the position, scale, and rotation (in floats) that the object will " + behaviourType + " to.");
        }
            

        if (behaviourType.Equals("snap"))
        {
            EditorGUILayout.LabelField("Enter the position, scale, and rotation relative to the zone object that the snapping object will " + behaviourType + " into.");
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label("Position X");
        //string positionX;
        //EditorGUILayout.TextField()
        positionX = EditorGUILayout.TextField(positionX);

        GUILayout.Label("Position Y");
        positionY = EditorGUILayout.TextField(positionY);

        GUILayout.Label("Position Z");
        positionZ = EditorGUILayout.TextField(positionZ);

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        GUILayout.Label("Rotation X");
        rotationX = EditorGUILayout.TextField(rotationX);

        GUILayout.Label("Rotation Y");
        rotationY = EditorGUILayout.TextField(rotationY);

        GUILayout.Label("Rotation Z");
        rotationZ = EditorGUILayout.TextField(rotationZ);

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        GUILayout.Label("Scale X");
        scaleX = EditorGUILayout.TextField(scaleX);

        GUILayout.Label("Scale Y");
        scaleY = EditorGUILayout.TextField(scaleY);

        GUILayout.Label("Scale Z");
        scaleZ = EditorGUILayout.TextField(scaleZ);

        GUILayout.EndHorizontal();

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

            FlowBehaviour fb = new FlowBehaviour("Click", id, firstObject, firstObject, new FlowAction(true));
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

            addingChain = true;
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
            

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Disable";

            string id = Guid.NewGuid().ToString();

            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);

            addingChain = true;
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            FlowAction flowAction = new FlowAction();
            flowAction.ActionType = "Disable";

            string id = Guid.NewGuid().ToString();
            
            FlowBehaviour fb = new FlowBehaviour("Immediate", id, firstObject, firstObject, flowAction);
            AddBehaviour(fb);

            addingChain = false;
            window = EWindowView.PROJECT_HUB;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void _CreateSnapZoneView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Snap Zone");
        //GUILayout.Space(30f);

        EditorGUILayout.LabelField("Select the snap zone object. Then select the object that will snap into the snap zone object.");
        GUILayout.Space(30f);

        selectedTarget = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "Snap Zone Object:", selectedTarget, objectOptions);
        selectedTrigger = EditorGUI.Popup(new Rect(0, 130, position.width, 30), "Snapping Object:", selectedTrigger, objectOptions);
        
        GUILayout.Space(80f);

        CreateTransformUI("snap");


        GUILayout.Space(100f);
        GUILayout.Label("Add another interaction?");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObjectId = objectIds[selectedTrigger].ToString();

            CreateTeleportCoordinates(firstObjectId, true, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ);

            addingChain = true;
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObject = objectIds[selectedTrigger].ToString();

            CreateTeleportCoordinates(firstObject, true, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, scaleX, scaleY, scaleZ);

            addingChain = false;
            showAllOptions = false;
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
        Debug.Log("About to add behaviour");
        // Create the list of behaviours that need to add newFlowBehaviour to their chain 
        List<string> behavioursToLinkTo = new List<string>();
        if(BehaviourEventManager.PreviousBehaviourId != null)
        {
            behavioursToLinkTo.Add(BehaviourEventManager.PreviousBehaviourId);
        }

        // Create the new behaviour
        Operations.CreateBehaviour(newFlowBehaviour, ConfigurationSingleton.SingleInstance.CurrentProject.Id, behavioursToLinkTo, (sender, e) =>
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

