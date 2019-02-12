using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingAlgorythm {

    public delegate void UpdateQueueHandler (int queueSize, float  length);
    static public event UpdateQueueHandler UpdateUIEvent;

    static public event Action IncrementEnqueueEvent;

    static public event Action ResetUIEvent;

    static public event Action SearchCompletedEvent;


    public Dictionary<Node, Node> cameFrom { get; protected set; }

    static public bool IsSearching { get; protected set; }

    public virtual IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) { return null; }

    public void StopSearch() {
        IsSearching = false;
    }

    protected void SearchComplete (bool hasPath) {
        IsSearching = false;

        if (!hasPath) {
            Debug.Log("No path exists.");
        }
        else {
            Debug.Log("Path complete.");
        }

        if (SearchCompletedEvent != null)
            SearchCompletedEvent();
    }


    protected float CalculatePathLength (Node start, Node current) {
        float pathLength = 0;
        while (current != start) {
            Node next = cameFrom[current];
            pathLength += Mathf.Abs (Vector2.Distance (current.pos, next.pos));
            current = next;
        }

        return pathLength;
    }

    public void ResetUI () {
        if (ResetUIEvent != null)
            ResetUIEvent ();
    }

    protected void UIIncrementEnqueuings () {
        if (IncrementEnqueueEvent != null)
            IncrementEnqueueEvent ();
    }

    protected void UIUpdate (int queueSize, float pathLength) {
        if (UpdateUIEvent != null)
            UpdateUIEvent (queueSize, pathLength);
    }
}