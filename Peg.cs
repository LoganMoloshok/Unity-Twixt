using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour {

	//private GameObject[] links = new GameObject[8];
	private List<GameObject> links = new List<GameObject>();
	private string owner = "";

	public void setOwner(string o){
		owner = o;
	}

	public string getOwner(){
		return owner;
	}

	public void setLink(GameObject newLink){
		links.Add(newLink);
	}

	public void removeLink(int idx){
		links[idx] = null;
	}
		
}

/*
make graph:
for each peg in list of pegs
	if peg color = turn
		add to stack

*/