using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/*
    The goal of this script is to allow for each "alive" subject to have an energy. When completely depleted, they simply die and they have to eat to have more.
    The energy is being depleted simply by moving and being alive.
    For now, the blops are by default looking for food (they dont have a choice to make - see BlopMovement). In the future, we could add some choices. 
    And make the probability of such choices to evolve with time. (natural selections)
*/
public class Energy : MonoBehaviour
{
   

    public int Gen = 1; // the generation of the blop (the first blop has 1)

    public GameObject model; // the type of children the blops will have (basically it's a copy from themselves, 
                             //To Investigate :weirdly its the exact copy so if they become sick at somepoint the children will also be)
    Color deathColor = Color.black; 
    public bool isDead = false;
    float InitialEnergy = 400f;
    public float currentEnergy;

    float foodEnergy = 100f; // the energy it gives to eat some food
    float energyProcreationMin = 401f; // for now we say that if it reaches the procreation energy then it creates a new blop.
    float energyProcreation = 150f; // the energy it takes to make the child


    // energy depletion 
    float offSetAlive = 1f; // the energy it takes to be alive, even if not moving.
    float frictionCoef = 0.1f; // this coef is simply to account for the lose of energy (which is proportionnal to the squared speed of the blop)
    float frictionForce; // that is going to be the frinction that when multiplied by the speed gives the loss in energy

    // -------- setter / getter -------------- //

    public void setIsDead(){
        this.isDead = true;
    }

    public bool getIsDead(){
        return this.isDead;
    }

    public void setEnergyToZero(){
        this.currentEnergy = 0f;
    }

    public void setGen(int Gen){
        this.Gen = Gen;
    }
    
    public int getGen(){
        return this.Gen;
    }

    // --------- Functions : Start and Update --------------- //
    void Start()
    {
        this.currentEnergy = this.InitialEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isDead){ // we check if its not dead. 

            // if start by looking if we are chasing after food.
            if (this.GetComponent<BlopMovement>().getFood()){ // if true; then the food has no yet been eaten since we spotted it

                if(this.GetComponent<BlopMovement>().getFoodReached()){ // if we reached a food, we eat it and it gives energy to the blop.

                    this.currentEnergy += foodEnergy;

                    // certain % of chance to become sick
                    this.GetComponent<Virus>().becomeSick();

                    // and we reset the food.
                    this.GetComponent<BlopMovement>().resetFood(); // dues to several upgrades done, its a little be complicated but basically 
                    // it says to blomovement that it can stop chasing for food.
                    // and that the food itself has to wait a certain amount of time to be available again.


                    if (this.currentEnergy > this.energyProcreationMin){ // we check if we can have a child.
                        this.procreate();
                        this.currentEnergy -= this.energyProcreation;
                    }
                }
            }

            else{ // the food has been eaten so we reset the food.
                this.GetComponent<BlopMovement>().resetFood();
            }

            // then we deplete the energy from being alive and moving 
            this.frictionForce = this.frictionCoef * this.GetComponent<Rigidbody>().velocity.magnitude;
            this.currentEnergy -= this.offSetAlive + Mathf.Abs(this.frictionForce *  this.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime);

            // and we check if we still have enough energy to live.
            if(this.currentEnergy < 0 ){
                // if not, we make sure he blop wont move anymore (and change its color)
                this.GetComponent<BlopMovement>().enabled = false; // we disable the movement. But the virus remains.
                this.GetComponent<CapsuleCollider>().enabled = false; // we disable the collider. But the virus remains.
                this.GetComponent<Rigidbody>().Sleep(); // We force the blop to stop mouving. But the virus remains.

                // we pass the message that it's dead to the 2 scripts : energy. We dont give it to "Virus.cs" since it's not dead from the virus. That what allows us to tell from what the blup died of.
                this.isDead = true;

                // and we give it its "dead color" 
                        // it can become a problem here - if we want to swap a color for another specy that does not have a "body" child
                this.transform.Find("body").GetComponent<MeshRenderer>().material.color = this.deathColor; 
            }
        }
    }

    public void procreate(){
        // script used to create a new blop, weirdly enough, it seems to copy the parent object (maybe the model is linked to it - to investigate)
        GameObject newObj = (GameObject) Instantiate(this.model); 

        newObj.transform.position = this.transform.position;
        newObj.GetComponent<Energy>().setGen(this.Gen + 1); // we add one to its generation
        newObj.name =  newObj.GetComponent<Energy>().getGen().ToString(); // we may want to find it a better name. 
        newObj.tag = "Blop";
        newObj.transform.parent = this.transform.parent; // and we give it the same parent that the current object has
    }

}
