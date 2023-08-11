using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog_Alert : MonoBehaviour{
    [SerializeField] private Text textInfo;
    [SerializeField] private Button buttonOk;

    public UnityAction onClose;
    public bool touchOutForClose;

    public void setup(string _textInfo,string _textButton) {
        textInfo.text = _textInfo;
        buttonOk.GetComponentInChildren<Text>().text = _textButton;
    }

    public void onPanel() {
        if (touchOutForClose && onClose != null){
            onClose();
            Destroy(gameObject);
        }
    }
    public void onButton() {
        if (onClose != null)
            onClose();
        Destroy(gameObject);
    }
}
