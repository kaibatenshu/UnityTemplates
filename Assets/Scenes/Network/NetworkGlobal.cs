using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGlobal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private static NetworkGlobal ins = null;
    public static NetworkGlobal instance{
        get{
            if (ins == null){
                GameObject go = new GameObject();
                ins = go.AddComponent<NetworkGlobal>();
                go.name = ins.GetType().Name;
                DontDestroyOnLoad(ins);
            }
            return ins;
        }
    }
}
