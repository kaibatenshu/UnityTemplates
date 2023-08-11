using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene00_SplashManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickButtonTest() {
        GlobalCanvas.instance.ShowAlertDialog("change to Login Scene", "Đồng ý", false, () => {
            SceneManager.LoadScene("Scene01_Login");
        });
        //GlobalCanvas.instance.trace();
    }
}
