using UnityEngine;
using UnityEngine.UI;

public class TreeUINode : MonoBehaviour {

    public int Value { get; private set; }

    private Text label;

    private void Awake () {
        label = GetComponentInChildren<Text> ();       
    }

    public void AssignScore (int? value) {
        string text;
        if (value.HasValue)
            text = value.Value.ToString ();
        else
            text = "";

        label.text = text;
    }

}
