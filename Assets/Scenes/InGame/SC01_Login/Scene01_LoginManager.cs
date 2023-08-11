using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene01_LoginManager : MonoBehaviour{


    public void onButtonLoginWithGoogle() {
        #if UNITY_EDITOR
        string webClientId = "359259691495-7fr928042n33gc1q7dc3dt1hkvr6m7g9.apps.googleusercontent.com";
        string webClientSecret = "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";
        string redirect_uri = "https://backendgame.com/GoogleSignIn.html";
        GlobalCanvas.instance.UnityEditorLoginGoogle(
            webClientId,
            webClientSecret,
            redirect_uri,
            (id_token) => {
                Debug.Log("Login success and show Json Info");
                Application.OpenURL("https://oauth2.googleapis.com/tokeninfo?id_token="+id_token);
            },
            (errorString) => { 
                Debug.LogError(errorString);
            }
            );
        #elif UNITY_ANDROID

        #elif UNITY_IOS

        #else

        #endif

    }


    public void onClickButton() {
        GlobalCanvas.instance.ShowAlertDialog("aaaaaaaaabbbbbbbbbbbbbbbbbbbb");
    }
}
