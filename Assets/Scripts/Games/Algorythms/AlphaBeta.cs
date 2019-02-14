using System;
using UnityEngine;


public class AlphaBeta : GamesAlgorithm
{

    public override void Search (TreeNode root, int branching, int depth)
    {
        CheckNode (root);

        if (root.leafID.HasValue)
            OnLeafActivated (TreeNode.GetByID (root.leafID.Value));

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
    /// <returns>the value of the node</returns>
    private void CheckNode (TreeNode node, int parentBestScoreMax, int parentBestScoreMin)
    {
        node.bestScoreMax = parentBestScoreMax;
        node.bestScoreMin = parentBestScoreMin;

        for (int i = 0; i < node.branches.Length; i++) {
            //if (minimizer) value < alpha - prune
            if ((node.Type == NodeType.Min) && (node.Score <= node.bestScoreMax)) {
                Debug.LogFormat ("Node {0} pruned branch index {1}. Alpha: {2}, Node value:{3}", node.ID.ToString (), i, node.bestScoreMax, node.Score);

                Prune (node);
                break;
            }

            //if (maximizer) value > beta - prune
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
            if (node != TreeGenerator.Root)
                node.parentBranch.SetState (NodeState.Explored);
        }

        if (node.branches.Length == 0) {
            node.SetState (NodeState.Explored);
            node.parentBranch.SetState (NodeState.Explored);
        }

    }

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
