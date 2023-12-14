using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableLoader : MonoBehaviour
{
    public GameObject _objectInstance;
    public bool _sceneInstance = false;
    private SceneInstance sceneInstance;




    private void Start()
    {
        Addressables.InitializeAsync().Completed += (d) => { Debug.Log("Addressables InitializeAsync"); };
    }

    public string _key;


    [ContextMenu("DownloadBundle")]
    public void DownloadBundle() => StartCoroutine(Co_DownloadBundle());



    public IEnumerator Co_DownloadBundle()
    {
        //string key = "assetKey";
        //Clear all cached AssetBundles
        // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
        Addressables.ClearDependencyCacheAsync(_key);

        //Check the download size
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(_key);
        yield return getDownloadSize;

        //If the download size is greater than 0, download all the dependencies.
        if (getDownloadSize.Result > 0)
        {
            AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(_key);
            yield return downloadDependencies;

            if (downloadDependencies.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.LoadSceneAsync("NewYork.unity", LoadSceneMode.Additive);
            }
            else Debug.Log(downloadDependencies.Status);
        }

        else Debug.Log("DownloadSize < 0");
    }


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    if (_objectInstance == null)
        //    {
        //        Debug.Log("Object Intantiate");
        //        asset.InstantiateAsync().Completed += OnComplete;
        //    }
        //    else
        //    {
        //        Debug.Log("ReleaseInstance Object");
        //        asset.ReleaseInstance(_objectInstance);
        //    }
        //}
        //else
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!_sceneInstance)
            {
                Debug.Log("Scene Intantiate");
                Addressables.LoadSceneAsync(_key, LoadSceneMode.Additive, true).Completed += OnCompleteScene;
                //assetScene.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnCompleteScene;

            }
            else
            {
                Debug.Log("ReleaseInstance Scene");
                Addressables.UnloadSceneAsync(sceneInstance).Completed += (s) => { _sceneInstance = false; };
                //assetScene.UnLoadScene().Completed += (s) => { assetScene.ReleaseAsset(); ; _sceneInstance = false; };
            }
        }
    }

    private void OnCompleteScene(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene Succeeded");
            _sceneInstance = true;
            sceneInstance = obj.Result;
        }
    }

    private void OnComplete(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Object Succeeded");
            _objectInstance = obj.Result;
        }
    }
}