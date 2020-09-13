using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This simple script allows for a time for the food to be unavailable after having been eaten.
*/
public class Spawn : MonoBehaviour
{

    float timeBeforeRespawning = 15f; // 30 secs
    bool hasBeenEaten = false;
    float timeSinceEaten = 0f;


    // ---------- Setter / Getter ----------------- //
    public bool getHasBeenEaten(){
        return this.hasBeenEaten;
    }

    // -------------- Start and update functions ----------- //

   private void Start() {

        float gameSpeed = this.transform.parent.transform.parent.GetComponent<Parameters>().getGameSpeed();
        this.timeBeforeRespawning = this.timeBeforeRespawning / gameSpeed; 

   }
    private void Update() {

        if (this.hasBeenEaten){
            if (this.timeSinceEaten > this.timeBeforeRespawning){
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<Collider>().enabled = true;
                this.hasBeenEaten = false;
                this.timeSinceEaten = 0f;
            }
            else {
                this.timeSinceEaten += Time.deltaTime;
            }
        }
    }

    // ---------------- Other functions ----------------- //
    public void isEatenBy(){
        this.hasBeenEaten = true;
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
    }

    
}
