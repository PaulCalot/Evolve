using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 
    This script allows for a virus to appear randomly when eating food. In this case, the given animal becomes sick.
    It also allows for the blop to survive it and become resistant or to die from it (randomly once again). 
    Lastly, it allows for the sick blop to pass the virus to other blop that it would collide with.
*/
public class Virus : MonoBehaviour
{
        // textures :
    Color deathColor = Color.black;
    Color sickColor = Color.green;
    Color curedColor = Color.red;

        // State of the animal 
    public bool isSick = false;
    public bool isCured = false; // when cured the blop becomes resistant to the virus
    public bool isDead = false;

        // probability of dying, getting sick and cured
    float probDeath=0.0001f;
    float probCured=0.0005f;
    float probReproduction = 0.5f;
    float probSick = 0.2f;
    
    // -------------------  setter / getter ----------------- //
    public bool getIsSick(){
        return this.isSick;
    }

    public bool getIsDead(){
        return this.isDead;
    }

    private void Start() {
        
    }

    // --------------- Update function -------------- //
    private void Update() {
        if (!this.GetComponent<Energy>().getIsDead()){ // we have to make sure its not already dead. That is required since we dont destroy anymore the dead blops (or whatever it is)

            if (this.isSick){

                float x = Random.Range(0f,1f);

                if (x<this.probDeath){ // it dies from the virus

                    // then we make sure the blop wont move anymore.
                    this.GetComponent<BlopMovement>().enabled = false; // we disable the movement. But the virus remains.
                    this.GetComponent<CapsuleCollider>().enabled = false; // we disable the collider. But the virus remains.
                    this.GetComponent<Rigidbody>().Sleep(); // We force the blop to stop mouving. But the virus remains.

                    // we pass the message that it's dead to the 2 scripts : energy and virus
                    this.GetComponent<Energy>().setIsDead();
                    this.isDead = true;

                    // and we give it its "dead color" 
                    this.transform.Find("body").GetComponent<MeshRenderer>().material.color = this.deathColor;

                    // And we also set the energy of the blop to zero :
                    this.GetComponent<Energy>().setEnergyToZero(); 

                }

                else if(x<this.probDeath+this.probCured){ // its cured from the virus 
                    this.isSick = false;
                    this.isCured = true;
                    this.transform.Find("body").GetComponent<MeshRenderer>().material.color = this.curedColor;
                    
                }

                // or it stays the same
            }
        }
    }



    private void OnTrigerEnter(Collider other) { // OnTrigerEnter is a little bit costly. Se we may want to do it differently, but then there is not many collisions, so it's ok.
        // this function allows for the virus to spread from one individu to another.
        // In  theory, once a blop is dead, the collider is deactivated, and so this function wont be called anymore.
        if (other.gameObject.tag == "Blop"){

            if (other.gameObject.GetComponent<Virus>().isSick && !this.isCured){ // we make sure that the blop is not already resistant.

                float x = Random.Range(0f,1f);

                if (x<this.probReproduction){ // then the blop becomes sick.
                    this.isSick = true;
                    this.transform.Find("body").GetComponent<MeshRenderer>().material.color = this.sickColor; // swap material.

                }
            }  
        }
    }
    
    // only called when they eat something. (to know if they become sick or not)
      public void becomeSick(){
          // we have to make sure its not already resistant
        if (!this.isCured){
            float x = Random.Range(0f,1f);
            if (x<probSick){
                this.isSick = true; // swap material (or texture)
                this.transform.Find("body").GetComponent<MeshRenderer>().material.color = this.sickColor;
            }   
        }
    }
}
