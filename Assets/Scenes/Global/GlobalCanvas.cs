
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GlobalCanvas : MonoBehaviour{
    public void ShowLoading(bool touchClose)
    {

    }

    public void ShowAlertDialog(string alert, string textButton="Ok", UnityAction onClose = null){ShowAlertDialog(alert, textButton, false, onClose);}
    public void ShowAlertDialog(string alert, string textButton, bool touchOutForClose, UnityAction onClose=null){
        GameObject prefab = Instantiate(PrefabUtility.LoadPrefabContents("Assets/Scenes/Global/Dialog/Alert/Dialog_Alert.prefab"), GameObject.Find("Canvas").transform);
        prefab.GetComponent<Dialog_Alert>().onClose = onClose;
        prefab.GetComponent<Dialog_Alert>().touchOutForClose = touchOutForClose;
        prefab.GetComponent<Dialog_Alert>().setup(alert, textButton);
        prefab.name = "AlertDialog";
    }





    public void trace()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        ShowAlertDialog("bbbbbbbbbbbbbbbbbbbbbb");
    }

    private static GlobalCanvas ins = null;
    public static GlobalCanvas instance { get {
            if (ins == null){
                GameObject gameObject = new GameObject("GlobalCanvas");
                DontDestroyOnLoad (gameObject);
                ins = gameObject.AddComponent<GlobalCanvas>();
            }
            return ins; 
        } 
    }
}
