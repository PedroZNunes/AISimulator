using System;


public class TreeSearchAlgorithm
{

    static public event Action searchEnded;

    /// <summary>
    /// Starts the search
    /// </summary>
    /// <param name="root">Root node at the top of the tree</param>
    /// <param name="branching">How many children each node has</param>
    /// <param name="depth">How many generations</param>
    public virtual void Search (TreeNode root, int branching, int depth) {}


    /// <summary>
    /// Procs the event warning that the UI should calculate the nodes that were pruned.
    /// </summary>
    protected void OnSearchEnded () {
        if (searchEnded != null)
            searchEnded ();
    }

}
