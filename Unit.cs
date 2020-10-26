using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using FSM;                  // Finite state machine --> Own collection

//------------------------------------ || Unit ||-----------------------------------\\

public class Unit : MonoBehaviour {    // monobehavior == base class. (Like the object-class in java?)



   // int detectionRange = 40;
   // bool closeEnough = false;





    // New unnit  class variables --> (For the statemachine):

                                                          // A counter to keep track of seconds in the gameTimer

   // public StateMachine<Unit> stateMachine { set; get; }                            // A statemachine variable





    // Original unit class variables --> (for the pathfinding):

	public Transform target;    // Tranform can access: position, rotation and scale. We use it for the position of the target.
	public float speed;           // Unit speed
	Vector3[] path;             // Array of Vector3 (worldpoints) that holds the path.
    int targetIndex;            // Moves the unit closer to the target (Has a value for everytime we change directions --> targetIndex increments when we change directions)

	void Start(){   // enabled before any updates are called for the first time. Is called once in the lifetime of a script


        // new methods for the state machine:

       // stateMachine = new StateMachine<Unit>(this);                                // A statemachine object 
      //  stateMachine.ChangeState(IdleState.Instance);                               // Set tghe state to be the idle state (First state)
      //  gameTimer = Time.time;                                                      // game timer object


        // Original method for pathfinding:



             PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);   // Request a path from our path request manager
        

	}

    //------------------------------------ || Path found! ||-----------------------------------\\

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){               // We have found the shortest path.

		if (pathSuccessful) {                                                      // If we have found the shortest path

			path = newPath;                                                        // The path is our new path
			StopCoroutine ("FollowPath");                                          // Stop the coroutine --> if there is already a coroutine going on
			StartCoroutine ("FollowPath");                                         // start the coroutine 

            // A coroutine is a function that can pause the execution of a method and leave the control back to unity.
            // We use this so that we don´t run all the code of a method to end at once in a single frame --> 

		}

	}

    //------------------------------------ || Follow the path ||-----------------------------------\\

    // IEnumerator is used when we use Coroutine
    IEnumerator FollowPath()
    {                                                   // We have found a path, now follow it!

        Vector3 currentWaypoint = path[0];                                     // Vector3 that is a current waypoint from the simplified path
        {
            while (true)
            {                                                          // Runs until we break or return
                if (transform.position == currentWaypoint)
                {
                    // When we hit a waypoint in the simplified path

                    targetIndex++;                                                  // Increments target index
                    if (targetIndex >= path.Length)
                    {
                        // If we have moved past or are equal to the target
                        yield break;                                                // break the while loop

                    }
                    currentWaypoint = path[targetIndex];                            // The current waypoint is the (next/ incremented) incrementet targetIndex waypoint
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                // Update the position --> move the unit closer to target.


                yield return null;                                                  // return nothing
            }

        }
    }

	// New method for the state machine:


    /*
	private void Update()
	{




        if (Vector3.Distance(target.position, transform.position) <= detectionRange){
            closeEnough = true;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            Debug.Log("We´re in soldiers!");
        }
        */

        /*
        if(Time.time > gameTimer + 1){                                          
            // if time has past

            gameTimer = Time.time;                                              // Set our game timer to unitys timer --> now we can keep track of the time with our seconds variable
            seconds++;                                                          // increment seconds by one
            Debug.Log(seconds);                                                 // DEBUG 


        }
        if (seconds == 5){                                                      // If five seconds have past
            seconds = 0;                                                        // re-set seconds back 0
            switchState = !switchState;                                         // Switching state to opposite of current state                          
            chaseState = !chaseState;
            //idleState = false;
        }
        stateMachine.Update();                                                  // Update the state machine 
      
        
	}
  */

	//------------------------------------ || Graphical indicators ||-----------------------------------\\

	public void OnDrawGizmos(){

		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);

				} else {

					Gizmos.DrawLine (path[i-1], path[i]);
				}

			}


		}
	}

}
