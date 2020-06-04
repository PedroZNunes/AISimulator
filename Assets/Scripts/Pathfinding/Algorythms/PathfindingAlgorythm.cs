using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingAlgorythm {

    public delegate void UpdateQueueHandler( int queueSize, float length, float addInfoValue = 0f );
    static public event UpdateQueueHandler UpdateUIEvent;

    static public event Action IncrementEnqueueEvent;

    static public event Action AlteredSearchStateEvent;


    public Dictionary<Node, Node> cameFrom { get; protected set; }

    static private bool isSearching = false;
    static public bool IsSearching {
        get { return isSearching; }
        protected set {
            isSearching = value;

            if (AlteredSearchStateEvent != null)
                AlteredSearchStateEvent();
        }
    }


    public virtual IEnumerator Search( List<Node> nodes, int size, Node start, Node goal, int framesPerSecond ) { return null; }

    public void CancelSearch() {
        IsSearching = false;
    }

    protected void SearchComplete( bool hasPath ) {
        IsSearching = false;

        if (!hasPath) {
            Debug.Log( "No path exists." );
        }
        else {
            Debug.Log( "Path complete." );
        }
    }


    protected float CalculatePathLength( Node start, Node current ) {
        float pathLength = 0;
        while (current != start) {
            Node next = cameFrom[current];
            pathLength += Mathf.Abs( Vector2.Distance( current.pos, next.pos ) );
            current = next;
        }

        return pathLength;
    }

    protected void UIIncrementEnqueuings() {
        if (IncrementEnqueueEvent != null)
            IncrementEnqueueEvent();
    }

    protected void UIUpdate( int queueSize, float pathLength, float addInfoValue = 0f ) {
        if (UpdateUIEvent != null)
            UpdateUIEvent( queueSize, pathLength, addInfoValue );
    }

}