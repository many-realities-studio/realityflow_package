/*
 * Copyright(c) 2017-2018 Sketchfab Inc.
 * License: https://github.com/sketchfab/UnityGLTF/blob/master/LICENSE
 */
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Ionic.Zip;

public class SketchfabImporter
{
    private readonly List<string> _unzippedFiles;
    private GameObject ActiveObject;

    // Settings
    private readonly string _unzipDirectory = Application.temporaryCachePath + "/unzip";
    private string _importDirectory = Application.dataPath + "/Import";
    private string _currentSampleName = "Imported";
    private bool _addToCurrentScene = false;
    private string _gltfInput;

    public SketchfabImporter(/*GLTFEditorImporter.ProgressCallback progressCallback, GLTFEditorImporter.RefreshWindow finishCallback*/)
    {
        _unzippedFiles = new List<string>();
    }

    public void configure(string importDirectory, string prefabName, bool addToScene = false)
    {

        if (importDirectory.Length > 0)
        {
            // if (!GLTFUtils.isFolderInProjectDirectory(importDirectory))
            // {
            //     Debug.LogError("Import directory in not in Assets");
            // }
            // else
            // {
            //     _importDirectory = importDirectory;
            // }

            _importDirectory = importDirectory;
        }

        if (prefabName.Length > 0)
        {
            _currentSampleName = prefabName;
        }

        _addToCurrentScene = addToScene;
    }

    private string findGltfFile(string directory)
    {
        string gltfFile = "";
        DirectoryInfo info = new DirectoryInfo(directory);
        foreach (FileInfo fileInfo in info.GetFiles())
        {
            _unzippedFiles.Add(fileInfo.FullName);
            if (isSupportedFile(fileInfo.FullName))
            {
                gltfFile = fileInfo.FullName;
            }
        }

        return gltfFile;
    }

    private void deleteExistingGLTF()
    {
        string gltfFile = findGltfFile(_unzipDirectory);
        if (gltfFile != "")
        {
            File.Delete(gltfFile);
        }
    }

    private string unzipGltfArchive(string zipPath)
    {
        if (!Directory.Exists(_unzipDirectory))
        {
            _ = Directory.CreateDirectory(_unzipDirectory);
        }
        else
        {
            deleteExistingGLTF();
        }

        // Extract archive
        ZipFile zipfile = ZipFile.Read(zipPath);

        foreach (ZipEntry e in zipfile)
        {
            // check if you want to extract e or not
            _unzippedFiles.Add(_unzipDirectory + "/" + e.FileName);
            e.Extract(_unzipDirectory, ExtractExistingFileAction.OverwriteSilently);
        }


        return findGltfFile(_unzipDirectory);
    }

    private string unzipGLTFArchiveData(byte[] zipData)
    {
        if (!Directory.Exists(_unzipDirectory))
        {
            _ = Directory.CreateDirectory(_unzipDirectory);
        }
        else
        {
            deleteExistingGLTF();
        }

        MemoryStream stream = new MemoryStream(zipData);
        ZipFile zipfile = ZipFile.Read(stream);
        foreach (ZipEntry e in zipfile)
        {
            // check if you want to extract e or not
            _unzippedFiles.Add(_unzipDirectory + "/" + e.FileName);
            e.Extract(_unzipDirectory, ExtractExistingFileAction.OverwriteSilently);
        }

        return findGltfFile(_unzipDirectory);
    }

    private string stripProjectDirectory(string directory)
    {
        return directory.Replace(Application.dataPath, "Assets");
    }

    public void loadFromBuffer(byte[] data)
    {
        // if (!GLTFUtils.isFolderInProjectDirectory(_importDirectory))
        // {
        //     Debug.LogError("Import directory is outside of project directory. Please select path in Assets/");
        //     return;
        // }

        if (!Directory.Exists(_importDirectory))
        {
            _ = Directory.CreateDirectory(_importDirectory);
        }

        _gltfInput = unzipGLTFArchiveData(data);

        ActiveObject = new GameObject();

        UnityGLTF.GLTFComponent gltfComponent = ActiveObject.AddComponent<UnityGLTF.GLTFComponent>();
        gltfComponent.Multithreaded = true;
        gltfComponent.UseStream = true;
        gltfComponent.AppendStreamingAssets = false;
        gltfComponent.GLTFUri = _gltfInput;
        gltfComponent.name = _currentSampleName;
    }

    private bool isSupportedFile(string filepath)
    {
        string ext = Path.GetExtension(filepath);
        return ext == ".gltf" || ext == ".glb";
    }

    public void loadFromFile(string filepath)
    {
        _gltfInput = filepath;
        if (Path.GetExtension(filepath) == ".zip")
        {
            _gltfInput = unzipGltfArchive(filepath);
        }

        if (!isSupportedFile(_gltfInput))
        {
            //EditorUtility.DisplayDialog("Import Failed", "No glTF data found", "OK");
            return;
        }

        if (!Directory.Exists(_importDirectory))
        {
            _ = Directory.CreateDirectory(_importDirectory);
        }

        // _importer.setupForPath(_gltfInput, _importDirectory, _currentSampleName, _addToCurrentScene);
        // _importer.Load();
    }

    // public void cleanArtifacts()
    // {
    //     GLTFUtils.removeFileList(_unzippedFiles.ToArray());
    // }

    // public void OnDestroy()
    // {
    //     GLTFUtils.removeFileList(_unzippedFiles.ToArray());
    //     GLTFUtils.removeEmptyDirectory(_unzipDirectory);
    // }
}
