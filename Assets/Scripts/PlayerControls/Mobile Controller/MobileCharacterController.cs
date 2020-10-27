using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;


[RequireComponent(typeof(NavMeshAgent))]

public class MobileCharacterController : MonoBehaviour
{
    public Camera playerCamera;
    public Vector3 cameraOffset;
    public GameObject targetIndicatorPrefab;
    NavMeshAgent agent;
    GameObject targetObject;
   
    public Animator Anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        Assert.IsNotNull(agent);
        Assert.IsNotNull(Anim);

        if (targetIndicatorPrefab)
        {
            targetObject = Instantiate(targetIndicatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            targetObject.SetActive(false);
        }
    }

    void Update()
    {

        if (agent.isStopped)
        {
            Debug.Log("On the point");
        }
        else
        {
            Debug.Log("Moving to the point");
        }

#if (UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1) && !UNITY_EDITOR
            //Handle mobile touch input
            for (var i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    MoveToTarget(touch.position);
                }
            }
#else
        //Handle mouse input
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MoveToTarget(Input.mousePosition);
        }
#endif

        //Camera follow
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, transform.position + cameraOffset, Time.deltaTime * 7.4f);
        playerCamera.transform.LookAt(transform);
    }

    void MoveToTarget(Vector2 posOnScreen)
    {
        Ray screenRay = playerCamera.ScreenPointToRay(posOnScreen);

        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit, 75))
        {
            agent.destination = hit.point;

            //Show marker where we clicked
            if (targetObject)
            {
                targetObject.transform.position = agent.destination;
                targetObject.SetActive(true);
            }
        }
    }
}