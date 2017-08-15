using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour {

    private SearchAlgorythm searchAlgorythm;

    [SerializeField]
    private Sprite inactiveLink;

    [SerializeField]
    private Sprite activeLink;

    public void StartPathing( List<Node> nodes , int size , Node start , Node goal , bool trackVisitedNodes ) {
        if (searchAlgorythm != null)
            searchAlgorythm.Search ( nodes, size, start, goal, trackVisitedNodes );
        else
            Debug.LogError ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm ( SearchAlgorythm algorythm ) {
        searchAlgorythm = algorythm;
    }

}
