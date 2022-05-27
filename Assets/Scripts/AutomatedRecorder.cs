using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomatedRecorder : MonoBehaviour
{
    private int index = 0;
    private static string[] assetPaths;
    [SerializeField] private GameObject cameraRecorder;

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

    // Start is called before the first frame update
    void Start()
    {
        assetPaths = PopulateSkyboxes();
        LoadNextScene();

        GameObject.DontDestroyOnLoad(this.gameObject);
        EventBroadcaster.Instance.AddObserver(EventNames.ON_RECORDING_FINISHED, OnRecordingFinished);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void LoadNextScene()
    {
        if (this.index > 0)
        {
            SceneManager.LoadScene(index);
        }
        else if (this.index < assetPaths.Length)
        {
            this.SceneManager_sceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.index++;
        GameObject.Instantiate(this.cameraRecorder);
    }

    private void OnRecordingFinished()
    {
        Debug.Log("Finished recording scene");
        this.LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
