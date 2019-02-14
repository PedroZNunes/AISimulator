using System;
using UnityEngine;


public class Minimax : GamesAlgorithm
{

    public override void Search (TreeNode root, int branching, int depth)
    {
        CheckNode (root);

        OnSearchEnded ();

        if (root.leafID != null)
            OnLeafActivated (TreeNode.GetByID ((int)root.leafID));

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
            if (node != TreeGenerator.Root)
                node.parentBranch.SetState (NodeState.Explored);
        }

        if (node.branches.Length == 0) {
            node.SetState (NodeState.Explored);
            node.parentBranch.SetState (NodeState.Explored);
        }
    }

}
