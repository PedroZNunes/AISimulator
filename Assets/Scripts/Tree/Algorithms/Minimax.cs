using UnityEngine;

/// <summary>
/// MinMax checks the tree in a top-down fashion, dealing with each row as turns in a game.
/// In your turn the bot should look for the best possible option, a MAX. In your oponents turn the bot should look for the best enemy option, which would be the bots own worst outcome, in other words, a MIN.
/// The MAX filters the best outcome of possible ones, denotes by numbered scores. The Min does the same, but looks for the worst one. 
/// MAX Stands for Maximum and is denoted in the graph by a red triangle.
/// MIN stands for Minimum and is denoted in the graph by a blue upside-down triangle.
/// </summary>
public class Minimax : TreeSearchAlgorithm
{

    public override void Search (TreeNode root, int branching, int depth)
    {
        CheckNode (root);

        OnSearchEnded ();

        Debug.LogFormat ("The output is {0}, from leaf {1}", root.Score, root.leafID);
    }


    /// <summary>
    /// Checks node passing current alpha and beta to it
    /// </summary>
    /// <param name="node">node to be checked</param>
    /// <returns>the value of the node</returns>
    private void CheckNode (TreeNode node)
    {
        for (int i = 0; i < node.branches.Length; i++) {
            //look at the next children, pass its alpha and beta to it.
            TreeNode other = node.GetOtherNodeFromBranchByIndex (i);
            CheckNode (other);
            //if the children is a maximizer or a minimizer, the the loop will go on.
            //if the children is a leaf, the leaf passes the value back up to the parent

            //if (minimizer) new value < value - assign and update beta
            if ((node.Type == NodeType.Min) && (other.Score < node.Score)) {
                node.SetScore (other);
            }
            //if (maximizer) new value > value - assign and update alpha
            else if ((node.Type == NodeType.Max) && (other.Score > node.Score)) {
                node.SetScore (other);
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

}
