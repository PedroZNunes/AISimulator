using System;


public class GamesAlgorithm
{

    public delegate void LeafActivated (TreeNode node);
    static public event LeafActivated leafActivated;

    static public event Action searchEnded;

    
    public virtual void Search (TreeNode root, int branching, int depth) {}

    /// <summary>
    /// function to proc the event warning that a node has been analyzed. This is useful for separating the search in frames.
    /// </summary>
    /// <param name="activeLeaf"> the leaf is that is active in the algorithm. </param>
    protected void OnLeafActivated (TreeNode activeLeaf) {
        if (leafActivated != null)
            leafActivated (activeLeaf);
    }

    /// <summary>
    /// function to proc the event warning that the UI should calculate the nodes that were pruned.
    /// </summary>
    protected void OnSearchEnded () {
        if (searchEnded != null)
            searchEnded ();
    }

}
