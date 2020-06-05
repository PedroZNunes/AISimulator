using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Expands the shortest path so far
/// </summary>
public class BranchAndBound : GraphSearchAlgorithm {
    
    public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) {
        IsSearching = true;
        Debug.LogFormat ("Searching path using Branch and Bound algorythm");
        bool hasPath = false;

        bool[,] visited = new bool[size, size];
        cameFrom = new Dictionary<Node, Node> ();
        List<NodeDistance> frontier = new List<NodeDistance> ();
        Node current = new Node ();
        float pathLength = 0;

        frontier.Add (new NodeDistance (start, 0));
        cameFrom[start] = null;
        visited[(int)start.pos.x, (int)start.pos.y] = true;


        while (frontier.Count > 0) {
            current = frontier[0].node;
            pathLength = frontier[0].distance;
            frontier.RemoveAt (0);

            UIUpdate( frontier.Count, pathLength );

            //visualize path to current
            GraphSearcher.VisualizePath (cameFrom, current, start);
            yield return new WaitForSeconds (1f / framesPerSecond);

            if (current == goal) {
                hasPath = true;
                break;
            }

            //get the maxNodes nodes closest to the objective and keep them.
            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];

                if (!visited[(int) neighbour.pos.x, (int) neighbour.pos.y]) {
                    visited[(int) neighbour.pos.x, (int) neighbour.pos.y] = true;

                    cameFrom[neighbour] = current;
                    frontier.Add (new NodeDistance (neighbour, pathLength + Mathf.Abs(Vector2.Distance (neighbour.pos, current.pos))));
                    UIIncrementEnqueuings ();
                }

                frontier.Sort ();
            }
        }

        SearchComplete(hasPath);
        yield break;
    }

}
