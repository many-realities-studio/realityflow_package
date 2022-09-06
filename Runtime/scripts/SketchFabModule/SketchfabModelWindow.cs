/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using TMPro;

public class SketchfabModelWindow : MonoBehaviour
{
    public Image prefabImage;
    public TMP_Text modelName;
    public TMP_Text modelAuthor;
    //public Button download;

    SketchfabModel _currentModel;
    //SketchfabUI _ui;
    //SketchfabBrowser _window;

    string _prefabName;
    string _importDirectory;
    bool _addToCurrentScene;
    SketchfabRequest _modelRequest;
    SketchfabBrowserWindow _browser;
    bool show = false;
    byte[] _lastArchive;

    Vector2 _scrollView = new Vector2();
    
    public void destroyModelPage()
    {
        Destroy(this);
    }
    public void displayModelPage(SketchfabModel model, SketchfabBrowserWindow browser)
    {
        _browser = browser;
        if(_currentModel == null || model.uid != _currentModel.uid)
        {

            _currentModel = model;
            Debug.Log("Current Model is: "+_currentModel.name);
            _prefabName = _currentModel.name;
            _importDirectory = Application.dataPath + "/Import/" + _prefabName.Replace(" ", "_");
        }
        else
        {
            _currentModel = model;
        }
    modelName.text = _currentModel.name;

        modelAuthor.text = _currentModel.author;
        prefabImage.sprite = Sprite.Create(_currentModel._preview, new Rect(0.0f, 0.0f, _currentModel._preview.width, _currentModel._preview.height), new Vector2(0.5f, 0.5f), 100.0f);
        show = true;
    }

/*
    void onChangImportDirectoryClick()
    {
        string newImportDir = EditorUtility.OpenFolderPanel("Choose import directory", Application.dataPath, "");
        if (GLTFUtils.isFolderInProjectDirectory(newImportDir))
        {
            _importDirectory = newImportDir;
        }
        else if (newImportDir != "")
        {
            EditorUtility.DisplayDialog("Error", "Please select a path within your current Unity project (with Assets/)", "Ok");
        }
    }
*/
    public void onImportModelClick()
    {
        /*
        if (!assetAlreadyExists() || EditorUtility.DisplayDialog("Override asset", "The asset " + _prefabName + " already exists in project. Do you want to override it ?", "Override", "Cancel"))
        {
            // Reuse if still valid
            if (_currentModel.tempDownloadUrl.Length > 0 && EditorApplication.timeSinceStartup - _currentModel.downloadRequestTime < _currentModel.urlValidityDuration)
            {
                requestArchive(_currentModel.tempDownloadUrl);
            }
            else
            {
                fetchGLTFModel(_currentModel.uid, OnArchiveUpdate, _window._logger.getHeader());
            }
        }
        */
        if (_currentModel.tempDownloadUrl.Length > 0)
        {
            requestArchive(_currentModel.tempDownloadUrl);
        }
        else
        {
            Debug.Log("UID: "+_currentModel.uid);
            if(_browser == null)
            {
                Debug.Log("Browser null");
            }
            if(_browser._logger == null)
            {
                Debug.Log("Logger null");
            }
            fetchGLTFModel(_currentModel.uid, OnArchiveUpdate, _browser._logger.getHeader());
        }
    }

    private bool assetAlreadyExists()
    {
        string prefabPath = _importDirectory + "/" + _prefabName + ".prefab";
        return File.Exists(prefabPath);
    }

    private void OnArchiveUpdate()
    {
        //EditorUtility.ClearProgressBar();
        string _unzipDirectory = Application.temporaryCachePath + "/unzip";
        //_browser._browserManager.setImportProgressCallback(UpdateProgress);
        //_browser._browserManager.setImportFinishCallback(OnFinishImport);
        
        // Need this
        _browser._browserManager.importArchive(_lastArchive, _unzipDirectory, _importDirectory, _prefabName, _addToCurrentScene);
    }

    private void handleDownloadCallback(float current)
    {
        if(_modelRequest != null)
        {
            _browser._browserManager._api.dropRequest(ref _modelRequest);
            _modelRequest = null;
        }
    }


    private void OnFinishImport()
    {
        Debug.Log("Finished Import");
    }

    public void fetchGLTFModel(string uid, RefreshCallback fetchedCallback, Dictionary<string, string> headers)
    {
        string url = SketchfabPlugin.Urls.modelEndPoint + "/" + uid + "/download";
        _modelRequest = new SketchfabRequest(url, headers);
        _modelRequest.setCallback(handleDownloadAPIResponse);
        _browser._browserManager._api.registerRequest(_modelRequest);
    }

    void handleArchive(byte[] data)
    {
        _lastArchive = data;
        OnArchiveUpdate();
    }


    void handleDownloadAPIResponse(string response)
    {
        JSONNode responseJson = Utils.JSONParse(response);
        if(responseJson["gltf"] != null)
        {
            _currentModel.tempDownloadUrl = responseJson["gltf"]["url"];
            _currentModel.urlValidityDuration = responseJson["gltf"]["expires"].AsInt;
        // _currentModel.downloadRequestTime = EditorApplication.timeSinceStartup;
            requestArchive(_currentModel.tempDownloadUrl);
        }
        else
        {
            Debug.Log("Unexpected Error: Model archive is not available");
        }
        //this.Repaint();
    }

    void requestArchive(string modelUrl)
    {
        SketchfabRequest request = new SketchfabRequest(_currentModel.tempDownloadUrl);
        request.setCallback(handleArchive);
        //request.setProgressCallback(handleDownloadCallback);
        SketchfabPlugin.getAPI().registerRequest(request);
    }

/*

    public void UpdateProgress(UnityGLTF.GLTFEditorImporter.IMPORT_STEP step, int current, int total)
    {
        string element = "";
        switch (step)
        {
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.BUFFER:
                element = "Buffer";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.IMAGE:
                element = "Image";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.TEXTURE:
                element = "Texture";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.MATERIAL:
                element = "Material";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.MESH:
                element = "Mesh";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.NODE:
                element = "Node";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.ANIMATION:
                element = "Animation";
                break;
            case UnityGLTF.GLTFEditorImporter.IMPORT_STEP.SKIN:
                element = "Skin";
                break;
        }

        //EditorUtility.DisplayProgressBar("Importing glTF", "Importing " + element + " (" + current + " / " + total + ")", (float)current / (float)total);
        //this.Repaint();
    }
*/
/*
    private void OnDestroy()
    {
        if(_window != null)
            _window.closeModelPage();
    }
*/

    public void CloseWindow()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
    }
}
