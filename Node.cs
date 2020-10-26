using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//--- || Node class ||---\\
public class Node : IHeapItem<Node> {                      
   
	public bool walkable;           
	public Vector3 worldPosition;  
   

	public int gCost;               
	public int hCost;              
	                                 
	public int gridX;             
	public int gridY;			     
	public Node parent;          
    int heapIndex;                
   
	//-- || Node constructor ||--\\
	public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY){
		this.walkable = walkable;
		this.worldPosition = worldPosition;
		this.gridX = gridX;
		this.gridY = gridY;
	}

	//-- || fCost get-method ||--\\
	public int FCost{
		get{ 
			return gCost + hCost;
		}	
	}
    //-- || Heap index set & get ||--\\

    public int HeapIndex
    {
        get {  return heapIndex;}               

        set  {heapIndex = value;}               
    }

    //-- || Compare fCosts ||--\\

    public int CompareTo(Node nodeToCompare)       
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);                   
        if (compare == 0){
            compare = hCost.CompareTo(nodeToCompare.hCost);                   
        }
        return -compare;                                                      
    }
}

