using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : SingletonDontDestroy<PoolManager>
{

    Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public override void OnReset()
    {
        foreach (var objlist in pools.Values)
            foreach (var obj in objlist)
                obj.SetActive(false);
    }

    public override void OnCreate()
    {
        SceneManager.sceneLoaded += PoolRemove;
    }
    public void PoolRemove(Scene scene, LoadSceneMode loadSceneMode)
    {
        OnReset();
    }

    public void AddPooling(GameObject origin, Transform parent)
    {
        if (!pools.ContainsKey(origin))
        {
            pools.Add(origin, new List<GameObject>());
        }
        foreach(Transform trans in parent)
        {
            GameObject obj = trans.gameObject;
            if (obj != origin)
                pools[origin].Add(obj);
        }
    }
    public GameObject Init(GameObject origin, float duration = -1)
    {
        if (origin != null)
        {
            GameObject copy = null;
            if (pools.ContainsKey(origin))
            {
                if (pools[origin].FindAll((GameObject x) => !x.activeSelf).Count > 0)
                {
                    copy = pools[origin].Find((GameObject x) => !x.activeSelf);
                    copy.SetActive(true);
                    if (duration != -1)
                    {
                        AutoDestruct autoDestruct = copy.GetComponent<AutoDestruct>();
                        if (autoDestruct != null)
                            autoDestruct.duration = duration;
                        else
                            copy.AddComponent<AutoDestruct>().duration = duration;
                    }
                    return copy;
                }
            }
            else
            {
                pools.Add(origin, new List<GameObject>());
            }
            copy = Instantiate(origin);
            pools[origin].Add(copy);
            copy.SetActive(true);
            DontDestroyOnLoad(copy);
            if (duration != -1)
            {
                AutoDestruct autoDestruct = copy.GetComponent<AutoDestruct>();
                if (autoDestruct != null)
                    autoDestruct.duration = duration;
                else
                    copy.AddComponent<AutoDestruct>().duration = duration;
            }
            return copy;
        }
        return null;
    }
}
