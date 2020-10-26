using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-- || Dijkstra-Node class ||--\\

public class DNode : IHeapItem<DNode>   
{
	public bool walkable;                    
	public Vector3 worldPosition;            
	public int gCost = int.MaxValue;      
	public int gridX;                       
	public int gridY;                       
	public DNode parent;          
	int heapIndex;                      
  
	//---- || Dijkstra-Node constructor ||----\\
	public DNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
	{
		this.walkable = walkable;
		this.worldPosition = worldPosition;
		this.gridX = gridX;
		this.gridY = gridY;
	}

	//-- || Heap index set & get ||--\\

	public int HeapIndex{

		set {
			heapIndex = value;                 
		}
		get{
			return heapIndex;           
		}
	}

	//--- || Compare gCosts ||---\\

	public int CompareTo(DNode nodeToCompare){                    

		int compare = gCost.CompareTo(nodeToCompare.gCost); 
		return -compare;                                            
	}
}