
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // determina qual objeto é player 
    [SerializeField] private GameObject playerPrefab;
    // determina quanto tempo ate o inimigo andar de um way point pra outro
    [SerializeField] float timeWait;
    [SerializeField] float radius;
    private WaitForSeconds wait;
    [SerializeField] Animator anim;
    [SerializeField] Transform[] wayPoints;
    private NavMeshAgent nav;
    private int index;
   

    
    private bool chasePlayer;
    private bool isChasing;
    private bool attackPlayer = false;

    [SerializeField] private float distAttack;

    //Não precia alterar nada aqui dentro do codigo só preencher as coisas la fora do unity

    // Start is called before the first frame update
    void Start()
    {

        // sorteia um way point pro inimigo ir
        index = Random.Range(0, wayPoints.Length);
        // o valor de espera ate andar pra outro lugar
        wait = new WaitForSeconds(timeWait);
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(StartPatrol());
        attackPlayer = false;
        chasePlayer = false;
        distAttack = nav.stoppingDistance;
    }




    // Update is called once per frame
    // Controla animação de movimento e habilita pra atar
    void Update()
    {
        // a minha animção e controlada por um bleendtree
        anim.SetFloat("Move", nav.velocity.magnitude, 0.06f, Time.deltaTime);
        Chasing();
        DoAttack();

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
        attackPlayer = false;
        float distance = Vector3.Distance(wayPoints[index].transform.position, transform.position);
        nav.SetDestination(wayPoints[index].transform.position);

        if (distance <= nav.stoppingDistance)
        {
            FaceTarget();
        }



    }

    public void Chasing()
    {

        float distance = Vector3.Distance(transform.position, playerPrefab.transform.position);

        if (playerPrefab != null  && distance <= radius && isChasing == false)
        {

            isChasing = true;
            

        }
        else if (distance > radius)
        {
            isChasing = false;
            
        }

        if (isChasing)
        {
            nav.SetDestination(playerPrefab.transform.position);

            if (distance <= nav.stoppingDistance)
            {
                FaceTarget();
            }
        }


    }

    public void DoAttack()
    {
        int distance = (int) Vector3.Distance(playerPrefab.transform.position, transform.position);

        if (playerPrefab != null && distance <= distAttack && isChasing)
        {
            anim.SetBool("doAttack", true);
            attackPlayer = true;
          
           
        }
        else if(playerPrefab != null && Vector3.Distance(transform.position, playerPrefab.transform.position) > distAttack && isChasing)
        {
            anim.SetBool("doAttack", false);
        }

        if (attackPlayer)
        {
            nav.speed = 0;
            nav.isStopped = true;
        }
        else
        {
            nav.speed = 2.5f;
            nav.isStopped = false;
        }
    }

    public void FaceTarget()
    {
        Vector3 direction = (playerPrefab.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 9f);
    }

    public void OnCollisionEnter(Collision collision)
    {




    }
}
