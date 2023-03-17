using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkGlobal : MonoBehaviour{
    private System.Object syncLock;
    private Action actionUpdateUI;
    public void setUpdateUI(Action _action){
        new Thread(new ThreadStart(()=>{
            lock(syncLock){
                while(actionUpdateUI!=null)
                    Thread.Sleep(1);
                actionUpdateUI=_action;
            }
        })).Start();
    }
    IEnumerator doActionUIGame(Action _action) { _action(); yield break; }

    // Start is called before the first frame update
    void Start(){
        syncLock = new System.Object();
    }

    // Update is called once per frame
    void Update(){
        if(actionUpdateUI!=null){
            StartCoroutine(doActionUIGame(actionUpdateUI));
            actionUpdateUI=null;
        }
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
