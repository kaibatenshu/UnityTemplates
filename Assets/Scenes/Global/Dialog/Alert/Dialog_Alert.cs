using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_Alert : MonoBehaviour{
    [SerializeField] private Text text;

    public Action onClose;

    void Start(){

    }

    public void onButtonClose() {
        Debug.Log("<color=orange>Dialog_Alert</color> -> button ");
        if (onClose != null)
            onClose();
        gameObject.SetActive(false);
    }
}
