using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelManager : MonoBehaviour
{
    [Header("REFERENCE")]
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        Addressables.LoadAssetAsync<GameObject>("level1").Completed += OnLevelLoaded;
    }

    private void OnLevelLoaded(AsyncOperationHandle<GameObject> handle)
    {
        GameObject level = Instantiate(handle.Result, transform);
        level.name = handle.Result.name;

        gameManager.Init();
    }
}
