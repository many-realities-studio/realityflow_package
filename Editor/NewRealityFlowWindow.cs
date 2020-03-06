﻿using System.Collections.Generic;
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
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.UserMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.ProjectMessages;
using Packages.realityflow_package.Runtime.scripts.Messages.RoomMessages;

[CustomEditor(typeof(FlowWebsocket))]
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

    private Tuple<string, string>[] _ProjectList = null;

    FlowTObject newObject = null;// = new FlowTObject(new Color(0, 0, 0, 0), "TestFlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "TestObject");
    FlowUser newUser = null;// = new FlowUser("testUsername", "TestPassword");

    public static FlowUser currentUser = null;
    public static FlowProject currentProject = null;

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
    FlowTObject testObject;
    FlowUser testUser;
    FlowProject testProject;

    /// <summary>
    /// This function is called when this object becomes enabled and active
    /// </summary>
    private void OnEnable()
    {
        if(testObject == null)
        {
             testObject= new FlowTObject(new Color(0, 0, 0), "FlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "name");
        }
        if (testUser == null)
        {
             testUser = new FlowUser("user", "pass"); ;
        }
        if(testProject == null)
        {
            testProject = new FlowProject("flowId", "description", 0, "projectName", new FlowTObject[]
            {
                 testObject
            });
        }

        if(newObject == null)
            newObject = new FlowTObject(new Color(0, 0, 0, 0), "TestFlowId", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "TestObject");
        if(newUser == null)
            newUser = new FlowUser("testUsername", "TestPassword");

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

            if(newUser == null)
            {
                newUser = new FlowUser("user", "pass");
            }

            // User messages
            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(LoginUser_Received).ToString() + ".json", MessageSerializer.ConvertToString( new LoginUser_Received("message", true)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(RegisterUser_Received).ToString() + ".json", MessageSerializer.ConvertToString(new RegisterUser_Received("message", true)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\UserMessages\" + typeof(LogoutUser_Received).ToString() + ".json", MessageSerializer.ConvertToString(new LogoutUser_Received("message", true)));

            // Object messages
            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(CreateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new CreateObject_Received(testObject)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(DeleteObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteObject_Received(testObject)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(UpdateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new UpdateObject_Received(testObject)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ObjectMessages\" + typeof(FinalizedUpdateObject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new FinalizedUpdateObject_Received(testObject)));

            // Project messages
            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(CreateProject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new CreateProject_Received("message", true)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(DeleteProject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new DeleteProject_Received("message", true)));

            Tuple<string, string>[] testList = { new Tuple<string, string>(testProject.FlowId, testProject.ProjectName) };
            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(GetAllUserProjects_Received).ToString() + ".json", MessageSerializer.ConvertToString(new GetAllUserProjects_Received(testList)));

            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\ProjectMessages\" + typeof(OpenProject_Received).ToString() + ".json", MessageSerializer.ConvertToString(new OpenProject_Received(testProject)));

            // Room messages
            System.IO.File.WriteAllText(@"C:\Users\Matthew Kurtz\Desktop\FlowTests\SentCommands\RoomMessages\" + typeof(JoinRoom_Received).ToString() + ".json", MessageSerializer.ConvertToString(new JoinRoom_Received(testProject)));

            //Operations.CreateObject(newObject, newUser, "TestProjectId", CreateObjectCallbackTest);

            //UpdateObject_SendToServer updateObject = new UpdateObject_SendToServer(newObject, newUser, "ProjectId");
            //Operations.FlowWebsocket.SendMessage(updateObject);

            //Operations.DeleteObject("TestProjectId", DeleteObjectCallback);


            //FinalizedUpdateObject_SendToServer finalizedUpdateObject = new FinalizedUpdateObject_SendToServer(newObject, newUser, "ProjectId");
            //Operations.FlowWebsocket.SendMessage(finalizedUpdateObject);
        }

        // if (GUILayout.Button("Edit", GUILayout.Height(20)))
        // {
        //     if(NewObjectManager.Sphere == null)
        //     {
        //         NewObjectManager.Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     }
        //     FlowTObject newObject = new FlowTObject(NewObjectManager.Sphere);
        //     newObject.flowId = 2.ToString();

        //     NewObjectManager.EditObject(newObject);
        // }

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

    private void DeleteObjectCallback(object sender, DeleteObjectMessageEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    private void CreateObjectCallbackTest(object sender, CreateObjectMessageEventArgs eventArgs)
    {
        Debug.Log("Final: " + eventArgs.message.ToString());
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
            currentUser = new FlowUser(uName, pWord);
            Operations.Login(currentUser, (_, e) =>
            {
                Debug.Log("login callback: " + e.message.WasSuccessful.ToString());
                if (e.message.WasSuccessful == true)
                {
                    window = EWindowView.USER_HUB;
                }
                else
                {
                    currentUser = null;
                    // TODO: Display that login failed
                }
            });
        }

        // Create "Register" Button and define onClick action

        if (GUILayout.Button("Register", GUILayout.Height(30)))
        {
            Operations.Register(uName, pWord, (sender, e) => { Debug.Log(e.message); });
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
            if(currentUser != null)
            {
                Operations.Logout(currentUser, (sender, e) => { Debug.Log(e.message); });
            }
            // Set logged (global) state to false
            //loggedIn = false;

            // Clear the user ID
            //Config.userId = "-9999";

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
        //GameObject objectManagerGameObject = GameObject.FindGameObjectWithTag("ObjManager");

        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the Project Hub screen
            window = EWindowView.PROJECT_HUB;
        }
        EditorGUILayout.EndHorizontal();
        Dictionary<string, GameObject>.KeyCollection gameObjectList = FlowTObject.idToGameObjectMapping.Keys;

        foreach(string FlowId in gameObjectList)
        {
            if(GUILayout.Button(FlowTObject.idToGameObjectMapping[FlowId].name, GUILayout.Height(30)))
            {
                //TODO: make sure this deletes the object
                Operations.DeleteObject(FlowId, (_, e) => Debug.Log(e.message));
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
        if(_RefreshProjectList == true)
        {
            Operations.GetAllUserProjects(currentUser, (_, e) =>
            {
                _ProjectList = e.message.ProjectList;
                Debug.Log(e.message);
            });

            _RefreshProjectList = false;
        }

        // Create "Back" Button and define onClick action
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back", GUILayout.Height(20)))
        {
            // Send the user to the User Hub screen
            window = EWindowView.USER_HUB;
        }
        EditorGUILayout.EndHorizontal();

        if(_ProjectList != null)
        {
            foreach (Tuple<string, string> project in _ProjectList)
            {
                if (GUILayout.Button(project.Item2, GUILayout.Height(30)))
                {
                    Operations.OpenProject(project.Item1, currentUser, (_, e) =>
                    {
                        Debug.Log(e.message);
                        window = EWindowView.PROJECT_HUB;
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
                currentProject = new FlowProject("GUID", "", 0, "", new List<FlowTObject>());
                // TODO: Generate guid
                Operations.CreateProject(currentProject, currentUser, (_, e) =>
                {
                    if(e.message.WasSuccessful == true)
                    {
                        window = EWindowView.PROJECT_HUB;
                        Debug.Log(e.message);
                    }
                    else
                    {
                        currentProject = null;
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends a Delete event to the server to delete the desired game object
    /// </summary>
    /// <param name="objectToBeDeleted"></param>
    public void DeleteObject(FlowTObject objectToBeDeleted)
    {
        Operations.DeleteObject(objectToBeDeleted.FlowId, (_, e) => { Debug.Log(e.message); });
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
}