using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Expands the path with the sortest (distance so far + distance to the goal) 
/// </summary>
public class AStar : GraphSearchAlgorithm {
    
    public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) {
        IsSearching = true;
        Debug.LogFormat ("Searching path using A* algorythm");
        bool hasPath = false;

        bool[,] visited = new bool[size, size];
        cameFrom = new Dictionary<Node, Node> ();
        List<NodeDistance> frontier = new List<NodeDistance> ();
        Node current = new Node ();
        float pathLength;

        frontier.Add (new NodeDistance (start, 0 + Mathf.Abs (Vector2.Distance (start.pos, goal.pos))));
        cameFrom[start] = null;
        visited[(int)start.pos.x, (int)start.pos.y] = true;


        while (frontier.Count > 0) {
            current = frontier[0].node;
            pathLength = frontier[0].distance - Mathf.Abs(Vector2.Distance (current.pos, goal.pos));
            frontier.RemoveAt (0);

            UIUpdate( frontier.Count, pathLength, Mathf.Abs( Vector2.Distance( current.pos, goal.pos ) ) + pathLength );

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
                    float totalDistance = pathLength + Mathf.Abs (Vector2.Distance (neighbour.pos, current.pos)) + Mathf.Abs (Vector2.Distance (neighbour.pos, goal.pos));
                    frontier.Add (new NodeDistance (neighbour, totalDistance));
                    UIIncrementEnqueuings ();
                }

                frontier.Sort ();
            }
        }

        SearchComplete(hasPath);
        yield break;
    }

}
