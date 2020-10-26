using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//------------------------------------ || Request Manager class ||-----------------------------------\\


// This class is controlling the path requests from units by making a queue system --> This is to avoid that more than one enemy calls for a path in the same frame


public class PathRequestManager : MonoBehaviour {
    

	public bool useHeap;
    public bool useLoopSearch;

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();                    // This is request queue --> firast in - first out.
	PathRequest currentPathRequest;                                                     // Our current path request.  



    static PathRequestManager instance;                                                 // The pathRequestManager that is being used now. (this) --> self-reference (an instance of the script)                                             
	Pathfinding pathfinding;                                                            // A pathfinding object 

	bool isProcessingPath;                                                              // Boolean to know if the request is alreeady in queue

    void Awake(){   // Awake is called when the script object is initialized. Is called once in the lifetime of the script.
                    // Is called on all objects in the scene before any objects "Start" method --> unless objects are instantiated during gameplay.


		instance = this;                                                                // Self-reference --> instance = this instance..
		pathfinding = GetComponent<Pathfinding> ();                                     // Is placed in awake so that this will be the first thing to happen --> we instanciate the pathfinding variable as a pathfinding object.

	}

    //------------------------------------ || Request a path ||-----------------------------------\\

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback){       

		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);         // A new path Request taking in a start and end position and a callback to tell if a path is found.
		instance.pathRequestQueue.Enqueue (newRequest);                                  // Put the new path request into the queue
		instance.TryProcessNext ();                                                      // Check if we have a path yet                                                                                              

	
	}

    //------------------------------------ || Begin processing a path ||-----------------------------------\\

	void TryProcessNext(){

		if (!isProcessingPath && pathRequestQueue.Count > 0) {
            // If the path is not already processing and the queue is not empty

			currentPathRequest = pathRequestQueue.Dequeue ();                            // Take the path request out of the Queue
			isProcessingPath = true;                                                     // We are now processing the path


			if (useHeap){
				useLoopSearch = false;
			
			pathfinding.StartFindPathHeap (currentPathRequest.pathStart, currentPathRequest.pathEnd);       // Ask the pathfinding object to find a path
			} else if (useLoopSearch){
				useHeap = false;
				pathfinding.StartFindPathLoop(currentPathRequest.pathStart, currentPathRequest.pathEnd);

			}
			}

	}

    //------------------------------------ || finished processing the path ||-----------------------------------\\

	public void FinishedProcessingPath(Vector3[] path, bool success){

		currentPathRequest.callback (path, success);                    // Insert the path and the succes criteria into the callback Action
		isProcessingPath = false;                                       // We are not processing the path anymore 
		TryProcessNext();                                               // Check if a new path request has entered the queue


	}

    //------------------------------------ || The path request structure ("class") ||-----------------------------------\\

	struct PathRequest{

		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

        //------------------------------------ || Path request constructor ||-----------------------------------\\

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback){

			pathStart = _start;
			pathEnd = _end;
			callback = _callback;



		}
	}
}
