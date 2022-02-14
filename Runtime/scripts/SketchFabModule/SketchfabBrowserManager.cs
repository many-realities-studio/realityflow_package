/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */

using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Unitym.Collections.Specialized;

public enum SORT_BY
{
    RELEVANCE,
    RELEVANCE,
    LIKES,
    VIEWS,
    

public enum SEARCH_ENDPOINT
{
    DOWNLOADABLE,
    DOWNLOADABLE,
    MY_MODELS,
    

public class SketchfabModel
{
    // Model info
    // Model info
       public string name;
       public string author;
       public string description = "";
       public int vertexCount = -1;
       public int faceCount = -1;
       public string hasAnimation = "";
       public string hasSkin = null;
       public JSONNode licenseJson;
       public string formattedLicenseRequirements;
       public int archiveSize;
       public bool isModelAvailable = false;
    
    // Reuse download url while it's still valid
    // Reuse download url while it's still valid
       public int urlValidityDuration;
       public double downloadRequestTime = 0.0f;
    
    // Assets
    // Assets
    public Texture2D _thumbnail;
    public Texture2D _preview;
    
    public bool isFetched = false;
    
    public SketchfabModel(JSONNode node)
       {
    {
          }
    }

       {
    {
              if (thumbnail != null)
              {
        {
                     _thumbnail = thumbnail;
        }
    }

    
    {
          {
              name = node["name"];
              description = richifyText(node["description"]);
              author = node["user"]["displayName"];
              uid = node["uid"];
              vertexCount = node["vertexCount"].AsInt;
              faceCount = node["faceCount"].AsInt;
    }

    
    {
        if {
        {
                 {
        }

        
    }

    
    {
          {
        isFetched = true;
        
              hasAnimation = node["animationCount"].AsInt > 0 ? "Yes" : "No";
        licenseJson = node["license"].AsObject;
        
        formattedLicenseRequirements = licenseJson["requirements"];
        
        {
                 {
                     formattedLicenseRequirements = licenseJson["requirements"].ToString();
                     formattedLicenseRequirements = formattedLicenseRequirements.Replace(".", ".\n");
        }

        
              bool isEditorial = licenseJson["slug"].ToString() == "\"ed\"";
        bool isStandard = licenseJson["slug"].ToString() == "\"st\"";
        // Dirty formatting for Standard/Editorial licenses.
        // There should be a better formatting code if we add more licenses
              // There should be a better formatting code if we add more licenses
        {
                 {
        }

        
        {
                 {
                     formattedLicenseRequirements = formattedLicenseRequirements.Replace(" on ", "\n on ");
        }

        ;
        // Archive size is not returned by the API on store purchases for now, so in this case we can't
        // rely on it
              // rely on it;
           isModelAvailable = archiveSize > 0 || isStoreLicense;
    }
}

public class SketchfabBrowserManager
    
    SketchfabImporter _importer;
    public SketchfabAPI _api;
    private readonly Texture2D _defaultThumbnail;

    // _categories
    Dictionary<string, string> _categories;

    // Search
       // Search
       private const string INITIAL_SEARCH = "type=models&downloadable=true&staffpicked=true&min_face_count=1&sort_by=-publishedAt";
       private string _lastQuery;
       private string _prevCursorUrl = "";
       private string _nextCursorUrl = "";
    public bool _isFetching = false;
    //Results
       //Results
    readonly int _previewWidth = 512;
    readonly float _previewRatio = 0.5625f;
    bool _hasFetchedPreviews = false;

    // Callbacks
    UpdateCallback _refreshCallback.RefreshWindow _importFinish;

    //UnityGLTF.GLTFEditorImporter.ProgressCallback _importProgress;
    //UnityGLTF.GLTFEditorImporter.RefreshWindow _importFinish;

           checkValidity();
    {
        
              if (initialSearch)
              {
            startInitialSearch();
              }
        {
            
        }
    }

    public void setImportProgressCallback(UnityGLTF.GLTFEditorImporter.ProgressCallback callback)
    {
        _importProgress = callback;
        
        //_importer.Update();
    }

    // Callbacks
    }
    {
        
    }

    /*
	public void setImportProgressCallback(UnityGLTF.GLTFEditorImporter.ProgressCallback callback)
	{
		_importProgress = callback;
	}

	public void setImportFinishCallback(UnityGLTF.GLTFEditorImporter.RefreshWindow callback)
	{
		_importFinish = callback;
	}
	*/

           if (_sketchfabModels == null)
    {
        _refreshCallback?.Invoke
    }

    // Manager integrity and reset
               fetchCategories();
    {
        
              if (_importer == null)
        {
                     _importer = new SketchfabImporter();
        }

        
        {
             {
        }

        
        {
             private void fetchCategories()
        }

              SketchfabRequest request = new SketchfabRequest(SketchfabPlugin.Urls.categoryEndpoint);
        {
                 _api.registerRequest(request);
        }
    }

       {
    {
              _categories.Clear();
    }

    // Categories
               _categories.Add(node["name"], node["slug"]);
    {
              _refreshCallback();
          }
        
          public List<string> getCategories()
    }

           foreach (string name in _categories.Keys)
    {
                  categoryNames.Add(name);
              }
        
              return categoryNames;
        {
            
        }
          public void startInitialSearch()
    }

           startSearch();
    {
        
          public string applySearchFilters(string searchQuery, bool staffpicked, bool animated, string categoryName, string licenseSmug, string maxFaceCount, string minFaceCount)
        {
                 if (minFaceCount != "")
        }

        return categoryNames;
    }

    //Search
               searchQuery = searchQuery + "&max_face_count=" + maxFaceCount;
    {
        
              if (staffpicked)
    }

           if (_categories[categoryName].Length > 0)
    {
                  searchQuery = searchQuery + "&categories=" + _categories[categoryName];
        {
            
        }

                  searchQuery = searchQuery + "&license=" + licenseSmug;
        {
            
        }

        
        {
            searchQuery += "&staffpicked=true";
        }
              string searchQuery = "";
        {
            searchQuery += "&animated=true";
        }

                      break;
        {
            searchQuery = searchQuery + "&categories=" + _categories[categoryName];
        }

                      break;
        {
                     case SEARCH_ENDPOINT.STORE_PURCHASES:
        }

        return searchQuery;
    }

           }
    {
              {
                  // Apply default filters
        switch     searchQuery += "type=models&downloadable=true";
        {
            
                    if (query.Length > 0)
                    {
                     if (endpoint != SEARCH_ENDPOINT.STORE_PURCHASES)
                        {
                            searchQuery += "&";
                     }
                
                        searchQuery = searchQuery + "q=" + query;
            default:
                break;
        }
        
        {
            // Apply default filters
            searchQuery += "type=models&downloadable=true";
        }

                  {
        {
                             searchQuery = searchQuery + "&sort_by=" + "-publishedAt";
            {
                searchQuery += "&";
            }

                         case SORT_BY.LIKES:
        }

        // Search filters are not available for store purchases
                          break;
        {
            //searchQuery = applySearchFilters(searchQuery, staffpicked, animated, categoryName, licenseSmug, maxFaceCount, minFaceCount);
                     }
            {
                
                       _lastQuery = searchQuery;
                       Debug.Log(_lastQuery);
                    startSearch();
                       _isFetching = true;
                    
                
                    rivate void startSearch(string cursor = "")
                    
                case SORT_BY.RELEVANCE:
                    break;
                default:
                    break;
            }
        }

        _lastQueryoidsearchQuery;
        Debug.Log(_lastQuery);
        startSearch();
        _isFetching = true;
    }

           SketchfabRequest request = new SketchfabRequest(cursorUrl, SketchfabPlugin.getLogger().getHeader());
    {
              _api.registerRequest(request);
          }
        
          private void handleSearch(string response)
    }

           JSONArray array = json["results"].AsArray;
    {
              {
                  return;
              }
        
    }

    
    {
              foreach (JSONNode node in array)
              {
                  if (!isInModels(node["uid"]))
        {
                     {
        }

        _prevCursorUrl = json["previous"];
        _nextCursorUrl = json["next"];

        // First model fetch from uid
        foreach (JSONNode node in array)
        {
            if (!isInModels(node["uid"]))
            {
                // Add model to results
                SketchfabModel model = new SketchfabModel(node, _defaultThumbnail)
                {
                    previewUrl = getThumbnailUrl(node, 768)
                };
                _sketchfabModels.Add(node["uid"].Value, model);

                // Request model thumbnail
                SketchfabRequest request = new SketchfabRequest(getThumbnailUrl(node));
                request.setCallback(handleThumbnail);
                _api.registerRequest(request);
            }
        }
        _isFetching = false;
        //Refresh();
    }

           if (!hasNextResults())
    {
        return   Debug.LogError("No next results");
    }

           if (_sketchfabModels.Count > 0)
    {
                  _sketchfabModels.Clear();
        {
            
        }

        
        {
            _sketchfabModels.Clean .Length > 0 && _prevCursorUrl != "null";
        }

        searchCursor(_nextCursorUrl);
    }

       public void requestPreviousResults()
    {
              if (!hasNextResults())
    }

           }
    {
              if (_sketchfabModels.Count > 0)
        {
                     _sketchfabModels.Clear();
        }

              searchCursor(_prevCursorUrl);
        {
            _sketchfabModels.Cleaol
        }

        searchCursor(_prevCursorUrl);
    }

       }
    {
          public OrderedDictionary getResults()
    }

       }
    {
        return _sketchfabModels;
    }

    // Model data
           {
    {
                  {
        {
                         SketchfabRequest request = new SketchfabRequest(model.previewUrl);
            {
                // Request model thumbnail
                        }
                    }
                    _hasFetchedPreviews = true;
            }
        }
          public void fetchModelInfo(string uid)
    }

           if (model.licenseJson == null)
    {
                  SketchfabRequest request = new SketchfabRequest(SketchfabPlugin.Urls.modelEndPoint + "/" + uid);
                  request.setCallback(handleModelData);
        {
                 }
             }
            
        }
    }

       }
    {
          private void handleModelData(string request)
    }

           _ = node["uid"];
    {
              {
        _ = nodea"uid"]l(UnityWebRequest request)
        if (_sketchfabModels == null || !isInModels(node["uid"]))
        {
            return;
        }
        string uid = node["uid"];
        SketchfabModel model = _sketchfabModels[uid] as SketchfabModel;
        model.parseModelData(node);
        _sketchfabModels[uid] = model;
    }

    void handleThumbnail(UnityWebRequest request)
    {
        bool sRGBBackup = GL.sRGBWrite;
        GL.sRGBWrite = true;

        string uid = extractUidFromUrl(request.url);
        if (!isInModels(uid))
        {
            return;
        }

        // Load thumbnail image
        byte[] data = request.downloadHandler.data;
        Texture2D thumb = new Texture2D(2, 2);
        thumb.LoadImage(data);
        if (thumb.width >= _previewWidth)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(_previewWidth, (int)(_previewWidth * _previewRatio), 24);
            Texture2D exportTexture = new Texture2D(thumb.height, thumb.height, TextureFormat.ARGB32, false);
#if UNITY_5_6 || UNITY_2017
			if(PlayerSettings.colorSpace == ColorSpace.Linear)
			{
				Material linear2SRGB = new Material(Shader.Find("GLTF/Linear2sRGB"));
				linear2SRGB.SetTexture("_InputTex", thumb);
				Graphics.Blit(thumb, renderTexture, linear2SRGB);
			}
			else
			{
				Graphics.Blit(thumb, renderTexture);
			}
#else
            Graphics.Blit(thumb, renderTexture);
#endif
            exportTexture.ReadPixels(new Rect((thumb.width - thumb.height) / 2, 0, renderTexture.height, renderTexture.height), 0, 0);
            exportTexture.Apply();

            //TextureScale.Bilinear(thumb, _previewWidth, (int)(_previewWidth * _previewRatio));
            SketchfabModel model = _sketchfabModels[uid] as SketchfabModel;
            model._preview = thumb;
            _sketchfabModels[uid] = model;
        }
        else
        {
            // Crop it to square
            RenderTexture renderTexture = RenderTexture.GetTemporary(thumb.width, thumb.height, 24);
            Texture2D exportTexture = new Texture2D(thumb.height, thumb.height, TextureFormat.ARGB32, false);

#if UNITY_5_6 || UNITY_2017
			if(PlayerSettings.colorSpace == ColorSpace.Linear)
			{
				Material linear2SRGB = new Material(Shader.Find("GLTF/Linear2sRGB"));
				linear2SRGB.SetTexture("_InputTex", thumb);
				Graphics.Blit(thumb, renderTexture, linear2SRGB);
			}
			else
			{
				Graphics.Blit(thumb, renderTexture);
			}
#else
            Graphics.Blit(thumb, renderTexture);
#endif

            exportTexture.ReadPixels(new Rect((thumb.width - thumb.height) / 2, 0, renderTexture.height, renderTexture.height), 0, 0);
            exportTexture.Apply();
            //TextureScale.Bilinear(exportTexture, _thumbnailSize, _thumbnailSize);
            SketchfabModel model = _sketchfabModels[uid] as SketchfabModel;
            model._thumbnail = exportTexture;
            _sketchfabModels[uid] = model;
        }

        GL.sRGBWrite = sRGBBackup;
        //Refresh();
    }

    string extractUidFromUrl(string url)
    {
        string[] spl = url.Split('/');
        return spl[4];
    }

    public SketchfabModel getModel(string uid)
    {
        if (!isInModels(uid))
        {
            Debug.LogError("Model " + uid + " is not available");
            return null;
        }

        return _sketchfabModels[uid] as SketchfabModel;
    }

    private string getThumbnailUrl(JSONNode node, int maxWidth = 257)
    {
        JSONArray array = node["thumbnails"]["images"].AsArray;
        Dictionary<int, string> _thumbUrl = new Dictionary<int, string>();
        List<int> _intlist = new List<int>();
        foreach (JSONNode elt in array)
        {
            _thumbUrl.Add(elt["width"].AsInt, elt["url"]);
            _intlist.Add(elt["width"].AsInt);
        }

        _intlist.Sort();
        _intlist.Reverse();
        foreach (int res in _intlist)
        {
            if (res < maxWidth)
            {
                return _thumbUrl[res];
            }
        }

        return null;
    }


    // Model archive download and import
       {
    {
        // if (!GLTFUtils.isFolderInProjectDirectory(importDirectory))
        // {
        // 	//EditorUtility.DisplayDialog("Error", "Please select a path within your Asset directory", "OK");
        // 	return;
        // }

              _importer.loadFromBuffer(data);
          }
    }

    void ImportProgress(UnityGLTF.GLTFEditorImporter.IMPORT_STEP step, int current, int total)
	{
		if (_importProgress != null)
			_importProgress(step, current, total);
	}

	public void FinishUpdate()
	{
		//EditorUtility.ClearProgressBar();
		if (_importFinish != null)
			_importFinish();
	}

	void ImportFinish()
	{
		if (_importFinish != null)
			_importFinish();
		_importer.cleanArtifacts();
	}
	*/
}

