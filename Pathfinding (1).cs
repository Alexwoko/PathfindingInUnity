using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

//-- || Pathfinding class ||--\\

public class Pathfinding : MonoBehaviour { 

	public Transform seeker, target;        
	public long AStarMilliseconds;         
	List<Node> openList;                   
	Heap<Node> openSet;                   
	HashSet<Node> closedSet;                
	Grid1 grid;                             

	//--- || Awake method ||---\\

	void Awake(){   

		AStarMilliseconds = 0;                                      
		grid = GetComponent<Grid1> ();                          

		if (grid.UseAStarHeap)                                      
        {
            StartCoroutine(FindPathHeap(seeker.position, target.position));   
        }
		if (grid.UseAStarLoop)                                     
        {
            StartCoroutine(FindPathLoop(seeker.position, target.position));   
        }      
	}

	//-- || Update method ||--\\

	void Update(){                                                 

		if(Input.GetButtonDown("Jump")){                           

			if(grid.UseAStarHeap){                                 
				StartCoroutine(FindPathHeap(seeker.position, target.position));    
			}
			if(grid.UseAStarLoop){                                  
				StartCoroutine(FindPathLoop(seeker.position, target.position));    
			}
		}
	}

	//--- || Heap coroutine ||---\\
   
	IEnumerator FindPathHeap(Vector3 startPos, Vector3 targetPos){      
      
		Stopwatch swHeap = new Stopwatch();                    
		swHeap.Start();                                            
		Vector3[] waypoints = new Vector3[0];                     
		bool pathSuccess = false;								  
		Node startNode = grid.NodeFromWorldPoint (startPos);	  
		Node targetNode = grid.NodeFromWorldPoint (targetPos);	  

		if (startNode.walkable && targetNode.walkable) {          

			openSet = new Heap<Node> (grid.MaxSize);              
			closedSet = new HashSet<Node> ();		              
			openSet.Add (startNode);                              

            while (openSet.Count > 0) {                          
            
				Node currentNode = openSet.RemoveFirst();                  
				closedSet.Add(currentNode);                 
				grid.closedSet = closedSet;                   
            
				if (currentNode == targetNode){ 
                
				swHeap.Stop();                           
				print("Path found in: " + swHeap.ElapsedMilliseconds + " ms");     
				AStarMilliseconds = swHeap.ElapsedMilliseconds;                  
						pathSuccess = true;                         
						break;                                        
					}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {             
				if (!neighbour.walkable || closedSet.Contains (neighbour)) { 
						continue;			
					}
               
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);

					if (newMovementCostToNeighbour < currentNode.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;         
						neighbour.hCost = GetDistance(neighbour, targetNode); 
						neighbour.parent = currentNode;                       
						openSet.Add(neighbour);

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour); 
                            						else 
							openSet.UpdateItem(neighbour); 

					}               
				}            
			}
		}	                 
		yield return null;													
		if (pathSuccess) {												
			waypoints = RetracePath (startNode, targetNode);				
		}
	}

	//--|| array coroutine ||--\\

    IEnumerator FindPathLoop(Vector3 startPos, Vector3 targetPos){
      
		Stopwatch swLoop = new Stopwatch();                          
		swLoop.Start();                                            

		Vector3[] waypoints = new Vector3[0];                   
        bool pathSuccess = false;                                 

        Node startNode = grid.NodeFromWorldPoint (startPos);     
        Node targetNode = grid.NodeFromWorldPoint (targetPos);    

        if (startNode.walkable && targetNode.walkable) {         
         
			openList = new List<Node>();          
			closedSet = new HashSet<Node> ();       
            openList.Add (startNode);                             

			while (openList.Count > 0) {                          
																 
				Node currentNode = openList[0];                     

				for (int i = 1; i < openList.Count; i++) {         
                               
					if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost) {

						currentNode = openList[i];                             
                        }
                    }

				openList.Remove (currentNode);                   
            
                    closedSet.Add(currentNode);                  

                    if (currentNode == targetNode) {              

					swLoop.Stop();                                                                                        
					print("Path found in: " + swLoop.ElapsedMilliseconds + " ms");  
					AStarMilliseconds = swLoop.ElapsedMilliseconds;                 
                        pathSuccess = true;                                    
                        break;                                       
                    }
                
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {         
                if (!neighbour.walkable || closedSet.Contains (neighbour)) {   
                        continue;           
                    }
               
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour); 

					if (newMovementCostToNeighbour < currentNode.gCost || !openList.Contains (neighbour)) {

                        neighbour.gCost = newMovementCostToNeighbour;          
                        neighbour.hCost = GetDistance (neighbour, targetNode);    
                        neighbour.parent = currentNode;                         

						if (!openList.Contains(neighbour))
                          
							openList.Add(neighbour);     

                    }
                }
            }
        }
                         
        yield return null;                                                        
        if (pathSuccess) {                                                     
            waypoints = RetracePath (startNode, targetNode);                   
			grid.closedSet = closedSet;                                       
        }
    }   

	//-- || Retrace path ||--\\

	Vector3[] RetracePath(Node startNode, Node endNode){
      
		List<Node> path = new List<Node>();       						
		Node currentNode = endNode;		
        
		while (currentNode != startNode) {

			path.Add (currentNode);										
			grid.path = path;                                                
			currentNode = currentNode.parent;							
		}
		Vector3[] waypoints = SimplifyPath (path);  						
		Array.Reverse(waypoints);                                              		   return waypoints;													
	}

	//--- || Simplify path ||---\\

	Vector3[] SimplifyPath(List<Node> path){

        List<Vector3> waypoints = new List<Vector3> ();                       
        Vector2 directionOld = Vector2.zero;                                   

		for (int i = 1; i < path.Count; i++) {                                 

            Vector2 directionNew = new Vector2 (path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY); 
			if (directionNew != directionOld) {

				waypoints.Add(path[i].worldPosition);                         
			}
            directionOld = directionNew;                                       
		}
		return waypoints.ToArray ();                                           
	}
   
	//--- || Calculate distance between two nodes ||---\\
    
	int GetDistance(Node nodeA, Node nodeB){

		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);      
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);	   
      
		if (dstX > dstY)         

		return 14 * dstY + 10* (dstX - dstY);    
		return 14 * dstX + 10* (dstY - dstX);          
	} 
}
