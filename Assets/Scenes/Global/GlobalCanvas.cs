
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GlobalCanvas : MonoBehaviour{
    public void ShowLoading(bool touchClose)
    {

    }

    public void ShowAlertDialog(string alert, string textButton="Ok", UnityAction onClose = null){ShowAlertDialog(alert, textButton, false, onClose);}
    public void ShowAlertDialog(string alert, string textButton, bool touchOutForClose, UnityAction onClose=null){
        GameObject _alertDialog = Instantiate(PrefabUtility.LoadPrefabContents("Assets/Scenes/Global/Dialog/Alert/Dialog_Alert.prefab"), GameObject.Find("Canvas").transform);
        _alertDialog.GetComponent<Dialog_Alert>().onClose = onClose;
        _alertDialog.GetComponent<Dialog_Alert>().touchOutForClose = touchOutForClose;
        _alertDialog.GetComponent<Dialog_Alert>().setup(alert, textButton);
        _alertDialog.name = "AlertDialog";
    }

    public void UnityEditorLoginGoogle(string _webClientId,string _webClientSecret, string _redirect_uri, UnityAction onSuccess,UnityAction<string> onError) {
        #if TEST
        Debug.Log("Link login : <color=gray>https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=profile%20email&redirect_uri=" + _redirect_uri + "&client_id=" + _webClientId + "</color>");
        #endif
        Application.OpenURL("https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=profile%20email&redirect_uri=" + _redirect_uri + "&client_id=" + _webClientId);
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
