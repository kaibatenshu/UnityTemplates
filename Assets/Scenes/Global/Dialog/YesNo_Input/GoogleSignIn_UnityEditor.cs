using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GoogleSignIn_UnityEditor : MonoBehaviour{
    public const string webClientId = "500518206021-aclbgjoamh3el2g50r7v1sn4aqe63nci.apps.googleusercontent.com";
    public const string webClientSecret = "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";
    public const string redirect_uri = "https://backendgame.com/GoogleSignIn.html";
    public void openWeb() {
        Debug.Log("Link login : https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=profile%20email&redirect_uri=" + redirect_uri + "&client_id=" + webClientId);
        Application.OpenURL("https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=profile%20email&redirect_uri=" + redirect_uri + "&client_id=" + webClientId);
    }

    IEnumerator parseGoogleCode(string googleCode) { 
        Debug.Log("Login with Google code===>" + GUIUtility.systemCopyBuffer);

        WWWForm form = new WWWForm();
        form.AddField("code", String.IsNullOrEmpty(googleCode)? GUIUtility.systemCopyBuffer : googleCode);
        form.AddField("client_id", webClientId);
        form.AddField("client_secret", webClientSecret);
        form.AddField("redirect_uri", redirect_uri);
        form.AddField("grant_type", "authorization_code");

        UnityWebRequest www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success){
            Debug.LogError(www.error);
            gameObject.SetActive(false);
        }else{
            JObject jsonResult = JObject.Parse(www.downloadHandler.text);
            Debug.Log("??ng nh?p thành công : " + www.downloadHandler.text);
            string idToken = jsonResult["id_token"].ToString();
            string access_token = jsonResult["access_token"].ToString();

            Debug.Log("idToken : "+jsonResult["id_token"]);
            Application.OpenURL("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token="+jsonResult["id_token"]);
        }
    }

    // Start is called before the first frame update
    void Start(){


    }

    // Update is called once per frame
    void Update(){
        
    }
}

