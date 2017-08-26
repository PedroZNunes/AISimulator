using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamesAlgorythm  {

    static public event Action NodeAnalyzedEvent;
    static public event Action SkippedNodesEvent;

    static public event Action ResetUIEvent;

    static public bool IsSearching { get; protected set; }

    public virtual IEnumerator Search (GamesNode root, int branching, int depth, int framesPerSecond) { return null; }

    protected void NodeAnalyzed () {
        if (NodeAnalyzedEvent != null)
            NodeAnalyzedEvent ();
    }

    protected void CalculateSkippedNodes () {
        if (SkippedNodesEvent != null)
            SkippedNodesEvent ();
    }

}
