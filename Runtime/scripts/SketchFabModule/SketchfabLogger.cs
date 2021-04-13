using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class SketchfabLogger : MonoBehaviour
{
    public GameObject loggerWindow;
    public GameObject accessKeyWindow;
    public GameObject modelResultsWindow;
    private string accessTokenKey = "skfb_access_token";
    //SketchfabProfile _current;
    RefreshCallback _refresh;
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    string username;
    string password = "";
    bool _isUserLogged = false;
    bool _hasCheckedSession = false;

    public enum LOGIN_STEP
    {
        GET_TOKEN,
        CHECK_TOKEN,
        USER_INFO
    }

    public SketchfabLogger(RefreshCallback callback = null)
    {
        _refresh = callback;
        checkAccessTokenValidity();
        /*
        if (username == null)
        {
            username = EditorPrefs.GetString("skfb_username", "");
        }
        */
    }

    public bool isUserLogged()
    {
        return _isUserLogged;
    }

    /*
    public bool canAccessOwnModels()
    {
        return !isUserBasic();
    }

    public SketchfabProfile getCurrentSession()
    {
        return _current;
    }
    */

    public void Login()
    {
        if(usernameInputField.text != null && passwordInputField != null)
        {
            username = usernameInputField.text.ToString();
            password = passwordInputField.text.ToString();
            requestAccessToken(username, password);
        }
    }
    public void logout()
    {
        accessTokenKey = "";
        _isUserLogged = false;
        _hasCheckedSession = true;
    }

    public void requestAccessToken(string user_name, string user_password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("username", user_name));
        formData.Add(new MultipartFormDataSection("password", user_password));

        SketchfabRequest tokenRequest = new SketchfabRequest(SketchfabPlugin.Urls.oauth, formData);
        tokenRequest.setCallback(handleGetToken);
        tokenRequest.setFailedCallback(onLoginFailed);
        SketchfabPlugin.getAPI().registerRequest(tokenRequest);
    }

    private void handleGetToken(string response)
    {
        string access_token = parseAccessToken(response);
        //EditorPrefs.SetString("skfb_username", username);
        if (access_token != null)
        {
            _isUserLogged = true;
            registerAccessToken(access_token);
            loggerWindow.SetActive(!loggerWindow.activeInHierarchy);
            accessKeyWindow.SetActive(!accessKeyWindow.activeInHierarchy);
            accessKeyWindow.GetComponent<TMP_Text>().text = accessTokenKey;
            modelResultsWindow.SetActive(!modelResultsWindow.activeInHierarchy);
        }

        /*if (_current == null)
        {
            requestUserData();
        }
        */
        // _refresh();
    }

    private string parseAccessToken(string text)
    {
        JSONNode response = Utils.JSONParse(text);
        if (response["access_token"] != null)
        {
            return response["access_token"];
        }

        return null;
    }

    private void registerAccessToken(string access_token)
    {
        accessTokenKey = access_token;
        Debug.Log(accessTokenKey);
    }

    /*
    public void requestAvatar(string url)
    {
        string access_token = EditorPrefs.GetString(accessTokenKey);
        if (access_token == null || access_token.Length < 30)
        {
            Debug.Log("Access token is invalid or inexistant");
            return;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + access_token);
        SketchfabRequest request = new SketchfabRequest(url, headers);
        //request.setCallback(handleAvatar);
        SketchfabPlugin.getAPI().registerRequest(request);
    }
    */
    public Dictionary<string, string> getHeader()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " + accessTokenKey);
        return headers;
    }

    /*
    private string getAvatarUrl(JSONNode node)
    {
        JSONArray array = node["avatar"]["images"].AsArray;
        foreach (JSONNode elt in array)
        {
            if (elt["width"].AsInt == 100)
            {
                return elt["url"];
            }
        }

        return "";
    }
    */
    /*
    // Callback for avatar
    private void handleAvatar(byte[] responseData)
    {
        if (_current == null)
        {
            Debug.Log("Invalid call avatar");
            return;
        }
        bool sRGBBackup = GL.sRGBWrite;
        GL.sRGBWrite = true;

        Texture2D tex = new Texture2D(4, 4);
        tex.LoadImage(responseData);
#if UNITY_5_6 || UNITY_2017
        if (PlayerSettings.colorSpace == ColorSpace.Linear)
        {
            var renderTexture = RenderTexture.GetTemporary(tex.width, tex.height, 24);
            Material linear2SRGB = new Material(Shader.Find("GLTF/Linear2sRGB"));
            linear2SRGB.SetTexture("_InputTex", tex);
            Graphics.Blit(tex, renderTexture, linear2SRGB);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        }
#endif
        TextureScale.Bilinear(tex, (int)AVATAR_SIZE.x, (int)AVATAR_SIZE.y);
        _current.setAvatar(tex);

        GL.sRGBWrite = sRGBBackup;
        if (_refresh != null)
            _refresh();
    }
    */

    /*
    public void requestUserData()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", "Bearer " +accessTokenKey);
        SketchfabRequest request = new SketchfabRequest(SketchfabPlugin.Urls.userMe, headers);
        request.setCallback(handleUserData);
        request.setFailedCallback(logout);
        SketchfabPlugin.getAPI().registerRequest(request);
    }
    */

    private void onLoginFailed(string res)
    {
        JSONNode response = Utils.JSONParse(res);
        Debug.Log("Login error, Authentication failed: " + response["error_description"]);
        logout();
    }

    public void checkAccessTokenValidity()
    {
        string access_token = accessTokenKey;
        if (access_token == null || access_token.Length < 30)
        {
            _hasCheckedSession = true;
            return;
        }
        //requestUserData();
    }

    private void handleUserData(string response)
    {
        JSONNode userData = Utils.JSONParse(response);
        //_current = new SketchfabProfile(userData["username"], userData["displayName"], userData["account"]);
        //requestAvatar(getAvatarUrl(userData));
        _isUserLogged = true;
        _hasCheckedSession = true;
    }

    /*
    public bool canPrivate()
    {
        return _current != null && _current._userCanPrivate == 1;
    }

    public bool checkUserPlanFileSizeLimit(long size)
    {
        if (_current == null)
            return false;
        if (_current.maxUploadSize > size)
            return true;

        return false;
    }

    public bool isUserBasic()
    {
        if (_current != null)
            return _current.accountLabel == "BASIC" || _current.accountLabel == "PLUS";
        else
            return true;
    }

    private void onCanPrivate(string response)
    {
        JSONNode planResponse = Utils.JSONParse(response);
        _current._userCanPrivate = planResponse["canProtectModels"].AsBool ? 1 : 0;
    }
    */
}
