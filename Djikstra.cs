using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//--- || Dijkstra class ||---\\

public class Djikstra : MonoBehaviour {
   
	public Transform target, seeker;         
	Grid1 grid;                             
	public long DijkstraMilliseconds;       
	Heap<DNode> openSet;                                     
   
	//--- || Awake ||---\\

	public void Awake()  {   
      
		grid = GetComponent<Grid1>();    
    }

	//--|| Start ||--\\

	public void Start()     
    {
		if (grid.UseDijkstraHeap){                         
         
			DijkstraSearchHeap(seeker.position, target.position);       
		} 

		if(grid.UseDijkstraLoop){                          
			DijkstraSearchLoop(seeker.position, target.position);      
		}   
    }

	//--- || Update ||---\\
    
	void Update()       
    {
		if (Input.GetButtonDown("Jump"))                         
        { 
			if (grid.UseDijkstraHeap)                       
            {
				DijkstraSearchHeap(seeker.position, target.position);      
            }
			if (grid.UseDijkstraLoop)                     
            {
				DijkstraSearchLoop(seeker.position, target.position);       
            }         
        }
    }

	//--- || Dijkstra heap search / sort ||---\\

	public void DijkstraSearchHeap(Vector3 startPos, Vector3 goalPos){
      
		Stopwatch dsw = new Stopwatch();            
		dsw.Start();                               
      
		DNode startNode = grid.DNodeFromWorldPoint(startPos);       
		DNode endNode = grid.DNodeFromWorldPoint(goalPos);         

		startNode.gCost = 0;                                        

		if (startNode.walkable && endNode.walkable){    

			openSet = new Heap<DNode>(grid.MaxSize);                    
			HashSet<DNode> closedList = new HashSet<DNode>();             
			openSet.Add(startNode);                                  
            
			while(openSet.Count > 0){                                  
              
				DNode currentNode = openSet.RemoveFirst();              
				closedList.Add(currentNode);                           

            if (currentNode == endNode){                                
					
				dsw.Stop();                                                   
				print("Path found in: " + dsw.ElapsedMilliseconds + " ms");   
				DijkstraMilliseconds = dsw.ElapsedMilliseconds;              

				RetracePath(startNode, endNode);                        
                break;
            }
				foreach (DNode neighbour in grid.GetDNeighbours(currentNode)){ 

				if(!neighbour.walkable || closedList.Contains(neighbour) {         
						continue;                                                         
                }
			        int tentative_dist = currentNode.gCost + GetDistance(currentNode, neighbour);   

					if(tentative_dist < neighbour.gCost){              
						neighbour.gCost = tentative_dist;               
						neighbour.parent = currentNode;                 
						closedList.Add(neighbour);                             
						grid.dClosedList = closedList;                  
                    }
					if (!openSet.Contains(neighbour))                    
					{
						openSet.Add(neighbour);                         
					}
					else { openSet.UpdateItem(neighbour); }                   
            }   
        }
        }
    } 

	//---- || Dijkstra loop search / sort ||----\\
    
	public void DijkstraSearchLoop(Vector3 startPos, Vector3 goalPos){

		Stopwatch dsw = new Stopwatch();                                 
		dsw.Start();                                                   
      
		DNode startNode = grid.DNodeFromWorldPoint(startPos);          
		DNode endNode = grid.DNodeFromWorldPoint(goalPos);          

		startNode.gCost = 0;                                                   
		HashSet<DNode> closedList = new HashSet<DNode>();              
		List<DNode> openList = new List<DNode>();                       

		openList.Add(startNode);                                        
      
		if (startNode.walkable && endNode.walkable){                           
			
            while(openList.Count > 0){                                  

				DNode currentNode = openList[0];                        

			for (int i = 1; i < openList.Count; i++){                  

				if(openList[i].gCost < currentNode.gCost){              
					currentNode = openList[i];                          
				}               
			}

			openList.Remove(currentNode);                             
			closedList.Add(currentNode);                                  
	
			if (currentNode == endNode){                               
               
				dsw.Stop();                                               
				print("Path found in: " + dsw.ElapsedMilliseconds + " ms");        
					DijkstraMilliseconds = dsw.ElapsedMilliseconds;           
				RetracePath(startNode, endNode);                   
				break;
			}
                  
			foreach (DNode neighbour in grid.GetDNeighbours(currentNode)){     
					
			if(!neighbour.walkable || closedList.Contains(neighbour) ){    
						continue;                                             
				}
			int tentative_dist = currentNode.gCost + GetDistance(currentNode, neighbour);          

					if(tentative_dist < neighbour.gCost){                    
						neighbour.gCost = tentative_dist;                    
						neighbour.parent = currentNode;                      
						closedList.Add(neighbour);                           
					
						grid.dClosedList = closedList;                       
					}
					if(!openList.Contains(neighbour)){                    
						openList.Add(neighbour);                          
					}               
			}
		}
		}
	} 

	//---|| Retrace path ||---\\
 
    public void RetracePath(DNode startNode, DNode endNode) {
      
		List<DNode> thePath = new List<DNode>();          
		DNode currentNode = endNode;                        

		while (!currentNode.Equals(startNode))        
        {
			thePath.Add(currentNode);                       
			currentNode = currentNode.parent;               

			grid.DijkstraPath = thePath;                   
        }
    }
    
	int GetDistance(DNode nodeA, DNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);     
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);       
      
        if (dstX > dstY)        

            return 14 * dstY + 10 * (dstX - dstY);    
        return 14 * dstX + 10 * (dstY - dstX);           
    }
}

