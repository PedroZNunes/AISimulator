using UnityEngine;

public class AlphaBeta : GamesAlgorithm
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
    /// <param name="bestScoreMax">best value in the path to the root for the maximizer</param>
    /// <param name="bestScoreMin">best value in the path to the root for the minimizer</param>
    /// <param name="leafID">The ID of the leaf that holds the value being returned by the function</param>
    /// <returns>the value of the node</returns>
    private void CheckNode (TreeNode node, int bestScoreMax, int bestScoreMin)
    {
        node.bestScoreMax = bestScoreMax;
        node.bestScoreMin = bestScoreMin;

        for (int i = 0; i < node.branches.Length; i++) {
            //if (minimizer) value < alpha - prune
            if ((node.Type == NodeType.Min) && (node.Score < node.bestScoreMax)) {
                //prune
                Debug.LogFormat ("Node {0} pruned branch index {1}. Alpha: {2}, Node value:{3}", node.ID.ToString (), i, node.bestScoreMax, node.Score);
                continue;
            }

            //if (maximizer) value > beta - prune
            else if ((node.Type == NodeType.Max) && (node.Score > node.bestScoreMin)) {
                //prune
                Debug.LogFormat ("Node {0} pruned branch index {1}. Beta: {2}, Node value:{3}", node.ID.ToString (), i, node.bestScoreMin, node.Score);
                continue;
            }

            //look at the next children, pass its alpha and beta to it.
            TreeNode child = node.GetOther (i);
            CheckNode (child, node.bestScoreMax, node.bestScoreMin);
            //if the children is a maximizer or a minimizer, the the loop will go on.
            //if the children is a leaf, the leaf passes the value back up to the parent

            //if (minimizer) new value < value - assign and update bestValue
            if ((node.Type == NodeType.Min) && (child.Score < node.Score)) {
                node.SetScore (child);
            }
            //if (maximizer) new value > value - assign and update alpha
            else if ((node.Type == NodeType.Max) && (child.Score > node.Score)) {
                node.SetScore (child);
            }
        }

        if (node.branches.Length == 0) {
            OnLeafActivated (node);
        }

    }

    private void CheckNode (TreeNode node)
    {
        CheckNode (node, int.MinValue, int.MaxValue);
    }
}
