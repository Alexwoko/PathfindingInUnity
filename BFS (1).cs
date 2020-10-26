using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//--|| Breath first search ||--\\

public class BFS : MonoBehaviour {            
    
	public Transform seeker, target;          
	Grid1 grid;                               
	public HashSet<Node> closedList;         
	public long BreathfirstMilliseconds;      

	//-- || Awake method ||--\\

	private void Awake() {  
 
        grid = GetComponent<Grid1>();                   
    }

	//--- || Start method ||---\\

	public void Start()  {   

		if (grid.UseBFS)           
        {
            BreathFirstSearch(seeker.position, target.position);               
        }
    }

	//---- || Update method ||----\\

	void Update()             
    {

		if (Input.GetButtonDown("Jump"))        
		{

			if (grid.UseBFS)                                

				BreathFirstSearch(seeker.position, target.position);         
		}
    }

	//---- || Breath first search method ||----\\

	public void BreathFirstSearch(Vector3 startPos, Vector3 goalPos) {

		Stopwatch bsw = new Stopwatch();            
		bsw.Start();                                

		List<Node> openList = new List<Node>();     
		closedList = new HashSet<Node>();          

		Node currentNode;                                        
		Node startNode = grid.NodeFromWorldPoint(startPos);       
		Node targetNode = grid.NodeFromWorldPoint(goalPos);       
		openList.Add(startNode);                                  

		if (startNode.walkable && targetNode.walkable)            
		{
			while (openList.Count > 0)                            
			{

				currentNode = openList[0];                       
				openList.Remove(currentNode);                      
				closedList.Add(currentNode);                       
				if (currentNode.Equals(targetNode))               
				{
					bsw.Stop();                                     

			print("Path found in: " + bsw.ElapsedMilliseconds + " ms");    
		    BreathfirstMilliseconds = bsw.ElapsedMilliseconds;              
               
					grid.BFSclosedList = closedList;                           
					RetracePath(startNode, targetNode);                  	
                    
					return;
				}

				foreach (Node i in grid.GetNeighbours(currentNode))               
				{
					if (!i.walkable || closedList.Contains(i))                      
					{
						continue;                      
					}   


					if (!openList.Contains(i))          
					{
						i.parent = currentNode;    
						openList.Add(i);                
					}
				}
			}
		}
	}

	//--- || Retrace the path ||---\\

	public void RetracePath(Node startNode, Node endNode) {
       
		List<Node> thePath = new List<Node>();         
        Node currentNode = endNode;                     
    
        while (!currentNode.Equals(startNode))          
        {
            thePath.Add(currentNode);                  
            currentNode = currentNode.parent;               
        }
        thePath.Reverse();                             
		grid.path = thePath;                                                   
    }
}
