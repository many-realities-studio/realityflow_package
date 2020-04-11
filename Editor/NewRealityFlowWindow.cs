using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using RealityFlow.Plugin.Scripts;
using RealityFlow.Plugin.Editor;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Messages.ObjectMessages;

[CustomEditor(typeof(FlowWebsocket))]
public class FlowNetworkManagerEditor : EditorWindow
{
    private string _Url = "ws://plato.mrl.ai:8999";

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
    private string openProjectId = "";


    private Dictionary<EWindowView, ChangeView> _ViewDictionary = new Dictionary<EWindowView, ChangeView>();
    private delegate void ChangeView();

    private IList<FlowProject> _ProjectList = null;

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
            uName = "";
            pWord = "";
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

        GUILayout.BeginHorizontal();
        GUILayout.Label("Open project by Id");
        openProjectId = EditorGUILayout.TextField(openProjectId);
        if (GUILayout.Button("Open"))
        {
            ProjectDebugInfo = "";
            if (openProjectId != "")
            {
                Debug.Log("opening project: " + openProjectId);
                Operations.OpenProject(openProjectId, ConfigurationSingleton.CurrentUser, (_, e) => 
                {
                    if (e.message.WasSuccessful == true)
                    {
                        ConfigurationSingleton.CurrentProject = e.message.flowProject;
                        window = EWindowView.PROJECT_HUB;
                    }
                    else
                    {
                        ProjectDebugInfo = "Project does not exist";
                    }
                });
                openProjectId = "";
            }
            else
            {
                ProjectDebugInfo = "Please enter the project ID before attempting to open";
                openProjectId = "";
            }
        }
        GUILayout.EndHorizontal();

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

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(ProjectDebugInfo);
        GUILayout.EndHorizontal();
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
    }

    private bool _RefreshProjectList = true;
    private string ProjectDebugInfo;

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

        if(_ProjectList != null)
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
                    if(e.message.WasSuccessful == true)
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
}
