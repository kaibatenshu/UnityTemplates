using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnityEditorLoginGoogle : MonoBehaviour{
    public InputField inputCode;
    public Button buttonLogin;

    public string webClientId;
    public string webClientSecret;
    public string redirect_uri;

    public UnityAction<string> onSuccess,onError;

    public void LoginWithGoogleCode() {
        if (string.IsNullOrEmpty(inputCode.text)) { 
            GlobalCanvas.instance.ShowAlertDialog("Please fill in  Google Code");
            if(onError != null)
                onError(null);
        } else { 
            StartCoroutine(parseGoogleCode(inputCode.text));
        }
    }



    IEnumerator parseGoogleCode(string googleCode){
        WWWForm form = new WWWForm();
        form.AddField("code", String.IsNullOrEmpty(googleCode) ? GUIUtility.systemCopyBuffer : googleCode);
        form.AddField("client_id", webClientId);
        form.AddField("client_secret", webClientSecret);
        form.AddField("redirect_uri", redirect_uri);
        form.AddField("grant_type", "authorization_code");

        UnityWebRequest www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success){
            JObject jsonResult = JObject.Parse(www.downloadHandler.text);
            string idToken = jsonResult["id_token"].ToString();
            string access_token = jsonResult["access_token"].ToString();
            onSuccess(idToken);
        }else{
            Debug.LogError(www.error);
            if (onError != null)
                onError(www.error);
        }
        Destroy(gameObject);
    }
}
