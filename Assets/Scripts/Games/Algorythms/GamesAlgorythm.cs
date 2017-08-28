using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamesAlgorythm {

    public delegate void NodeAnalyzedHandler (GamesNode node);
    static public event NodeAnalyzedHandler NodeAnalyzedEvent;

    static public event Action SkippedNodesEvent;

    static public event Action ResetUIEvent;

    protected int fps;

    static public bool IsSearching { get; protected set; }

    public virtual void Search (GamesNode root, int branching, int depth, int framesPerSecond) { }

    protected void NodeAnalyzed (GamesNode node) {
        if (NodeAnalyzedEvent != null)
            NodeAnalyzedEvent (node);
    }

    protected void CalculateSkippedNodes () {
        if (SkippedNodesEvent != null)
            SkippedNodesEvent ();
    }

}
