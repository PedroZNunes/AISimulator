using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SearchAlgorythm {

    public abstract Queue<Node> Search ( List<Node> nodes , int size , Node start , Node goal , bool trackVisitedNodes );
}
