
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject isPlayer;
 
    [SerializeField] float timeWait;
    [SerializeField] float radius;
    private WaitForSeconds wait;
    [SerializeField] Animator anin;
    [SerializeField] Transform[] wayPoints;
   


    private NavMeshAgent nav;
    private bool isChasing;
    private int index;


    private bool chasePlayer;
    // Start is called before the first frame update
    void Start()
    {
        index = Random.Range(0, wayPoints.Length);
        wait = new WaitForSeconds(timeWait);
        isPlayer = GameObject.FindGameObjectWithTag("Player");
        anin = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(StartPatrol());

        chasePlayer = false;

    }




    // Update is called once per frame
    void Update()
    {
        anin.SetFloat("Move", nav.velocity.sqrMagnitude, 0.06f, Time.deltaTime);
        Chasing();
    }


    IEnumerator StartPatrol()
    {
        while (true)
        {

            yield return wait;
            Patrol();

        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        // Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, radius);
    }

    public void Patrol()
    {
        index = index == wayPoints.Length - 1 ? 0 : index + 1;
        float distance = Vector3.Distance(wayPoints[index].transform.position, transform.position);
        nav.SetDestination(wayPoints[index].transform.position);

        if (distance <= nav.stoppingDistance)
        {
            FaceTarget();
        }



    }

    public void Chasing()
    {

        float distance = Vector3.Distance(isPlayer.transform.position, transform.position);

        if (isPlayer != null  && distance <= radius && isChasing == false )
        {

            isChasing = true;
            
        }
        else if (distance > radius)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            nav.SetDestination(isPlayer.transform.position);

            if (distance <= nav.stoppingDistance)
            {
                FaceTarget();
            }
        }


    }

    public void FaceTarget()
    {
        Vector3 direction = (isPlayer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void OnCollisionEnter(Collision collision)
    {
    }
}
