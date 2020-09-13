using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   /* 
        This function handles the movement of a given blup. So far, no choice is made (no random choice).
        We can however by playing on the parameters give different behaviors to the blup (more or less exploring etc. - especially by playing on the function NextTheta)
        Another possibility is to link it to the energy that we have remaining and explore until we have less that 70% 
        for example and get some food then (we can save points where we would be getting food).
    */  
public class BlopMovement : MonoBehaviour
{
    // there is the problem of those who never get close from there food...
    float SightRadius = 10f; // no sight angle yet
    float SpeedNorm = 60000f;
    float SpeedLimit = 7f; 
    float currentOrientation;
    Vector3 nextSpeed;

    GameObject food;
    Vector3 foodPosition;
    public bool foodFound = false;
    public bool foodReached = false;
    float DistanceEat = 0.4f;

    // ------------- Get and Set methods ---------------- //

    public bool getFoodReached(){
        return this.foodReached;
    }
    public void resetFood(){
        this.foodReached = false;
        this.foodFound = false;
        this.destroyFood();
        //this.initOrientation();
        //this.GetComponent<Rigidbody>().Sleep();
    }

    public Vector3 getFoodPosition(){
        return this.foodPosition;
    }

    public float getSpeedNorm(){
        return this.SpeedNorm;
    }

    public void destroyFood(){
        if (this.food != null){
            this.food.GetComponent<Spawn>().isEatenBy();    
        }
    }
    public bool getFood(){
        return this.food.GetComponent<MeshRenderer>().enabled == true; // return if the food has been disabled since we spotted it (to see if some other animal has eaten it)
    }


    // --------- Start and Update functions ------------ //
    private void Start(){
       this.initOrientation();
       // then we make up for the gameSpeed :
        float gameSpeed = this.transform.parent.transform.parent.GetComponent<Parameters>().getGameSpeed();
        this.SpeedNorm = this.SpeedNorm * gameSpeed;
        this.SpeedLimit = this.SpeedLimit * gameSpeed;
   }
     private void FixedUpdate() {
         // !this.GetComponent<Energy>().getIsDead() &&  // should be useless
       if (!this.foodReached){ // as long as we did not reach the food

            this.nextSpeed.y = 0; // we dont want to add any force towards the "y" coordinate. 
            this.GetComponent<Rigidbody>().AddForce(this.SpeedNorm * this.nextSpeed * Time.deltaTime); // we had the force.
            
            // limit velocity
            if (this.GetComponent<Rigidbody>().velocity.magnitude > this.SpeedLimit){
                this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity * (this.SpeedLimit/this.GetComponent<Rigidbody>().velocity.magnitude); 
            }

            this.UpdateOrientation(); // the blop takes the decision to where he will be heading next frame
       }
    }

    // ---------------- Orientation and looking-for-food functions --------------------- //
    public void initOrientation(){
        this.currentOrientation = Random.Range(-Mathf.PI,Mathf.PI);
        this.nextSpeed = new Vector3(Mathf.Sin(this.currentOrientation),0,Mathf.Cos(this.currentOrientation));
        // this.GetComponent<Rigidbody>().velocity = 100f*this.SpeedNorm * this.nextSpeed;
    }

    public void UpdateOrientation() {
        if (this.foodFound){ // we already have found food and we are converging towards it
            if(!this.foodReached){ // but still not reached.
                if(this.distance_xz(this.foodPosition)<this.DistanceEat){ // if we are close enough, we can eat it.
                    this.foodReached = true;
                }
                else {
                    // if not, we recompute the path.
                    this.nextSpeed = this.foodPosition - this.transform.position;
                    this.nextSpeed = this.nextSpeed / this.nextSpeed.magnitude;
                }
            }
            
            // this is the energy function taking the lead after that (if we reached the food). So no need for the "else" here.
            // maybe modifying that later?? I dont know if it's best like it is here.
        
        }
        else {
            if (this.checkFood()){
                this.nextSpeed = this.foodPosition - this.transform.position;
                this.nextSpeed = this.nextSpeed / this.nextSpeed.magnitude;
            }
            else {
                float theta = nextTheta(); //
                this.currentOrientation += theta;
                float s = Mathf.Sin(theta);
                float c = Mathf.Cos(theta);
                this.nextSpeed = new Vector3(this.nextSpeed.z*s + this.nextSpeed.x * c,0, -this.nextSpeed.x*s + this.nextSpeed.z * c); // new speed
            }
        }
    }

    public bool checkFood(){
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, this.SightRadius); // lossyScale is the global scale of the object (read only)
        
        // closest food
        float min = this.SightRadius + 1f;
        GameObject food = null;

        foreach (Collider collider in hitColliders){

            if (collider.gameObject.tag == "Food"){ // we the collider is food.
                float dist = this.distance_xz(collider.transform.position); // we compute the distance.
                if (dist<min){ // if this is closer than the previous saved one, we save its position, the gameObject itself and its current distance.
                    min = dist;
                    food = collider.gameObject;
                    this.foodPosition = collider.transform.position;
                }
            }

            // this following script is to go away from other blops. But in all generality, it does not make sense. Still save it for later (for example for an ENY)
           /* if (collider.gameObject.tag == "Blop"){
                
                Vector3 pos = collider.gameObject.transform.position;
                Vector3 dP = pos - this.transform.position;
                if (dP.sqrMagnitude != 0){
                    this.GetComponent<Rigidbody>().AddForce(- this.SpeedNorm*Time.deltaTime/dP.sqrMagnitude *dP);
                }
            }*/
        }

        // if we have found a food close to us, then we state we have found one and we save it. We could put it in the previous function. 
        if (min < this.SightRadius){
            this.foodFound = true;
            this.food = food;
            //this.GetComponent<Rigidbody>().Sleep();
            return(true);
        }
        return(false);
    }

  
    // ------------- Math functions ----------------- //


    // 2D-distance (along x and z axes)
    public float distance_xz(Vector3 pos){ 
        float dist = Mathf.Pow(this.transform.position.x - pos.x,2)+Mathf.Pow(this.transform.position.z - pos.z,2);
        return Mathf.Sqrt(dist);
    }
    public static float nextTheta(){ // can be improved. 
        float x = Random.Range(0f,1f); // gives a number between 0 and 1
        float a = 1f/1000f;
        float newOrientation;
        if (x<700f*a){ // angle = 0
            newOrientation = 0f;
        }
        else if (x<800f*a){ // -pi/4
            newOrientation = -0.25f; 
        }
        else if (x<900f*a){ // pi/4
            newOrientation = 0.25f; 
        }
        else if (x<940*a){ // -pi/2
            newOrientation = -0.5f;
        }
        else if (x<980*a){ // pi/2
            newOrientation = 0.5f;
        }
        else if (x<988*a){ // -3pi/4
            newOrientation = -0.75f; 
        }
        else if (x<996*a){ // 3pi/4
            newOrientation = 0.75f;
        }
        else { 
            newOrientation = -1f;
        }
        // Random.Range(-Mathf.PI/32f,Mathf.PI/32f)
        return (Mathf.PI * newOrientation + Random.Range(-Mathf.PI/32f,Mathf.PI/32f));
    }
    
}
