using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax : GamesAlgorythm {


    public override IEnumerator Search (GamesNode root, int branching, int depth, int framesPerSecond) {
        IsSearching = true;

        int output = CheckNode (root);

        Debug.LogFormat ("The output is {0}.", output);

        yield return 0;

        IsSearching = false;
    }

    private int CheckNode (GamesNode node) {
        return CheckNode (node, int.MinValue, int.MaxValue);
    }

    private int CheckNode (GamesNode node, int alpha, int beta) {
        //initialize the node
        node.alpha = alpha;
        node.beta = beta;

        for (int i = 0 ; i < node.links.Length ; i++) {
           
            //look at the next children, pass its alpha and beta to it.
            int value = CheckNode (node.links[i].GetOther (node), node.alpha, node.beta);
            //if the children is a maximizer or a minimizer, the the loop will go on.
            //if the children is a leaf, the leaf passes the value back up to the parent

            //if (minimizer) new value < value - assign and update beta
            if (node.nodeType == NodeType.Min) {
                if (value < node.value) {
                    node.value = value;
                    node.beta = value;
                }
            }
            //if (maximizer) new value > value - assign and update alpha
            else {
                if (value > node.value) {
                    node.value = value;
                    node.alpha = value;
                }
            }
        }

        if (node.links.Length == 0)
            NodeAnalyzed ();

        return node.value;
    }

}
