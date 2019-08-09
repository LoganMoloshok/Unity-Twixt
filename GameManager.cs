using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
// --- Variables
	//public GameObject[] colliders;  //not actually in use
	enum curTurn {red, black};
	public string turn;
	public int turnNo;
	public GameObject[] newBridges;
	public GameObject[] oldBridges;	private bool doPeg = false;

	//new stuff
	private GameObject[,] pegs = new GameObject[22,22]; //y,x
	public GameObject redPegs;
	public GameObject blackPegs;


	void Start () {
		//set starting turn = red
		turn = "red";
		turnNo = (int)curTurn.red;

		//init clickable collider grid
		for(int z = 0; z < 22; z++){
			for(int i = 0; i < 22; i++){
				Instantiate(Resources.Load("Prefabs/" + "PegCollider"), new Vector3(i*3,0,3*z), Quaternion.identity);
				// make collider children of holding object
				GameObject.Find("PegCollider(Clone)").transform.parent = GameObject.Find("Colliders").transform;
				// rename collider to position [ (1,1) etc]
				GameObject.Find("PegCollider(Clone)").name = (GameObject.Find("PegCollider(Clone)").transform.position.x/3)+(",")+(GameObject.Find("PegCollider(Clone)").transform.position.z/3).ToString();
			}
		}
	}


	void Update () {
		
		// On mouse click, draw ray
		if(Input.GetMouseButtonDown(0)){
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);//assumes this script is attached to the primary camera
			RaycastHit hit; //the object hit by the raycast (should be a PegCollider object)
        	Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);//draw ray in editor
			
			// if ray hits something
			if(Physics.Raycast(ray, out hit)){
				if(hit.collider != null){
					// Make last turn's bridges Old and clear null (deleted) bridges
					foreach(GameObject go in newBridges){
						if(go == null){
							newBridges = null;
						}else{
							go.tag = "Old";	
						}
					}
					
					if(turn == "red"){
						if(hit.transform.position.x * 3 != 0 && hit.transform.position.x != 63){
							MakePeg(hit.collider.transform.position);
							hit.collider.enabled = false;
							//CheckBridges(hit.collider.transform.position/3);
							ChangeTurn();
						}
					}
					
					else if(turn == "black"){
						if(hit.transform.position.z * 3 != 0 && hit.transform.position.z != 63){
							MakePeg(hit.collider.transform.position);
							hit.collider.enabled = false;
							//CheckBridges(hit.collider.transform.position/3);
							ChangeTurn();
						}
					}

					// update bridge tags
					UpdateLists();
				}

			}//end ray hit
		}//end mouse down
	}

	private void isInEndzone(){
		for(int i = 1; i < 22; i++){
			if (pegs [0, i]) {
				pegs [0, i].GetComponent<Peg> ().getLinks ();
			}
		}
	}
		
	private void MakePeg (Vector3 newPos) {
		//create a new Peg object
		GameObject newPeg = (GameObject)Instantiate(Resources.Load("Prefabs/" + "Peg_Prefab"), newPos, Quaternion.Euler(270,0,0));
		string cTurn = turn;


		//Set Peg's parent object, name, and color based on turn
		if(turn == "red"){
			newPeg.transform.parent = redPegs.transform;
			newPeg.name = ("redPeg ") + (newPeg.transform.position.x/3) + (",") + (newPeg.transform.position.z/3).ToString();
			newPeg.GetComponent<Renderer> ().material.color = Color.red;
		}
		else if(turn == "black"){
			newPeg.transform.parent = blackPegs.transform;
			newPeg.name = ("blackPeg ") + (newPeg.transform.position.x/3) + (",") + (newPeg.transform.position.z/3).ToString();
			newPeg.GetComponent<Renderer> ().material.color = Color.black;
		}

		//add new Peg to list of Pegs
		pegs[(int)newPeg.transform.position.z/3, (int)newPeg.transform.position.x/3] = newPeg;

		//set peg owner
		Peg ps = newPeg.GetComponent<Peg>();
		ps.setOwner (turn);

		//checkNeighbors (newPeg.transform.position);
		checkNeighbors(newPeg);
		isInEndzone ();
	}
		
	private void checkNeighbors(GameObject center){

		GameObject link = null; //the Peg object to link with
		Peg linkScript = null; //the Peg stript attached to the Peg object
		Peg centerScript = center.GetComponent<Peg>();

		int x = (int)center.transform.position.x/3; //x coordinate of first peg
		int z = (int)center.transform.position.z/3; //z coordinate of first peg
		int x2;										//x coordinate of second peg
		int z2;										//z coordinate of second peg
		float angle;								//angle between pegs

		//TODO: condense this with some helpers
		for(int i = 2; i > -3; i--){
			for(int j = 2; j > -3; j--){
				if(Mathf.Abs(i) + Mathf.Abs(j) == 3){					//only check cases of (|2|, |1|) or (|1|, |2|)
					if(x+j < 22 && z+i < 22 && x+j >= 0 && z+i >= 0 ){	//only check in bounds (0 to 22)
						if (pegs [(z + i), (x + j)]) {					//if a peg exists at location

							link = pegs [(z + i), (x + j)];
							linkScript = link.GetComponent<Peg> ();
							x2 = (int)link.transform.position.x/3;
							z2 = (int)link.transform.position.z/3;
							angle = Mathf.Atan2 (x-x2, z-z2) * 180 / Mathf.PI;

							if(linkScript.getOwner() == turn){
								// if the peg we would link to is owned by the current player,
								// make the bridge
								CreateBridge (center.transform.position, (int)angle);
							}

						}//peg
					}//range
				}//abs
			}//j
		}//i
	}

	private void ChangeTurn () {
		if(turn == "black"){
			turn = "red";
		}
		else if(turn == "red") {
			turn = "black";
		}
		turnNo++;
	}

// --- Create Bridge
	private void CreateBridge (Vector3 origin, int direction) {
		// create bridge at origin rotated to direction
		//GameObject newBridge = (GameObject)Instantiate(Resources.Load("Prefabs/" + turn + "Bridge"), origin*3, Quaternion.Euler(270,direction,0));
		GameObject newBridge = (GameObject)Instantiate(Resources.Load("Prefabs/" + turn + "Bridge"), origin, Quaternion.Euler(270,direction,0));

		//GameObject.Find(turn + "Bridge(Clone)").tag = "New";
		//newBridge.transform.parent = GameObject.Find(turn + "Bridges").transform;
		newBridge.name = (turn) + (" Bridge ") + (origin.x) + "," + (origin.z).ToString();

	}
// --- Update tags
	private void UpdateLists() {
		newBridges = GameObject.FindGameObjectsWithTag("New");
		oldBridges = GameObject.FindGameObjectsWithTag("Old");
	}
// ---
}


/*
 * count red
 * count black
 * 
 * 
 * if bride is valid
 * 
 * 
 * 
 * */