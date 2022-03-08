using System;
using System.Collections.Generic;
using System.Linq;

public class Circuit {
  List<Fragment> fragments;
  public IEnumerable<Fragment> Fragments => fragments;
  FlowGraph graph;
  public Circuit(IEnumerable<Fragment> fragments) {
    this.fragments = fragments.ToList();
    graph = new FlowGraph(fragments.SelectMany(f => new [] { f.outt, f.inn }).ToHashSet());
  }
  public Circuit() : this(Enumerable.Empty<Fragment>()) {
  }

  public void AddFragment(Fragment f) {
    fragments.Add(f);
    graph.AddNodes(f.outt, f.inn);
  }

  public void RemoveFragment(Fragment f) {
    fragments.Remove(f);
    graph.RemoveNodes(f.outt, f.inn);
  }

	internal void simulate(float dt) {
    foreach(var f in fragments) {
      f.Update(dt);
    }
    foreach(var f in fragments) {
      f.assignNodeFlows(dt);
    }

    graph.solve();

    foreach(var f in fragments) {
      f.exchange();
    }
	}

	internal void print() {
		foreach(var f in fragments) {
      System.Console.WriteLine(f);
    }
	}
}

public class FlowGraph {
  HashSet<Node> nodes;
  IEnumerable<Edge> allEdges => nodes.SelectMany(n => n.edges);

  public FlowGraph(HashSet<Node> nodes) {
    this.nodes = nodes;
  }

  // assign flows to all edges
  public void solve() {
    foreach(var e in allEdges) {
      e.reset();
    }

    foreach(var n in nodes) {
      n.assignOffers();
    }
    
    foreach(var e in allEdges) {
      e.resolveOffer();
    }
  }

  public void print() {
    System.Console.WriteLine($"Graph: {String.Join(", ", nodes.Select(n => n.name))}");
    foreach(var e in allEdges) {
      e.print();
    }
  }

	public void AddNodes(Node outt, Node inn) {
		nodes.Add(outt);
    nodes.Add(inn);
	}

	public void RemoveNodes(Node outt, Node inn) {
		nodes.Remove(outt);
    nodes.Remove(inn);
	}
}

public class Node {
  public string name;
  // set per simulate step
  public float flow = -1;
  public Node(string name) {
    this.name = name;
  }

  public List<Edge> edges = new List<Edge>();

	// assumes we're supplier, other is demander
	public void connectInn(Node inn) {
    // if (edges.Any(e => e.outt == this)) {
    //   System.Console.WriteLine($"duplicate edge {this.name}, {other.name}");
    // }
    Edge e = new Edge(this, inn);
    edges.Add(e);
    inn.edges.Add(e);
  }

  public void disconnectInn(Node inn) {
    var e = edges.FirstOrDefault(e => e.inn == inn);
    edges.Remove(e);
  }

  public void assignOffers() {
    var totalDemand = edges.Select(e => e.other(this).flow).Sum();
    var flowRatio = totalDemand == 0 ? 0 : flow / totalDemand;
    foreach(var e in edges) {
      var flowAmount = flowRatio * e.other(this).flow;
      if (e.outt == this) {
        e.outOffer = flowAmount;
      } else {
        e.inOffer = flowAmount;
      }
    }
  }
}

public class Edge {
  public Edge(Node outt, Node inn) {
    this.outt = outt;
    this.inn = inn;
  }
  
  public Node outt, inn;

  public float outOffer = -1, inOffer = -1, flow = -1;

  public Node other(Node me) {
    return me == outt ? inn : me == inn ? outt : null;
  }
  
  public void reset() {
    outOffer = inOffer = flow = -1;
  }

  public void resolveOffer() {
    flow = Math.Min(outOffer, inOffer);
  }

  public void print() {
    var flowString = flow == -1 ? "" : $" = {flow}";
    System.Console.WriteLine($"[{outt.name} {outOffer}] ---- [{inOffer} {inn.name}]{flowString}");
  }
}
