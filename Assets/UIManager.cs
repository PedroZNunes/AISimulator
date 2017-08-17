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

    private void Awake () {
        ResetUI ();
    }

    //events
    private void OnEnable () {
        SearchAlgorythm.UpdateQueueEvent += UpdateQueueSize;
        SearchAlgorythm.IncrementEnqueueEvent += IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent += ResetUI;
        SearchAlgorythm.UpdatePathLength += UpdatPathLength;
    }
    private void OnDisable () {
        SearchAlgorythm.UpdateQueueEvent -= UpdateQueueSize;
        SearchAlgorythm.IncrementEnqueueEvent -= IncrementEnqueue;
        SearchAlgorythm.ResetUIEvent -= ResetUI;
    }

    private void UpdateQueueSize (int queueSize) {
        if (queueSize > (Int32.Parse (maxQueueSizeValue.text))) {
            maxQueueSizeValue.text = queueSize.ToString();
        }

        queueSizeValue.text = queueSize.ToString();
    }

    private void IncrementEnqueue () {
        enqueuingsValue.text = (Int32.Parse (enqueuingsValue.text) + 1).ToString();
    }

    private void UpdatPathLength (float length) {
        pathLengthValue.text = String.Format ("{0:0.00}", length);
    }

    private void ResetUI () {
        enqueuingsValue.text = "0";
        maxQueueSizeValue.text = "0";
        queueSizeValue.text = "0";
        pathLengthValue.text = "0";
    }
}
