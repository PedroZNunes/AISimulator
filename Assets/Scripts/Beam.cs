using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Beam : SearchAlgorythm {

    private int maxPaths = 2;

    public Beam (int maxPaths ) {
        this.maxPaths = maxPaths;
    }

	public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond, bool trackVisitedNodes) {
        Debug.LogFormat ("Searching path using Beam algorythm with the maximum number of paths set to {0}", maxPaths);
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        cameFrom = new Dictionary<Node , Node> ();
        List<NodeDist> frontier = new List<NodeDist> ();
        Node current = new Node();

        frontier.Add (new NodeDist (start , 0));
        cameFrom[start] = null;

        float pathLength = 0;

        while (frontier.Count > 0) {
            current = frontier[0].node;
            frontier.RemoveAt (0);

            UIUpdatePathLength (CalculatePathLength (start, current));
            UIUpdateQueueSize (frontier.Count);

            //visualize path to current
            SearchManager.VisualizePath (cameFrom, current , start);
            yield return new WaitForSeconds (1f/framesPerSecond); 
            
            if (current == goal) {
                hasPath = true;
                break;
            }

            //get the maxNodes nodes closest to the objective and keep them.
            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];

                if (trackVisitedNodes) {
                    if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                        visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                        cameFrom[neighbour] = current;
                        frontier.Add (new NodeDist (neighbour , Vector2.Distance (neighbour.pos , goal.pos)));
                        UIIncrementEnqueuings ();
                    }
                }
                else {
                    cameFrom[neighbour] = current;
                    frontier.Add (new NodeDist (neighbour, Vector2.Distance (neighbour.pos, goal.pos)));
                    UIIncrementEnqueuings ();
                }

                frontier.Sort ();

                for (int j = 0 ; j < frontier.Count ; j++) {
                    if (j > maxPaths - 1) {
                        frontier.RemoveAt (j);
                    }
                }
            }
            
            
        }

        if (!hasPath) {
            Debug.Log ("No path exists.");
            yield break;
        }
        else {
            Debug.Log ("Path complete.");
            yield break;
        }
    }


}
