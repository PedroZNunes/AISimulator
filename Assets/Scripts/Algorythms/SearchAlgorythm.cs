﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingAlgorythm {

    public delegate void UpdateQueueHandler (int queueSize, float  length);
    public static event UpdateQueueHandler UpdateUIEvent;

    public static event Action IncrementEnqueueEvent;

    public static event Action ResetUIEvent;

    public Dictionary<Node, Node> cameFrom { get; protected set; }

    public static bool IsSearching { get; protected set; }

    public virtual IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) { return null; }

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