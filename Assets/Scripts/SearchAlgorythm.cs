using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorythm {

    public Dictionary <Node, Node> cameFrom { get; protected set; }

    public virtual IEnumerator Search ( List<Node> nodes , int size , Node start , Node goal , int framesPerSecond, bool trackVisitedNodes ) { return null; }

}