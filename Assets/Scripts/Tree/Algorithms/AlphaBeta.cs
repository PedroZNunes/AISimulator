using UnityEngine;

/// <summary>
/// AlphaBeta Checks the tree in a top-down fashion, dealing with each row as turns in a game.
/// In your turn the bot should look for the best possible option, a MAX. In your oponents turn the bot should look for the best enemy option, which would be the bots own worst outcome, in other words, a MIN.
/// The MAX filters the best outcome of possible ones, denotes by numbered scores. The Min does the same, but looks for the worst one. 
/// This is how the MinMax works, and both are pretty much the same, except that the AlphaBeta has the ability of pruning, having the potential to save a lot of memory.
/// PRuning happens when the algorithm sees that there is no possiblity that that subtree could contribute to the final outcome in any way. 
/// For example when a MIN is comparing his children and sees a MAX already calculated with a score of '3' on one side, and the other side has a MAX that already has '4' in it and is still checking his own children.
/// The MAX will not return anything smaller than 3, since it already has a 4, therefore it can be pruned along with whatever was under it that was unchecked. This clears up memory for other operations.
/// </summary>
public class AlphaBeta : TreeSearchAlgorithm
{

    public override void Search (TreeNode root, int branching, int depth)
    {
        //starts recursive searching
        CheckNode (root);

        OnSearchEnded ();
        Debug.LogFormat ("The output is {0}, from leaf {1}", root.Score, root.leafID);
    }

    /// <summary>
    /// Checks node passing current alpha and beta to it
    /// </summary>
    /// <param name="node">node to be checked</param>
    /// <param name="parentBestScoreMax">best value in the path to the root for the maximizer</param>
    /// <param name="parentBestScoreMin">best value in the path to the root for the minimizer</param>
    /// <param name="leafID">The ID of the leaf that holds the value being returned by the function</param>
    private void CheckNode (TreeNode node, int parentBestScoreMax, int parentBestScoreMin)
    {
        node.bestScoreMax = parentBestScoreMax;
        node.bestScoreMin = parentBestScoreMin;

        for (int i = 0; i < node.branches.Length; i++) {
            //if (minimizer) value < alpha -> prune
            if ((node.Type == NodeType.Min) && (node.Score <= node.bestScoreMax)) {
                Debug.LogFormat ("Node {0} pruned branch index {1}. Alpha: {2}, Node value:{3}", node.ID.ToString (), i, node.bestScoreMax, node.Score);

                Prune (node);
                break;
            }

            //if (maximizer) value > beta -> prune
            else if ((node.Type == NodeType.Max) && (node.Score >= node.bestScoreMin)) {
                Debug.LogFormat ("Node {0} pruned branch index {1}. Beta: {2}, Node value:{3}", node.ID.ToString (), i, node.bestScoreMin, node.Score);
                
                Prune (node);
                break;
            }

            //look at the next children, pass its alpha and beta to it.
            TreeNode childNode = node.GetOtherNodeFromBranchByIndex (i);
            CheckNode (childNode, node.bestScoreMax, node.bestScoreMin);
            //if the children is a maximizer or a minimizer, the the loop will go on.
            //if the children is a leaf, the leaf passes the value back up to the parent

            //if (minimizer) new value < value - assign and update bestValue
            if ((node.Type == NodeType.Min) && (childNode.Score < node.Score)) {
                node.SetScore (childNode);
            }
            //if (maximizer) new value > value - assign and update alpha
            else if ((node.Type == NodeType.Max) && (childNode.Score > node.Score)) {
                node.SetScore (childNode);
            }
            
            node.SetState (NodeState.Explored);
            if (node != TreeGenerator.root)
                node.parentBranch.SetState (NodeState.Explored);
        }

        if (node.branches.Length == 0) {
            node.SetState (NodeState.Explored);
            node.parentBranch.SetState (NodeState.Explored);
        }

    }

    /// <summary>
    /// Discards whole sub-trees of nodes. Happens when the algorithm sees that there is no possiblity that that subtree could contribute to the final outcome in any way. 
    /// For example when a Min sees '3' on one side and the other side has a max that already has '4' in it. The max will not return anything smaller than 3, since it already has a 4.
    /// Therefore it can be pruned along with whatever was under it that was unchecked.
    /// </summary>
    /// <param name="parentNode">Node to be pruned</param>
    private void Prune (TreeNode parentNode)
    {
        parentNode.SetState (NodeState.Pruned);

        //go from the node to the children recursively until get a leaf. 
        for (int i = 0; i < parentNode.branches.Length; i++) {
            TreeNode child = parentNode.GetOtherNodeFromBranchByIndex (i);

            if (child.Type != NodeType.Leaf)
                Prune (child);


            if (child.State != NodeState.Explored)
                child.SetState (NodeState.Pruned);

            if (child.parentBranch.State != NodeState.Explored)
            child.parentBranch.SetState (NodeState.Pruned);
        }
    }

    private void CheckNode (TreeNode node)
    {
        CheckNode (node, int.MinValue, int.MaxValue);
    }
}
