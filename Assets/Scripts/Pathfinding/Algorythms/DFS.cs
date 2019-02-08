using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// expands by depth first.
/// </summary>
public class DFS : PathfindingAlgorythm {

	public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) {
        IsSearching = true;
        Debug.Log ("Searching path using Depth First Search algorythm.");
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        cameFrom = new Dictionary<Node , Node> ();
        Stack<Node> frontier = new Stack<Node> ();
        Node current = new Node();

        frontier.Push (start);
        cameFrom[start] = null;

        while (frontier.Count > 0) {
            current = frontier.Pop ();

            UIUpdate (frontier.Count, CalculatePathLength (start, current));

            //visualize path to current
            Pathfinder.VisualizePath (cameFrom, current , start);
            yield return new WaitForSeconds (1f/framesPerSecond); 

            if (current == goal) {
                hasPath = true;
                break;
            }

            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];
                
                if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                    visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                    cameFrom[neighbour] = current;
                    frontier.Push (neighbour);
                    UIIncrementEnqueuings ();
                }
            }
        }

        SearchComplete(hasPath);
        yield break;
    }

}
