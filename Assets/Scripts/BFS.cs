using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class BFS : SearchAlgorythm {

	public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond, bool trackVisitedNodes) {
        Debug.Log ("Searching path using Breadth First Search algorythm.");
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        cameFrom = new Dictionary<Node , Node> ();
        Queue<Node> frontier = new Queue<Node> ();
        Node current = new Node();

        frontier.Enqueue (start);
        cameFrom[start] = null;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            UIUpdate (frontier.Count, CalculatePathLength (start, current));

            //visualize path to current
            SearchManager.VisualizePath (cameFrom, current , start);
            yield return new WaitForSeconds (1f/framesPerSecond); 

            if (current == goal) {
                hasPath = true;
                break;
            }

            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];
                
                if (trackVisitedNodes) {
                    if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                        visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                        cameFrom[neighbour] = current;
                        frontier.Enqueue (neighbour);
                        UIIncrementEnqueuings ();
                    }
                }
                else {
                    cameFrom[neighbour] = current;
                    frontier.Enqueue (neighbour);
                    UIIncrementEnqueuings ();
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
