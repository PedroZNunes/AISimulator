using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SearchAlgorythm {

    public delegate void UpdateQueueHandler (int queueSize);
    public static event UpdateQueueHandler UpdateQueueEvent;

    public delegate void UpdatePathLangthHandler (float pathLangth);
    public static event UpdatePathLangthHandler UpdatePathLength;

    public static event Action IncrementEnqueueEvent;

    public static event Action ResetUIEvent;

    public Dictionary<Node, Node> cameFrom { get; protected set; }

    public virtual IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond, bool trackVisitedNodes) { return null; }

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

    protected void UIUpdateQueueSize (int size) {
        if (UpdateQueueEvent != null)
            UpdateQueueEvent (size);
    }

    protected void UIUpdatePathLength (float pathLength) {
        if (UpdatePathLength != null)
            UpdatePathLength (pathLength);

    }

}