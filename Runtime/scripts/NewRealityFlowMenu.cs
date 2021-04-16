using Behaviours;
using Packages.realityflow_package.Runtime.scripts;
using Packages.realityflow_package.Runtime.scripts.Structures.Actions;
using RealityFlow.Plugin.Scripts;
using System;
//using System.Web.Security;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEditor;
using TMPro;


public class NewRealityFlowMenu : MonoBehaviour
{
   //private string _Url = "ws://plato.mrl.ai:8999";
    private string _Url = "ws://dc9f3aff9592.ngrok.io"; // the one we are using
    private const string Url = "ws://a73c9fa8.ngrok.io";

    // Keyboard
    public TouchScreenKeyboard keyboard;
    // View parameters
    private string uName;
    private string pWord;
    private string tempUName;
    private string tempPWord;
    private string projectName;
    private string userToInvite;
    private bool userIsGuest = false;

    private bool _RefreshProjectList = true;
    private bool displayProjectCode = false;
    private string openProjectId;

    //private Dictionary<EWindowView, ChangeView> _ViewDictionary = new Dictionary<EWindowView, ChangeView>();

    //private delegate void ChangeView();

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

    [Header("Panels")]
    public GameObject realityFlowMenuPanel;
    public GameObject guestUserComfirmationPanel;
    public GameObject guestUserHubPanel;
    public GameObject guestUserProjectPanel;

    [Header("TextFields")]
    public TMP_InputField mrtkTextBox = null;

    private enum MenuOptions
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

    public void OpenSystemKeyboard()
    {
        Debug.Log("Open Keyboard");
        keyboard =  TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
    public void Update() 
    {
        if (keyboard != null)
        {
            openProjectId = keyboard.text;
            // Do stuff with keyboardText
        }

        if (FlowWebsocket.websocket != null)
        {
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(Operations._FlowWebsocket.ReceiveMessage(), Operations._FlowWebsocket);
        }
    }

    public void SetupGuest()
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
        
        // Show Guest Confirm Window and turn off reality flow panel
        guestUserComfirmationPanel.SetActive(!guestUserComfirmationPanel.activeInHierarchy);
        realityFlowMenuPanel.SetActive(!realityFlowMenuPanel.activeInHierarchy);   
    }

    public void _ConfirmGuestLogin()
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
                    // Turn off confirmation panel and turn on guest hub view
                    guestUserHubPanel.SetActive(!guestUserHubPanel.activeInHierarchy);
                    guestUserComfirmationPanel.SetActive(!guestUserComfirmationPanel.activeInHierarchy);
                    Debug.Log("HIIIII");
                });
            }
            else
            {
                //Operations.DeleteUser(new FlowUser(tempUName, tempPWord));
                ConfigurationSingleton.SingleInstance.CurrentUser = null;
            }
        });

    }

    public void GuestUserJoinProject()
    {
        openProjectId = KeyboardManager.Instance.InputField.text.ToString();
        Debug.Log("Open Project Id: " + openProjectId);
        if(openProjectId != null) // let this be Join button if we have an openProjectId
        {
            Operations.OpenProject(openProjectId, ConfigurationSingleton.SingleInstance.CurrentUser, (_, e) =>
            {
                if (e.message.WasSuccessful == true)
                {
                    Debug.Log(e.message);
                    if (e.message.WasSuccessful == true)
                    {
                        ConfigurationSingleton.SingleInstance.CurrentProject = e.message.flowProject;
                        // Show GuestProjectHub
                        
                        guestUserProjectPanel.SetActive(!guestUserProjectPanel.activeInHierarchy);
                        guestUserHubPanel.SetActive(!guestUserHubPanel.activeInHierarchy);
                    }
                }
            });
        }
    }
    public void GuestUserLogout()
    {
        // Send logout event to the server
        if (ConfigurationSingleton.SingleInstance.CurrentUser != null)
        {
            // TODO: Logic for deleting user after logout
            //FlowUser toDelete = ConfigurationSingleton.SingleInstance.CurrentUser;
            Operations.DeleteUser(ConfigurationSingleton.SingleInstance.CurrentUser);

            userIsGuest = false;
            //window = EWindowView.LOGIN;
            realityFlowMenuPanel.SetActive(!realityFlowMenuPanel.activeInHierarchy);
            guestUserProjectPanel.SetActive(!guestUserProjectPanel.activeInHierarchy);
      
        }
    }
}
