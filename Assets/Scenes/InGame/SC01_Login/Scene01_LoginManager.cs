using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene01_LoginManager : MonoBehaviour{


    public void onButtonLoginWithGoogle() {
        #if UNITY_EDITOR
        string webClientId = "500518206021-aclbgjoamh3el2g50r7v1sn4aqe63nci.apps.googleusercontent.com";
        string webClientSecret = "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";
        string redirect_uri = "https://backendgame.com/GoogleSignIn.html";
        GlobalCanvas.instance.UnityEditorLoginGoogle(
            webClientId,
            webClientSecret,
            redirect_uri,
            () => { 
            
            },
            (errorString) => { 
            
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
