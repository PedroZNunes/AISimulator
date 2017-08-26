using UnityEngine;
using UnityEngine.UI;
using System;

public class UITreeNode : MonoBehaviour {

    public int Value { get; private set; }

    private Text label;

    private void Awake () {
        label = GetComponentInChildren<Text> ();       
    }

    public void AssignValue (int value) {
        Value = value;
        label.text = Value.ToString ();
    }

}
