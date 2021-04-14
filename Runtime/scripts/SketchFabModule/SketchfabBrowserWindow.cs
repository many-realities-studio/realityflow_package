/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Specialized;
using TMPro;

public class SketchfabBrowserWindow : MonoBehaviour
{
    enum SEARCH_IN
    {
        ALL_FREE_DOWNLOADABLE=0,
        MY_MODELS = 1,
        MY_STORE_PURCHASES=2
    }
    public List<GameObject> modelsButtons;
    public GameObject ModelButton;

    public GameObject modelResultsPanel;
    private bool reloadSearch = true;
    public TMP_InputField searchBox;

    // Sketchfab elements
    public SketchfabBrowserManager _browserManager;
    public SketchfabLogger _logger;

    public SketchfabImporter _importer;

    int _thumbnailSize = 128;
    Vector2 _scrollView = new Vector2();
    int _licenseIndex;
    int _categoryIndex;
    int _sortByIndex;
    int _polyCountIndex;
    SEARCH_IN _searchInIndex = SEARCH_IN.ALL_FREE_DOWNLOADABLE;

    // Upload params and options
    string[] _categoriesNames;

    // Exporter UI: dynamic elements
    string _currentUid = "";

    // Search parameters
    string[] _sortBy;
    string[] _polyCount;
    string[] _searchIn;
    string[] _license;
    string _query = "";
    bool _animated = false;
    bool _staffpicked = true;
    string _categoryName = "";

    float framesSinceLastSearch = 0.0f;
    float nbFrameSearchCooldown = 30.0f;

    void Start()
    {
        searchBox = (TMP_InputField)FindObjectOfType(typeof(TMP_InputField));
        checkValidity();
    }
    void OnEnable()
    {
        SketchfabPlugin.Initialize();
        _searchInIndex = SEARCH_IN.ALL_FREE_DOWNLOADABLE;
    }

    private void checkValidity()
    {
        if (_browserManager == null)
        {
            _browserManager = new SketchfabBrowserManager(OnRefreshUpdate, true);
            resetFilters();
            _currentUid = "";
            _categoryName = "";
            _categoriesNames = new string[0];

            // Setup sortBy
            _sortBy = new string[] { "Relevance", "Likes", "Views", "Recent" };
            _polyCount = new string[] { "Any", "Up to 10k", "10k to 50k", "50k to 100k", "100k to 250k", "250k +" };
            _searchIn = new string[] { "free downloadable", "my models", "store purchases" };
            _license = new string[] { "any", "CC BY", "CC BY SA", "CC BY-ND", "CC BY-NC", "CC BY-NC-SA", "CC BY-NC-ND", "CC0" }; // No search for store models so only CC licenses here

            //GL.sRGBWrite = true;
        }
        if(_importer == null)
        {
            _importer = new SketchfabImporter();
        }

        SketchfabPlugin.checkValidity();
        _logger = SketchfabPlugin.getLogger();
    }

    private void Update()
    {
        if (_browserManager != null)
        {
            _browserManager.Update();
            if (_categoriesNames.Length == 0 && _browserManager.getCategories().Count > 0)
            {
                //Debug.Log("The categories exists");
                _categoriesNames = _browserManager.getCategories().ToArray();
            }
            if (_browserManager.hasResults() /* && _browserManager.getResults()[0]._preview == null*/)
            {
                _browserManager.fetchModelPreview();
                displayResults();
            }

            framesSinceLastSearch++;
        }
    }

    public void triggerSearch()
    {
        reloadSearch = true;
        _query = searchBox.text.ToString();
        Debug.Log(_query);
        if (framesSinceLastSearch < nbFrameSearchCooldown)
            return;

        string licenseSmug;
        switch (_licenseIndex)
        {
            case 0:
                licenseSmug = "";
                break;
            case 1:
                licenseSmug = "by";
                break;
            case 2:
                licenseSmug = "by-sa";
                break;
            case 3:
                licenseSmug = "by-nd";
                break;
            case 4:
                licenseSmug = "by-nc";
                break;
            case 5:
                licenseSmug = "by-nc-sa";
                break;
            case 6:
                licenseSmug = "by-nc-nd";
                break;
            case 7:
                licenseSmug = "cc0";
                break;
            default:
                licenseSmug = "";
                break;
        }

        SORT_BY sort;
        switch (_sortByIndex)
        {
            case 0:
                sort = SORT_BY.RELEVANCE;
                break;
            case 1:
                sort = SORT_BY.LIKES;
                break;
            case 2:
                sort = SORT_BY.VIEWS;
                break;
            case 3:
                sort = SORT_BY.RECENT;
                break;
            default:
                sort = SORT_BY.RELEVANCE;
                break;
        }
        // Point clouds are not supported in Unity so check that polycount is not 0
        // here. It won't prevent model that are parially point clouds but it's better
        // than nothing
        string _minFaceCount = "1";
        string _maxFaceCount = "";
        switch(_polyCountIndex)
        {
            case 0:
                break;
            case 1:
                _maxFaceCount = "10000";
                break;
            case 2:
                _minFaceCount = "10000";
                _maxFaceCount = "50000";
                break;
            case 3:
                _minFaceCount = "50000";
                _maxFaceCount = "100000";
                break;
            case 4:
                _minFaceCount = "100000";
                _maxFaceCount = "250000";
                break;
            case 5:
                _minFaceCount = "250000";
                break;
        }

        SEARCH_ENDPOINT endpoint = SEARCH_ENDPOINT.DOWNLOADABLE;
        switch (_searchInIndex)
        {
            case SEARCH_IN.MY_MODELS:
                endpoint = SEARCH_ENDPOINT.MY_MODELS;
                break;
            case SEARCH_IN.MY_STORE_PURCHASES:
                endpoint = SEARCH_ENDPOINT.STORE_PURCHASES;
                break;
            default:
                endpoint = SEARCH_ENDPOINT.DOWNLOADABLE;
                break;
        }

        _browserManager.search(_query, _staffpicked, _animated, _categoryName, licenseSmug, _maxFaceCount, _minFaceCount, endpoint, sort);
        framesSinceLastSearch = 0.0f;
    }

    void displayResults()
    {
        if(reloadSearch)
        {
            OrderedDictionary models = _browserManager.getResults();
            int row = -900;
            int column = -450;
            int maxWidth = 900;
            int maxHeight = 450;

            foreach(GameObject modelButton in modelsButtons)
            {
                Destroy(modelButton);
            }
            modelsButtons.Clear();

            if (models != null && models.Count > 0) // Replace by "is ready"
            {
                if(modelsButtons == null) 
                {
                    modelsButtons = new List<GameObject>();
                }
                else 
                {
                    modelsButtons.Clear();
                }

                //for(int i = 0; i < models.Count; i++)
                foreach(SketchfabModel model in models.Values)
                {
                     // Layout
                    if(row % maxWidth == 0)
                    {
                        column += 150;
                        row = 0;
                    }
                    row += 150;
                    StartCoroutine(CreatedModelGameObject(model, row, column));
                   
                }
                reloadSearch = false;
            }
            else if (_browserManager._isFetching)
            {
            Debug.Log("Fetching models ....");
            }
        }
    }

    private IEnumerator CreatedModelGameObject(SketchfabModel model, int row, int column)
    {
        while (model._preview == null)
        {
            yield return null;
        }
        Sprite modelImage;
        GameObject modelButton = Instantiate(ModelButton, modelResultsPanel.transform, false);
        modelButton.transform.localPosition = new Vector3(row, column, 0);
        modelButton.name = model.name;
        modelButton.GetComponent<Model>().sketchFabModel = model;
        modelButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = model.author;
        modelImage = Sprite.Create(model._preview, new Rect(0.0f, 0.0f, model._preview.width, model._preview.height), new Vector2(0.5f, 0.5f), 100.0f);
        modelButton.GetComponent<Image>().sprite = modelImage;
        
    
        // Add to list of GameObjects
        modelsButtons.Add(modelButton);
    }

    void resetFilters()
    {
        _licenseIndex = 0;
        _categoryIndex = 0;
        _sortByIndex = 3;
        _polyCountIndex = 0;

        _query = "";
        _animated = false;
        _staffpicked = true;
        _categoryName = "All";
    }

    void resetFilersOwnModels()
    {
        _licenseIndex = 0;
        _categoryIndex = 0;
        _sortByIndex = 3;
        _polyCountIndex = 0;

        _query = "";
        _animated = false;
        _staffpicked = false;
        _categoryName = "All";
    }
    private void OnRefreshUpdate()
    {
        Debug.Log("Reload");
    }

}