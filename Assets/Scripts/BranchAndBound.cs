using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


/// <summary>
/// Expands the shortest path so far
/// </summary>
public class BranchAndBound : SearchAlgorythm {
    
    public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond, bool trackVisitedNodes) {
        Debug.LogFormat ("Searching path using Branch and Bound algorythm");
        bool hasPath = false;

        bool[,] visited = new bool[size, size];
        cameFrom = new Dictionary<Node, Node> ();
        List<NodeDist> frontier = new List<NodeDist> ();
        Node current = new Node ();
        float pathLength = 0;

        frontier.Add (new NodeDist (start, 0));
        cameFrom[start] = null;

        while (frontier.Count > 0) {
            current = frontier[0].node;
            pathLength = frontier[0].distance;
            frontier.RemoveAt (0);

            UIUpdatePathLength (pathLength);
            UIUpdateQueueSize (frontier.Count);

            //visualize path to current
            SearchManager.VisualizePath (cameFrom, current, start);
            yield return new WaitForSeconds (1f / framesPerSecond);

            if (current == goal) {
                hasPath = true;
                break;
            }

            //get the maxNodes nodes closest to the objective and keep them.
            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];

                if (trackVisitedNodes) {
                    if (!visited[(int) neighbour.pos.x, (int) neighbour.pos.y]) {
                        visited[(int) neighbour.pos.x, (int) neighbour.pos.y] = true;

                        cameFrom[neighbour] = current;
                        frontier.Add (new NodeDist (neighbour, pathLength + Mathf.Abs(Vector2.Distance (neighbour.pos, current.pos))));
                        UIIncrementEnqueuings ();
                    }
                }
                else {
                    cameFrom[neighbour] = current;
                    frontier.Add (new NodeDist (neighbour, pathLength + Mathf.Abs(Vector2.Distance (neighbour.pos, current.pos))));
                    UIIncrementEnqueuings ();
                }

                frontier.Sort ();
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
