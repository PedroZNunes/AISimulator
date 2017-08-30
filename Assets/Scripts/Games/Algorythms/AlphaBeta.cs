using UnityEngine;

public class AlphaBeta : GamesAlgorythm {

    public override void Search (GamesNode root, int branching, int depth) {
        CheckNode (root);

        CalculatePruned ();

        if (root.leafID != null)
            NodeAnalyzed (GamesNode.GetByID ((int) root.leafID));

        Debug.LogFormat ("The output is {0}, from leaf {1}", root.value, root.leafID);
    }

    private void CheckNode (GamesNode node) {
        CheckNode (node, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Checks node passing current alpha and beta to it
    /// </summary>
    /// <param name="node">node to be checked</param>
    /// <param name="alpha">best value in the path to the root for the maximizer</param>
    /// <param name="beta">best value in the path to the root for the minimizer</param>
    /// <param name="leafID">The ID of the leaf that holds the value being returned by the function</param>
    /// <returns>the value of the node</returns>
    private void CheckNode (GamesNode node, int alpha, int beta) {
        //initialize the node
        node.alpha = alpha;
        node.beta = beta;

        for (int i = 0 ; i < node.links.Length ; i++) {
            //if (minimizer) value < alpha - prune
            if (node.nodeType == NodeType.Min) {
                if (node.value < node.alpha) {
                    //prune
                    Debug.LogFormat ("Node {0} pruned link index {1}. Alpha: {2}, Node value:{3}", node.ID .ToString(), i, node.alpha, node.value);
                    continue;
                }
            }

            //if (maximizer) value > beta - prune
            else if (node.nodeType == NodeType.Max) {
                if (node.value > node.beta) {
                    //prune
                    Debug.LogFormat ("Node {0} pruned link index {1}. Beta: {2}, Node value:{3}", node.ID.ToString (), i, node.beta, node.value);
                    continue;
                }
            }

            //look at the next children, pass its alpha and beta to it.
            GamesNode other = node.GetOther (i);
            CheckNode (other, node.alpha, node.beta);
            //if the children is a maximizer or a minimizer, the the loop will go on.
            //if the children is a leaf, the leaf passes the value back up to the parent

            //if (minimizer) new value < value - assign and update beta
            if (node.nodeType == NodeType.Min) {
                if (other.value < node.value) {
                    node.value = other.value;
                    node.leafID = other.leafID;
                    node.beta = other.value;
                }
            }
            //if (maximizer) new value > value - assign and update alpha
            else {
                if (other.value > node.value) {
                    node.value = other.value;
                    node.leafID = other.leafID;
                    node.alpha = other.value;
                }
            }
        }

        if (node.links.Length == 0) {
            NodeAnalyzed (node);
        }

    }

}
