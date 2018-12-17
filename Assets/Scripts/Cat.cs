using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour {

    private Animator anim;
    private int speedHash = Animator.StringToHash("speed");
    private int lookHash = Animator.StringToHash("look");
    private int walkStateHash = Animator.StringToHash("Base Layer.Walk");
    private int idleStateHash = Animator.StringToHash("Base Layer.Idle_A");
    private int lookStateHash = Animator.StringToHash("Base Layer.Idle_B");

    private float timeOfLastAction;
    private const int SUCCESS = 0;
    private const int FAILURE = 1;
    private const int RUNNING = 2;

    private Vector3 targetDirection;
    private Vector3 targetLocation;

    private float speed = 0f;
    private float rotationSpeed = 70f; //rotation speed in deg/s

    private Vector3 lastTile = Vector3.zero;

    private const float WALK_SPEED = 1f;
    private const float IDLE_TIME = 2f;
    private const float PAUSE_TIME = 1f;

    private Stack<System.Func<int>> actions = new Stack<System.Func<int>>();

    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        timeOfLastAction = Time.time;
    }

    // Update is called once per frame
    void Update() {
        act();
    }

    /**
     * Sets the cat's speed to the given parameter. 0 is not moving.
     */
    public void setSpeed(float speed)
    {
        anim.SetFloat(speedHash, speed);
        this.speed = speed;
    }

    /**
     * Checks if the cat is not moving
     */
    public bool isIdle()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.fullPathHash == idleStateHash;
    }

    /**
     * Checks if the cat is walking
     */
    public bool isWalking()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.fullPathHash == walkStateHash;
    }

    /**
     * Makes the cat do an action
     */
    private void act()
    {
        if(actions.Count == 0)
        {
            bool choice = UnityEngine.Random.value > 0.8f;
            if (choice)
            {
                wait();
            }
            else
            {
                move();
            }
        }
        else
        {
            System.Func<int> current = actions.Peek();
            int state = current();
            if(state == SUCCESS || state == FAILURE)
            {
                actions.Pop();
            }
        }
    }

    /**
     * Makes the cat move
     */
    private void move()
    {
        actions.Push(stop);
        actions.Push(moveToTarget);
        actions.Push(rotateToTarget);
        actions.Push(walk);
        actions.Push(setTarget);
    }

    /**
     * Makes the cat wait
     */
    private void wait()
    {
        actions.Push(idle);
        actions.Push(stop);
    }

    private void clearActions()
    {
        while(actions.Count > 0)
        {
            actions.Pop();
        }
    }

    private Vector3 getCenterOfCurrentTile()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
        Ray ray = new Ray(origin, -Vector3.up);

        Physics.Raycast(ray, out hit);

        return hit.transform.position;
    }

    private bool atCenterOfTile()
    {
        Vector3 center = getCenterOfCurrentTile();

        return (Mathf.Abs(transform.position.x - center.x) < 0.1) && (Mathf.Abs(transform.position.z - center.z) < 0.1);
    }

    /**
     * Checks if the cat is at an intersection of passages
     */
     private bool checkSides()
    {
        RaycastHit rhit;
        RaycastHit lhit;
        Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
        Ray rray = new Ray(origin, transform.right);
        Ray lray = new Ray(origin, -transform.right);

        //Debug.DrawRay(origin, transform.right, Color.green);
        //Debug.DrawRay(origin, -transform.right, Color.green);

        Physics.Raycast(rray, out rhit);
        Physics.Raycast(lray, out lhit);

        return (rhit.distance > 1f) || (lhit.distance > 1f);
    }

    /**
     * Makes the cat start walking
     */
    private int walk()
    {
        setSpeed(WALK_SPEED);
        timeOfLastAction = Time.time;
        return SUCCESS;
    }

    /**
     * Makes the cat stop
     */
    private int stop()
    {
        setSpeed(0f);
        timeOfLastAction = Time.time;
        return SUCCESS;
    }

    /**
     * Makes the cat idle for IDLE_TIME seconds.
     */
    private int idle() //returns success after the cat has idled for 10 seconds
    {
        float current = Time.time;

        if (current - timeOfLastAction >= IDLE_TIME)
        {
            timeOfLastAction = Time.time;
            return SUCCESS;
        }
        return RUNNING;
    }

    private int pause()
    {
        float current = Time.time;

        if (current - timeOfLastAction >= PAUSE_TIME)
        {
            timeOfLastAction = Time.time;
            return SUCCESS;
        }
        return RUNNING;
    }

    /**
     * Makes the cat look left and right
     */
    private void look()
    {
        anim.SetTrigger(lookHash);
    }

    /**
     * sets targetDirection to the direction to which the cat must rotate
     */
    private int setTarget()
    {
        RaycastHit fhit;
        RaycastHit rhit;
        RaycastHit lhit;
        RaycastHit bhit;
        Vector3 origin = transform.position + new Vector3(0f, 0.5f, 0f);
        Ray fray = new Ray(origin, transform.forward);
        Ray rray = new Ray(origin, transform.right);
        Ray lray = new Ray(origin, -transform.right);
        Ray bray = new Ray(origin, -transform.forward);

        Physics.Raycast(fray, out fhit);
        Physics.Raycast(rray, out rhit);
        Physics.Raycast(lray, out lhit);
        Physics.Raycast(bray, out bhit);

        List<RaycastHit> hitList = new List<RaycastHit>();

        if ((fhit.distance > 0.6f))
        {
            hitList.Add(fhit);
        }
        if ((rhit.distance > 0.6f))
        {
            hitList.Add(rhit);
        }
        if ((lhit.distance > 0.6f))
        {
            hitList.Add(lhit);
        }
        if ((bhit.distance > 0.6f))
        {
            hitList.Add(bhit);
        }

        if((fhit.distance > 0.6f))
        {
            hitList.Add(fhit);
        }
        if ((rhit.distance > 0.6f))
        {
            hitList.Add(rhit);
        }
        if ((lhit.distance > 0.6f))
        {
            hitList.Add(lhit);
        }

        if(hitList.Count == 0)
        {
            hitList.Add(bhit);
        }

        int index = UnityEngine.Random.Range(0, hitList.Count); //pick a random hit
        targetDirection = hitList[index].transform.position - transform.position;
        targetLocation = hitList[index].transform.position;
        targetDirection.y = transform.position.y;
        targetLocation.y = transform.position.y;

        timeOfLastAction = Time.time;
        return SUCCESS;
    }

    /**
     * Rotates the cat to face targetDirection
     */
    private int rotateToTarget()
    {

        /*if(speed == 0f)
        {
            setSpeed(WALK_SPEED);
        }*/

        //Vector3 targetDir = target.position - transform.position;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        float step = Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;

        if (stateInfo.fullPathHash == walkStateHash)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, step, 0);
            //newDir.y = transform.rotation.y;

            transform.rotation = Quaternion.LookRotation(newDir);
        }

        if(Vector3.Angle(transform.forward, targetDirection) < 0.5f)
        {
            //setSpeed(0f);
            timeOfLastAction = Time.time;
            return SUCCESS;
        }

        return RUNNING;
    }

    /**
     * moves the cat to targetLocation
     */
    private int moveToTarget()
    {

        /*if(speed == 0)
        {
            setSpeed(WALK_SPEED);
        }*/

         if (checkSides())
         {
            if (atCenterOfTile())
            {
                Vector3 center = getCenterOfCurrentTile();
                if (center != lastTile)
                {
                    //distanceMoved = 0f;
                    look();
                    lastTile = center;
                    return FAILURE;
                }
            }
         }


        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.fullPathHash == walkStateHash)
        {
            float rotStep = Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, rotStep, 0);
            //newDir.y = transform.rotation.y;

            transform.rotation = Quaternion.LookRotation(newDir);

            float step = speed * Time.deltaTime;
            //distanceMoved += step;

            transform.position = Vector3.MoveTowards(transform.position, targetLocation, step);
        }

        if(Vector3.Distance(transform.position, targetLocation) < 0.9f)
        {
            /*if (speed != 0f)
            {
                setSpeed(0f);
            }
            if (isWalking())
            {
                return RUNNING;
            }*/
            //setSpeed(0f);
            timeOfLastAction = Time.time;
            //distanceMoved = 0f;
            lastTile = Vector3.zero;
            return SUCCESS;
        }

        return RUNNING;
    }
}
