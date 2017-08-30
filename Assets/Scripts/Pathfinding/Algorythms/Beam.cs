using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// goes straight to the goal by expanding only a certain number of paths. is not optimal, but gives a path and is pretty fast. sometimes gets stuck
/// </summary>
public class Beam : PathfindingAlgorythm {

    private int maxPaths = 2;

    public Beam (int maxPaths ) {
        this.maxPaths = maxPaths;
    }

	public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) {
        IsSearching = true;
        Debug.LogFormat ("Searching path using Beam algorythm with the maximum number of paths set to {0}", maxPaths);
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        cameFrom = new Dictionary<Node , Node> ();
        List<NodeDist> frontier = new List<NodeDist> ();
        Node current = new Node();

        frontier.Add (new NodeDist (start , 0));
        cameFrom[start] = null;

        while (frontier.Count > 0) {
            current = frontier[0].node;
            frontier.RemoveAt (0);

            UIUpdate (frontier.Count, CalculatePathLength (start, current));

            //visualize path to current
            Pathfinder.VisualizePath (cameFrom, current , start);
            yield return new WaitForSeconds (1f/framesPerSecond); 
            
            if (current == goal) {
                hasPath = true;
                break;
            }

            //get the maxNodes nodes closest to the objective and keep them.
            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];

                if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                    visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                    cameFrom[neighbour] = current;
                    frontier.Add (new NodeDist (neighbour , Vector2.Distance (neighbour.pos , goal.pos)));
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

        IsSearching = false;

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
