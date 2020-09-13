using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    This function allows for the tracking of the population. 
    Be careful, this script suppose a hierarchy : parent --> Blups --> each blup. It wont work without it. 
    If I want to be able to track more, I will have to change it a little bit.
    TODO : make a .txt to analyse it later on with python.
*/
public class Parameters : MonoBehaviour
{

    public float gameSpeed = 1f; // we use the gameSpeed but this functionnality does not work very properly so far
    public int population;
    public int FoodSpotNumber;
    public int infectedPopulation = 0;
    public int deathByVirus = 0;

    List<int> popEvol;
    List<int> popInfected;
    List<int> popDeadByVirus;

    public float savingTime = 5f;
    float time;

    // --------------- Getter and Setter ------------------- //

    public float getGameSpeed(){
        return this.gameSpeed;
    }

    // --------------- Start and Update functions --------------- //
    void Start()
    {

        time = 0f;
                // be careful gamespeed is fixed at the start of the game.
        this.popEvol = new List<int>(); // should go in a text file afterwards.     
        this.popInfected = new List<int>();
        this.popDeadByVirus = new List<int>();
        this.updatePops();
        // when using this function we need to make sure we have the right amount of population etc.
        // and we know why it has died etc. 
        // to start with we just keep track of the total population.

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > savingTime){    
            this.updatePops();
            time = 0f;
        }
    }

    // --------------- Other  functions -------------------- //
    private void updatePops(){
        // this functions updates all pops by looking for the children of the given object.
        this.deathByVirus = 0;
        this.population = 0;
        this.infectedPopulation = 0;
        foreach (Transform child in this.transform.Find("Blups").transform){
            if (child.GetComponent<Virus>().getIsDead()){
                this.deathByVirus += 1;
            }
            else {
                if (!child.GetComponent<Energy>().getIsDead()){
                    this.population += 1;
                    if (child.GetComponent<Virus>().getIsSick()){
                        this.infectedPopulation += 1;
                    }
                }
            }   
        }
        this.popEvol.Add(this.population); // we automatically add the new pop to the list
        this.popDeadByVirus.Add(this.deathByVirus);
        this.popInfected.Add(this.infectedPopulation);
    }

    
}
