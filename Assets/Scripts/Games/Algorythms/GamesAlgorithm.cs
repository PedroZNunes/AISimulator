using System;

public class GamesAlgorithm {

    public delegate void NodeAnalyzedHandler (TreeNode node);
    static public event NodeAnalyzedHandler NodeAnalyzedEvent;

    static public event Action PrunedNodesEvent;

    public virtual void Search (TreeNode root, int branching, int depth) { }

    /// <summary>
    /// function to proc the event warning that a node has been analyzed.
    /// </summary>
    /// <param name="node"> the node that has been analyzed </param>
    protected void NodeAnalyzed (TreeNode node) {
        if (NodeAnalyzedEvent != null)
            NodeAnalyzedEvent (node);
    }

    /// <summary>
    /// function to proc the event warning that the UI should calculate the nodes that were pruned.
    /// </summary>
    protected void CalculatePruned () {
        if (PrunedNodesEvent != null)
            PrunedNodesEvent ();
    }

}
