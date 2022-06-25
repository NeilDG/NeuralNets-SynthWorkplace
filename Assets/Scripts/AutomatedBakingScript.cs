using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


public class AutomatedBakingScript : MonoBehaviour
{
    private static int index = 193;
    private static int requiredBakes;
    private static string[] assetPaths;
    private static LightingSettings lightingSettings;

    private static int pngSceneIndex = 0;

    static string[] PopulateSkyboxes()
    {
        string[] guids = AssetDatabase.FindAssets("equirect t:material");
        string[] assetPaths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }
        
        return assetPaths;
    }

    static LightingSettings SetDebugSettings()
    {
        LightingSettings defaultSettings = new LightingSettings();
        defaultSettings.directSampleCount = 4;
        defaultSettings.indirectSampleCount = 4;
        defaultSettings.environmentSampleCount = 4;
        defaultSettings.lightmapMaxSize = 1024;
        defaultSettings.lightmapResolution = 2.0f;
        defaultSettings.prioritizeView = false;
        defaultSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

        return defaultSettings;
    }

    static LightingSettings SetProductionSettings()
    {
        LightingSettings defaultSettings = new LightingSettings();
        defaultSettings.directSampleCount = 1024;
        defaultSettings.indirectSampleCount = 1024;
        defaultSettings.environmentSampleCount = 1024;
        defaultSettings.lightmapMaxSize = 1024;
        defaultSettings.lightmapResolution = 8.0f;
        defaultSettings.prioritizeView = false;
        defaultSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

        return defaultSettings;
    }

    [MenuItem("Window/Automated Baking/Save Skyboxes")]
    static void SaveSkyboxes()
    {
        assetPaths = PopulateSkyboxes();
    }

    [MenuItem("Window/Automated Baking/Create Scene Copies")]
    static void SaveSceneCopies()
    {
        assetPaths = PopulateSkyboxes();
        requiredBakes = assetPaths.Length;
        //requiredBakes = 20;

        for (int i = index; i < requiredBakes; i++)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[i]);
            string sceneName = "Scene_" + i.ToString() + "_" + skyboxMat.name + ".unity";
            //string scenePath = Application.dataPath + "/Scenes/Skyboxes/" + sceneName;
            string scenePath = Application.dataPath + "/Scenes/Skyboxes_NoShadows/" + sceneName;
            bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
        }

        Debug.Log("Saved " +requiredBakes+ " scene copies.");
        
    }

    [MenuItem("Window/Automated Baking/Start Automated Baking")]
    static void StartAutomatedBaking()
    {
        assetPaths = PopulateSkyboxes();
        requiredBakes = assetPaths.Length;
        //requiredBakes = 20;

        Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[index]);
        RenderSettings.skybox = skyboxMat;

        Lightmapping.bakeStarted += OnBakeStarted;
        Lightmapping.bakeCompleted += OnBakeCompleted;

        string sceneName = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name + ".unity";
        //string scenePath = Application.dataPath + "/Scenes/Skyboxes/" + sceneName;
        string scenePath = Application.dataPath + "/Scenes/Skyboxes_NoShadows/" + sceneName;
        bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);

        Lightmapping.Clear();
        Lightmapping.ClearDiskCache();
        Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);
        EditorSceneManager.OpenScene(scenePath);
        Lightmapping.BakeAsync();

    }


    static void OnBakeStarted()
    {
        string sceneName = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name + ".unity";
        //string scenePath = Application.dataPath + "/Scenes/Skyboxes/" + sceneName;
        string scenePath = Application.dataPath + "/Scenes/Skyboxes_NoShadows/" + sceneName;

        //bool success = Lightmapping.TryGetLightingSettings(out lightingSettings);
        //lightingSettings = SetDebugSettings();
        lightingSettings = SetProductionSettings();
        Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);

        Debug.Log("Bake started for scene:" + sceneName);
        LightingSettings currentSettings = Lightmapping.GetLightingSettingsForScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Direct samples: " + currentSettings.directSampleCount);
        Debug.Log("Indirect samples: " + currentSettings.indirectSampleCount);
        Debug.Log("Environment samples: " + currentSettings.environmentSampleCount);
        Debug.Log("Baking device: " + currentSettings.lightmapper.ToString());
        Debug.Log("Skybox name: " + RenderSettings.skybox.name);
        
    }

    static void OnBakeCompleted()
    {
        string sceneName = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name + ".unity";
        Debug.Log("Bake completed. Saved scene: " +sceneName);

        //start next bake
        index++;
        if (index < requiredBakes)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[index]);
            RenderSettings.skybox = skyboxMat;

            Debug.Log("Baking next skybox.");
            Debug.Log("Direct samples: " + lightingSettings.directSampleCount);
            Debug.Log("Indirect samples: " + lightingSettings.indirectSampleCount);
            Debug.Log("Environment samples: " + lightingSettings.environmentSampleCount);
            Debug.Log("Baking device: " + lightingSettings.lightmapper.ToString());
            Debug.Log("Skybox name: " + RenderSettings.skybox.name);

            sceneName = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name + ".unity";
            //string scenePath = Application.dataPath + "/Scenes/Skyboxes/" + sceneName;
            string scenePath = Application.dataPath + "/Scenes/Skyboxes_NoShadows/" + sceneName;
            bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
            if (result)
            {
                Lightmapping.Clear();
                Lightmapping.ClearDiskCache();
                Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);
                EditorSceneManager.OpenScene(scenePath);
                Lightmapping.BakeAsync();
            }
            else
            {
                Debug.LogError("An error occurred when creating a scene copy.");
            }

        }

        if (index == requiredBakes)
        {
            Debug.Log("All bake completed");
            /*index = 0;
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);*/
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
