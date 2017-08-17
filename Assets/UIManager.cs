using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Text enqueuingsValue;

    [SerializeField]
    private Text maxQueueSizeValue;

    [SerializeField]
    private Text queueSizeValue;

    [SerializeField]
    private Text pathLengthValue;

    [SerializeField]
    private Text nodesExpandedValue;

    private void Awake () {
        ResetUI ();
    }

    //events
    private void OnEnable () {
        SearchAlgorythm.UpdateUIEvent += UpdateUI;
        SearchAlgorythm.IncrementEnqueueEvent += IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent += ResetUI;
    }
    private void OnDisable () {
        SearchAlgorythm.UpdateUIEvent -= UpdateUI;
        SearchAlgorythm.IncrementEnqueueEvent -= IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent -= ResetUI;
    }

    private void UpdateUI (int queueSize, float length) {
        //update queue info
        if (queueSize > (Int32.Parse (maxQueueSizeValue.text))) {
            maxQueueSizeValue.text = queueSize.ToString();
        }

        queueSizeValue.text = queueSize.ToString();

        //assign path length
        pathLengthValue.text = String.Format ("{0:0.00}", length);

        //increment nodes expanded
        nodesExpandedValue.text = (Int32.Parse (nodesExpandedValue.text) + 1).ToString ();
    }

    private void IncrementEnqueue () {
        enqueuingsValue.text = (Int32.Parse (enqueuingsValue.text) + 1).ToString();
    }
    
    private void ResetUI () {
        enqueuingsValue.text = "0";
        maxQueueSizeValue.text = "0";
        queueSizeValue.text = "0";
        pathLengthValue.text = "0";
        nodesExpandedValue.text = "0";
    }
}
