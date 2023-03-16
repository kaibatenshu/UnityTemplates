using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCanvas : MonoBehaviour
{
    [SerializeField] private Dialog_Alert alertDialog;
    [SerializeField] private Dialog_Loading loadingDialog;
    [SerializeField] private Dialog_YesNo_Input inputDialog;
    [SerializeField] private Dialog_YesNo_Text textDialog;



    public void ShowLoading(bool touchClose)
    {

    }
    public void ShowAlertDialog(){

    }


    // Start is called before the first frame update
    void Start()
    {
        ins = this;
        DontDestroyOnLoad(ins);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void trace()
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    }

    private static GlobalCanvas ins = null;
    public static GlobalCanvas instance { get { return ins; } }
}
