using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GraphSearchAlgorithm {

    public delegate void UpdateQueueHandler( int queueSize, float length, float addInfoValue = 0f );
    static public event UpdateQueueHandler UpdateUIEvent;

    static public event Action IncrementEnqueueEvent;

    static public event Action AlteredSearchStateEvent;

    /// <summary>
    /// Dictionary that lists each node in the current path in relation to their previous one, or if you may, where you 'came from' to get to this node
    /// </summary>
    public Dictionary<Node, Node> cameFrom { get; protected set; }

    /// <summary>
    /// Tracks if there's an ongoing search arlgorithm
    /// </summary>
    static private bool isSearching = false;
    static public bool IsSearching {
        get { return isSearching; }
        protected set {
            isSearching = value;

            if (AlteredSearchStateEvent != null)
                AlteredSearchStateEvent();
        }
    }

    /// <summary>
    /// Coroutine responsible for searching.
    /// </summary>
    /// <param name="nodes"> list of all nodes </param>
    /// <param name="size"> grid size </param>
    /// <param name="start"> start node </param>
    /// <param name="goal"> destination node </param>
    /// <param name="framesPerSecond"> animation speed in frames per second</param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds up every bit of path current active
    /// </summary>
    /// <param name="from"> Node you want to measure the path length from </param>
    /// <param name="to"> Node you want to get the length to </param>
    /// <returns></returns>
    protected float CalculatePathLength( Node from, Node to ) {
        float pathLength = 0;
        while (to != from) {
            Node next = cameFrom[to];
            pathLength += Mathf.Abs( Vector2.Distance( to.pos, next.pos ) );
            to = next;
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