using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link {
    public Node a { get; private set; }
    public Node b { get; private set; }

    public GameObject GO;

    public Link ( Node a , Node b , GameObject linkGO ) {
        this.a = a;
        this.b = b;
        this.GO = linkGO;
    }

    public bool HasNodes ( Node a, Node b) {
        return ( ( this.a == a && this.b == b ) || ( this.b == a && this.a == b ) );
    }
}