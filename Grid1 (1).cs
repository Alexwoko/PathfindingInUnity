using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid1 : MonoBehaviour {    
 

	public bool displayAStarPath;        
	public bool displayAStarClosedSet;   
    public bool UseBFS;                  
    public bool UseAStarLoop;            
	public bool UseAStarHeap;            
	public bool UseDijkstraHeap;         
	public bool UseDijkstraLoop;         
	public bool DisplayDijkstraPath;     

    public bool displayGridGizmos;      
	public Vector2 gridWorldSize;      
	public float nodeRadius;           
	public LayerMask unwalkableMask;   

	// public Node[,] grid;               
    // public DNode[,] dGrid;

	 private Node[,] grid;
	 private DNode[,] dGrid;          

	float nodeDiameter;                 
	int gridSizeX, gridSizeY;         

	void Awake(){                     
    
		nodeDiameter = nodeRadius * 2;                                         
		gridSizeX = Mathf.RoundToInt (gridWorldSize.xa / nodeDiameter);        
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);        
		CreateGrid();                                                         
		CreateDGrid();                                                        
    
	}

    //-------- || Max grid size method ||-------\\
    
    public int MaxSize{
        get { 
            return gridSizeX * gridSizeY;                                   
        }
    }

	//------- || Create Dijkstra Grid ||--------\\

	void CreateDGrid(){
        
		dGrid = new DNode[gridSizeX, gridSizeY];  
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++){
			for (int y = 0; y < gridSizeY; y++){

				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

		bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				

				dGrid[x, y] = new DNode(walkable, worldPoint, x, y);           
			}
		}
	}


	//------ || Create Grid ||-------\\

	void CreateGrid(){    
		
		grid = new Node[gridSizeX, gridSizeY];   
		Vector3 worldBottomLeft = transform.position -	 Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;  
			
			for(int x = 0; x < gridSizeX; x ++){
				for ( int y = 0; y < gridSizeY; y ++){
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
 

		 bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius, unwalkableMask)); 
	     grid[x,y] = new Node(walkable,worldPoint, x, y); 
         
				}
			}
	}


	//----- || Get neighbours of a Dijkstra node ||-----\\

	public List<DNode> GetDNeighbours (DNode node){

		List<DNode> dNeighbours = new List<DNode>();     

		for (int x = -1; x <= 1; x++){
			for (int y = -1; y <= 1; y++){
            
				if(x == 0 && y == 0)  
					continue;
                
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

	            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
					
					dNeighbours.Add(dGrid[checkX, checkY]);
				}
			}
		}
		return dNeighbours;
	}
    


	//----- || Get neighbours of a node ||------\\

	public List<Node> GetNeighbours (Node node){
         
		List<Node> neighbours = new List<Node> ();      

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				
				if (x == 0 && y == 0)                    
					
					continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;

				
					if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY ){
						neighbours.Add (grid [checkX, checkY]);
					}
			}
		}
		return neighbours;
	}



//-- || Convert a Dijkstra node world position into a grid coordinate ||--\\
    
	public DNode DNodeFromWorldPoint(Vector3 worldPosition){

		float percentageX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentageY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

		percentageX = Mathf.Clamp01(percentageX);
		percentageY = Mathf.Clamp01(percentageY);
        
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentageX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentageY);

		return dGrid[x, y];
	}

    

 //-- || Convert a node world position into a grid coordinate ||--\\

	public Node NodeFromWorldPoint(Vector3 worldPosition){ 

		float percentageX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; 
		float percentageY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        
		percentageX = Mathf.Clamp01 (percentageX);
		percentageY = Mathf.Clamp01 (percentageY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentageX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentageY);

		return grid [x, y];
	}
    
    

//- || variables sent from other scripts to be used for visual indictors ||-\\


	public List<Node> path;                
	public HashSet<Node> closedSet;             
	public List<DNode> dClosedList;       
	public List<Node> BFSclosedList;            
	public List<DNode> DijkstraPath;              

//- || Draw visual indicators for scene view --> not in "Game" mode/view ||-\\

    void OnDrawGizmos(){             
		
		Gizmos.DrawWireCube(transform.position, new Vector3 (gridWorldSize.x,1,gridWorldSize.y));         
	
		if (grid != null && displayGridGizmos) {   
        
			foreach (Node n in grid) {                                              
				Gizmos.color = (n.walkable)?Color.white : Color.red;           

				if (BFSclosedList != null && UseBFS)           
                {
                    if (BFSclosedList.Contains(n))              
                    {
                        Gizmos.color = Color.black;             
                    }


                    if (path.Contains(n))                       
                    {
                        Gizmos.color = Color.yellow;            
                    }
                }

                if (path != null && displayAStarPath)           
					if (path.Contains (n))                    
					Gizmos.color = Color.green;                 

				if (closedSet != null && displayAStarClosedSet)    
				if (closedSet.Contains(n) && !path.Contains(n))     
						Gizmos.color = Color.black;                


                

					Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));     
			}
            
			if (UseDijkstraHeap && DisplayDijkstraPath || UseDijkstraLoop && DisplayDijkstraPath) 
			{
				foreach (DNode dn in dGrid)                                   
				{
					Gizmos.color = (dn.walkable) ? Color.white : Color.red;   

					if (DijkstraPath != null && UseDijkstraHeap || DijkstraPath != null && UseDijkstraLoop)  
					{
						if (DijkstraPath.Contains(dn))                         
							Gizmos.color = Color.blue;                         
					}
                    
					Gizmos.DrawCube(dn.worldPosition, Vector3.one * (nodeDiameter - .1f));                     
				}
			}
		}
	}
}
