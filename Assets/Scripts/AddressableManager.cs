using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject characterObj;

    private List<GameObject> gameObjects = new List<GameObject>();

    private void Start()
    {
        Button_SpawnObject();
    }

    public void Button_SpawnObject()
    {
        characterObj.InstantiateAsync().Completed += (obj) =>
        {
            gameObjects.Add(obj.Result);
        };
    }

    public void Button_Release()
    {
        if (gameObjects.Count == 0) return;
        var idx = gameObjects.Count - 1;
        Addressables.ReleaseInstance(gameObjects[idx]);
    }
}
