using Behaviours;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using RealityFlow.Plugin.Editor;
using RealityFlow.Plugin.Scripts;
using System;
using System.Linq;
//using System.Web.Security;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

using Newtonsoft.Json; // TODO: TEMPORARY
using GraphProcessor;

[CustomEditor(typeof(FlowWebsocket))]
public class FlowNetworkManagerEditor : EditorWindow
{
    //private string _Url = "ws://plato.mrl.ai:8999";
    private string _Url = "ws://localhost:8999";
    private const string Url = "ws://a73c9fa8.ngrok.io";

    // View parameters
    private Rect headerSection;

    private Rect bodySection;
    private Texture2D headerSectionTexture;
    private Texture2D bodySectionTexture;
    private GUISkin skin;
    private Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    private Color bodySectionColor = new Color(150 / 255f, 150 / 255f, 150 / 255f, 1f);
    private string uName;
    private string graphName;
    private string pWord;
    private string tempUName;
    private string tempPWord;
    private EWindowView window = EWindowView.LOGIN;
    private string projectName;
    private string userToInvite;
    private bool userIsGuest = false;

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

    public string[] objectOptions;
    public ArrayList objectNames = new ArrayList();
    public ArrayList objectIds = new ArrayList();
    public Boolean addingChain = false;
    public int selectedTrigger = 0;
    public int selectedTarget = 0;
    public FlowBehaviour headBehaviour = null;

    public Boolean showAllOptions = false;

    private enum EWindowView
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
        DELETE_BEHAVIOUR = 14,
        DELETE_PROJECT_CONFIRMATION = 15,
        DELETE_VSGRAPH = 16,
        GUESTUSER_HUB = 17,
        GUESTPROJECT_HUB = 18,
        GUEST_CONFIRM_LOGIN = 19
    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Reality Flow")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        FlowNetworkManagerEditor window = (FlowNetworkManagerEditor)EditorWindow.GetWindow(typeof(FlowNetworkManagerEditor), false, "Reality Flow");
        window.Show();
    }

    /// <summary>
    /// This function is called when this object becomes enabled and active
    /// </summary>
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += _PlayModeStateHasChanged;

        InitTextures();
        skin = Resources.Load<GUISkin>("guiStyles/RFSkin");

        _ViewDictionary.Add(EWindowView.LOGIN, _CreateLoginView);
        _ViewDictionary.Add(EWindowView.USER_HUB, _CreateUserHubView);
        _ViewDictionary.Add(EWindowView.PROJECT_HUB, _CreateProjectHubView);
        _ViewDictionary.Add(EWindowView.DELETE_OBJECT, _CreateDeleteObjectView);
        _ViewDictionary.Add(EWindowView.LOAD_PROJECT, _CreateLoadProjectView);
        _ViewDictionary.Add(EWindowView.DELETE_PROJECT_CONFIRMATION, _CreateDeleteProjectConfirmationView);
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
        _ViewDictionary.Add(EWindowView.DELETE_VSGRAPH, _CreateDeleteVSGraphView);
        _ViewDictionary.Add(EWindowView.GUESTUSER_HUB, _CreateGuestUserHubView);
        _ViewDictionary.Add(EWindowView.GUESTPROJECT_HUB, _CreateGuestProjectHubView);
        _ViewDictionary.Add(EWindowView.GUEST_CONFIRM_LOGIN, _CreateGuestConfirmLoginView);
    }

    public void OnGUI()
    {
        _ViewDictionary[window]();
    }

    private void OnDestroy()
    {
        // Check in all objects
        foreach (FlowTObject obj in FlowTObject.idToGameObjectMapping.Values)
        {
            if (obj.CanBeModified == true)
            {
                Operations.CheckinObject(obj.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
            }
        }

        // Check in all graphs
        foreach (FlowVSGraph graph in FlowVSGraph.idToVSGraphMapping.Values)
        {
            if (graph.CanBeModified == true)
            {
                Operations.CheckinVSGraph(graph.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
            }
        }

        if (userIsGuest)
        {
            Operations.DeleteUser(new FlowUser(tempUName, tempPWord));
        }
        else
        {
            Operations.Logout(ConfigurationSingleton.SingleInstance.CurrentUser);
        }

        GameObject[] wbList;
        wbList = GameObject.FindGameObjectsWithTag("Canvas");

        foreach (GameObject wb in wbList)
        {
            // wb.transform.GetChild(2).gameObject.GetComponent<RealityFlowGraphView>().ClearGraph;
            GameObject runeTimeGraphObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(2).gameObject;
            RealityFlowGraphView rfgv = runeTimeGraphObject.GetComponent<RealityFlowGraphView>();
            rfgv.ClearWhiteBoard();
        }

        FlowTObject.RemoveAllObjectsFromScene();
        FlowVSGraph.RemoveAllGraphsFromScene();
        ConfigurationSingleton.SingleInstance.CurrentProject = null;
        ConfigurationSingleton.SingleInstance.CurrentUser = null;
    }

    private void _PlayModeStateHasChanged(PlayModeStateChange state)
    {
        ConfigurationSingleton.SingleInstance = AssetDatabase.LoadAssetAtPath<ConfigurationSingleton>("Assets/RealityFlowConfiguration.asset");

        FlowUser currentUser = ConfigurationSingleton.SingleInstance.CurrentUser;
        FlowProject currentProject = ConfigurationSingleton.SingleInstance.CurrentProject;

        if (state == PlayModeStateChange.EnteredEditMode
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

            // Check in all graphs
            foreach (FlowVSGraph graph in FlowVSGraph.idToVSGraphMapping.Values)
            {
                if (graph.CanBeModified == true)
                {
                    Operations.CheckinVSGraph(graph.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
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
                }
            });
        }

        // Create "Register" Button and define onClick action
        if (GUILayout.Button("Register", GUILayout.Height(30)))
        {
            Operations.Register(uName, pWord, _Url, (sender, e) => { Debug.Log(e.message); });
        }

        // Create "Log in as guest user" Button and define onClick action
        if (GUILayout.Button("Log in as guest user", GUILayout.Height(30)))
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var uNameString = new char[8];
            var pWordString = new char[8];

            var rand = new System.Random();
            
            for (int i = 0; i < uNameString.Length; i++)
            {
                uNameString[i] = chars[rand.Next(chars.Length)];
                pWordString[i] = chars[rand.Next(chars.Length)];
            }

            tempUName = ("Guest-" + new string(uNameString));
            tempPWord = (new string(pWordString));
            //tempPWord = GeneratePassword(8, 0);

            Debug.Log("Generated username: " + tempUName);
            Debug.Log("Generated password: " + tempPWord);

            Operations.Register(tempUName, tempPWord, _Url, (sender, e) => { Debug.Log(e.message); });

            userIsGuest = true;
            window = EWindowView.GUEST_CONFIRM_LOGIN;
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

        // // Create "Delete Project" Button and define onClick action
        // if (GUILayout.Button("Delete Project", GUILayout.Height(40)))
        // {
        //     // Send the user to the load project screen
        //     window = EWindowView.DELETE_PROJECT;
        // }

        EditorGUILayout.BeginHorizontal();

        openProjectId = EditorGUILayout.TextField(openProjectId);
        if (GUILayout.Button("Open"))
        {
            Operations.OpenProject(openProjectId, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
            {
                if (e.message.WasSuccessful == true)
                {
                    Debug.Log(e.message);
                    if (e.message.WasSuccessful == true)
                    {
                        ConfigurationSingleton.SingleInstance.CurrentProject = e.message.flowProject;
                        window = EWindowView.PROJECT_HUB;
                    }
                }
            });
        }
        EditorGUILayout.EndHorizontal();

        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("Logout", GUILayout.Height(20)))
        {
            // Send logout event to the server
            if (ConfigurationSingleton.SingleInstance.CurrentUser != null)
            {
                Operations.Logout(ConfigurationSingleton.SingleInstance.CurrentUser);
                window = EWindowView.LOGIN;
            }
        }
    }

    private void _CreateGuestConfirmLoginView()
    {
        GUILayout.Label("A guest account has been generated. Please proceed to the guest hub.");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Continue", GUILayout.Height(20)))
        {
            // Send the user to the Guest User Hub screen by logging them into their guest account
            ConfigurationSingleton.SingleInstance.CurrentUser = new FlowUser(tempUName, tempPWord);
            Operations.Login(ConfigurationSingleton.SingleInstance.CurrentUser, _Url, (_, e) =>
            {
                Debug.Log("login callback: " + e.message.WasSuccessful.ToString());
                if (e.message.WasSuccessful == true)
                {
                    Operations.GetAllUserProjects(ConfigurationSingleton.SingleInstance.CurrentUser, (__, _e) =>
                    {
                        // Project list should be empty, not sure if this is necessary.
                        _ProjectList = _e.message.Projects;
                        window = EWindowView.GUESTUSER_HUB;
                    });
                }
                else
                {
                    //Operations.DeleteUser(new FlowUser(tempUName, tempPWord));
                    ConfigurationSingleton.SingleInstance.CurrentUser = null;
                }
            });
        }
        EditorGUILayout.EndHorizontal();
    }

    private void _CreateGuestUserHubView()
    {
        EditorGUILayout.BeginHorizontal();

        openProjectId = EditorGUILayout.TextField(openProjectId);
        if (GUILayout.Button("Join project"))
        {
            Operations.OpenProject(openProjectId, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
            {
                if (e.message.WasSuccessful == true)
                {
                    Debug.Log(e.message);
                    if (e.message.WasSuccessful == true)
                    {
                        ConfigurationSingleton.SingleInstance.CurrentProject = e.message.flowProject;
                        window = EWindowView.GUESTPROJECT_HUB;
                    }
                }
            });
        }
        EditorGUILayout.EndHorizontal();

        // Create "Logout" Button and define onClick action
        if (GUILayout.Button("Logout", GUILayout.Height(20)))
        {
            // Send logout event to the server
            if (ConfigurationSingleton.SingleInstance.CurrentUser != null)
            {
                // TODO: Logic for deleting user after logout
                //FlowUser toDelete = ConfigurationSingleton.SingleInstance.CurrentUser;
                Operations.DeleteUser(ConfigurationSingleton.SingleInstance.CurrentUser);

                userIsGuest = false;
                window = EWindowView.LOGIN;
            }
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

            Dictionary<string, FlowTObject>.ValueCollection values = FlowTObject.idToGameObjectMapping.Values;

            foreach (FlowTObject obj in values)
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

        EditorGUILayout.BeginHorizontal();

        // Create "Create Visual Scripting Graph" Button and define onClick action
        if (GUILayout.Button("Create Visual Scripting Graph", GUILayout.Height(40)))
        {
            VSGraphSettings.OpenWindow();;
        }

        // Create "Delete Visual Scripting Graph" Button and define onClick action
        if (GUILayout.Button("Delete Visual Scripting Graph", GUILayout.Height(40)))
        {
            // TODO: Send user to screen that displays all graphs in the project for them to delete from.
            window = EWindowView.DELETE_VSGRAPH;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.TextArea("Project code: " + ConfigurationSingleton.SingleInstance.CurrentProject.Id);

        // Create "Delete This Project" Button and define onClick action
        if (GUILayout.Button("Delete This Project", GUILayout.Height(40)))
        {
            // Send user to Delete Project screen
            window = EWindowView.DELETE_PROJECT_CONFIRMATION;
        }

        // Button to print graphs for debug
        if (GUILayout.Button("Print all graphs in the idToVSGraphMapping dictionary", GUILayout.Height(40)))
        {
            foreach (FlowVSGraph graph in FlowVSGraph.idToVSGraphMapping.Values)
            {
                Debug.Log("Using JsonUtility: " + JsonUtility.ToJson(graph));
                try
                {
                    string NewtonGraph = JsonConvert.SerializeObject(graph, new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    Debug.Log("Using NewtonSoft: " + NewtonGraph);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                Debug.Log("serializedNodes as list:");
                foreach (var serializedNode in graph.serializedNodes.ToList())
                {
                    Debug.Log(serializedNode);
                }
                Debug.Log("Edges as list:");
                foreach (var edge in graph.edges.ToList())
			    {
                    Debug.Log(edge);
                }
                Debug.Log("ExposedParameters list:");
                foreach (var exparam in graph.exposedParameters.ToList())
			    {
                    Debug.Log(exparam);
                }

                Debug.Log("ExposedParameters as themselves serialized");
                foreach (ExposedParameter exparam in graph.exposedParameters)
                {
                    Debug.Log("serialized value of exposed parameter: " + exparam.serializedValue.value.ToString());
                    try
                    {
                        // string NewtonExparam = JsonConvert.SerializeObject(exparam, new JsonSerializerSettings()
                        // {
                        //     PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        // });
                        // Debug.Log("Newton serialized exposed parameter: " + NewtonExparam);
                        Debug.Log("JsonUtility serialized version of exposed parameter: " + JsonUtility.ToJson(exparam));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }
    }

    private void _CreateGuestProjectHubView()
    {
        // Create "Exit Project" Button and define onClick action
        if (GUILayout.Button("Exit Project", GUILayout.Height(20)))
        {
            ExitProject(); // Deletes all local instances of the objects and restores the projectID to default value

            // Send the user to the User Hub screen
            window = EWindowView.GUESTUSER_HUB;
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

            Dictionary<string, FlowTObject>.ValueCollection values = FlowTObject.idToGameObjectMapping.Values;

            foreach (FlowTObject obj in values)
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

        EditorGUILayout.BeginHorizontal();

        // Create "Create Visual Scripting Graph" Button and define onClick action
        if (GUILayout.Button("Create Visual Scripting Graph", GUILayout.Height(40)))
        {
            VSGraphSettings.OpenWindow();;
        }

        // Create "Delete Visual Scripting Graph" Button and define onClick action
        if (GUILayout.Button("Delete Visual Scripting Graph", GUILayout.Height(40)))
        {
            // TODO: Send user to screen that displays all graphs in the project for them to delete from.
            window = EWindowView.DELETE_VSGRAPH;
        }

        EditorGUILayout.EndHorizontal();

        // As user is a guest, I feel like they shouldn't be able to get a code to invite other users without owner's consent
        // EditorGUILayout.TextArea("Project code: " + ConfigurationSingleton.SingleInstance.CurrentProject.Id);

        // Create "Delete This Project" Button and define onClick action
        // if (GUILayout.Button("Delete This Project", GUILayout.Height(40)))
        // {
        //     // Send user to Delete Project screen
        //     window = EWindowView.DELETE_PROJECT_CONFIRMATION;
        // }

        // Graph Message testing button to open testing window
        if (GUILayout.Button("GRAPH MESSAGE TESTING", GUILayout.Height(40)))
        {
            // Opens a new window/unity utility tab to create an object
            GraphMessageTesting.OpenWindow();
        }
    }

    private void _CreateDeleteObjectView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the Project Hub screen
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
        }
        EditorGUILayout.EndHorizontal();

        foreach (FlowTObject currentFlowObject in FlowTObject.idToGameObjectMapping.Values)
        {
            if (GUILayout.Button(currentFlowObject.Name, GUILayout.Height(30)))
            {
                Operations.DeleteObject(currentFlowObject.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { Debug.Log("Deleted Object " + e.message); });
            }
        }
    }

    private bool _RefreshProjectList = true;
    private bool displayProjectCode = false;
    private string openProjectId;

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

        if (_ProjectList != null)
        {
            foreach (FlowProject project in _ProjectList)
            {
                if (GUILayout.Button(project.ProjectName, GUILayout.Height(30)))
                {
                    Operations.OpenProject(project.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
                    {
                        Debug.Log(e.message);
                        if (e.message.WasSuccessful == true)
                        {
                            ConfigurationSingleton.SingleInstance.CurrentProject = e.message.flowProject;
                            window = EWindowView.PROJECT_HUB;
                        }
                    });
                }
            }
        }
    }

    // Currently not used
    private void _CreateDeleteProjectView()
    {
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
                    Operations.DeleteProject(project, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
                    {
                        Debug.Log(e.message);
                        if (e.message.WasSuccessful == true)
                        {
                            window = EWindowView.USER_HUB;
                        }
                    });
                }
            }
        }
    }

    private void _CreateDeleteVSGraphView()
    {
        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the Project Hub screen
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
        }
        EditorGUILayout.EndHorizontal();

        // TODO: This info is likely incorrect for graphs, needs to be thought about
        foreach (FlowVSGraph currentFlowVSGraph in FlowVSGraph.idToVSGraphMapping.Values)
        {
            if (GUILayout.Button(currentFlowVSGraph.Name, GUILayout.Height(30)))
            {
                Operations.DeleteVSGraph(currentFlowVSGraph.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { Debug.Log("Deleted VSGraph " + e.message); });
            }
        }
    }

    private void _CreateDeleteProjectConfirmationView()
    {
        GUILayout.Label("Are you sure you want to delete the current project?");
        // Create "Confirm Project Deletion" Button and define onClick action
        if (GUILayout.Button("Confirm Project Deletion", GUILayout.Height(40)))
        {
            DeleteProject();

            window = EWindowView.USER_HUB;
        }
        if (GUILayout.Button("Cancel", GUILayout.Height(40)))
        {
            window = EWindowView.PROJECT_HUB;
        }
    }

    private void DeleteProject()
    {
        Operations.DeleteProject(ConfigurationSingleton.SingleInstance.CurrentProject, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
        {
            if (e.message.WasSuccessful == true)
            {
                ConfigurationSingleton.SingleInstance.CurrentProject = null;

                GameObject[] wbList;
                wbList = GameObject.FindGameObjectsWithTag("Canvas");

                foreach (GameObject wb in wbList)
                {
                    // wb.transform.GetChild(2).gameObject.GetComponent<RealityFlowGraphView>().ClearGraph;
                    GameObject runeTimeGraphObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(2).gameObject;
                    RealityFlowGraphView rfgv = runeTimeGraphObject.GetComponent<RealityFlowGraphView>();
                    rfgv.ClearWhiteBoard();
                }

                FlowTObject.RemoveAllObjectsFromScene();
                FlowVSGraph.RemoveAllGraphsFromScene();
            }
            else
            {
                Debug.LogWarning("Error deleting project.");
            }
        });
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
                ConfigurationSingleton.SingleInstance.CurrentProject = new FlowProject("GUID", "description", 0, projectName);

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
                    }
                });
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
                // Send the user to the Project Hub screen
                window = EWindowView.PROJECT_HUB;
            }
        }
    }

    private void ExitProject()
    {
        // Check in all objects
        foreach (FlowTObject obj in FlowTObject.idToGameObjectMapping.Values)
        {
            if (obj.CanBeModified == true)
            {
                Operations.CheckinObject(obj.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
            }
        }

        // Check in all graphs
        foreach (FlowVSGraph graph in FlowVSGraph.idToVSGraphMapping.Values)
        {
            if (graph.CanBeModified == true)
            {
                Operations.CheckinVSGraph(graph.Id, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) => { });
            }
        }

        Operations.LeaveProject(ConfigurationSingleton.SingleInstance.CurrentProject.Id, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
        {
            if (e.message.WasSuccessful == true)
            {
                ConfigurationSingleton.SingleInstance.CurrentProject = null;

                GameObject[] wbList;
                wbList = GameObject.FindGameObjectsWithTag("Canvas");

                foreach (GameObject wb in wbList)
                {
                    // wb.transform.GetChild(2).gameObject.GetComponent<RealityFlowGraphView>().ClearGraph;
                    GameObject runeTimeGraphObject = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(2).gameObject;
                    RealityFlowGraphView rfgv = runeTimeGraphObject.GetComponent<RealityFlowGraphView>();
                    rfgv.ClearWhiteBoard();
                }

                FlowTObject.RemoveAllObjectsFromScene();
                FlowVSGraph.RemoveAllGraphsFromScene();
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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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

                    Operations.DeleteBehaviour(deleteBehaviourIds, ConfigurationSingleton.SingleInstance.CurrentProject.Id, (_, e) =>
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
                    if (userIsGuest)
                    {
                        window = EWindowView.GUESTPROJECT_HUB;
                    }
                    else
                    {
                        window = EWindowView.PROJECT_HUB;
                    }
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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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
        }

        EditorGUILayout.EndHorizontal();

        //if (/*addingChain*/)
        if (addingChain)
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

    private void _CreateTeleportView()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Back", GUILayout.Height(30), GUILayout.Width(40)))
        {
            window = EWindowView.CREATE_BEHAVIOUR;
        }

        GUILayout.Space(10f);
        EditorGUILayout.LabelField("Teleport");

        EditorGUILayout.LabelField("Select the object that will teleport.");
        GUILayout.Space(30f);

        selectedTrigger = EditorGUI.Popup(new Rect(0, 100, position.width, 30), "TriggerObject:", selectedTrigger, objectOptions);
        GUILayout.Space(30f);

        CreateTransformUI("teleport");

        GUILayout.Space(120f);
        GUILayout.Label("Add another interaction?");
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(40)))
        {
            string firstObjectId = objectIds[selectedTrigger].ToString();

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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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
            if (userIsGuest)
            {
                window = EWindowView.GUESTPROJECT_HUB;
            }
            else
            {
                window = EWindowView.PROJECT_HUB;
            }
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
        if (BehaviourEventManager.PreviousBehaviourId != null)
        {
            behavioursToLinkTo.Add(BehaviourEventManager.PreviousBehaviourId);
        }

        // Create the new behaviour
        Operations.CreateBehaviour(newFlowBehaviour, ConfigurationSingleton.SingleInstance.CurrentProject.Id, behavioursToLinkTo, (sender, e) =>
        {
            if (e.message.WasSuccessful == true)
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