using TMPro;
using UnityEngine;

public class CubeArrows : MonoBehaviour
{
    public GameObject arrowX;
    public GameObject arrowY;
    public GameObject arrowZ;
    public GameObject arrowAll;
    public GameObject awayPoint;

    public float forceX = 1;
    public float forceY = 1;
    public float forceZ = 1;

    public float forceScaleBy = 1;

    public float flucuateSpeed = 10;
    private float radiusOfArrow = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arrowX.transform.eulerAngles = new Vector3(0, 0, 90);
        arrowY.transform.eulerAngles = new Vector3(0, 0, 0);
        arrowZ.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        flucuateTheForces();
        drawArrows();
    }

    void flucuateTheForces()
    {
        forceX += Random.Range(-Time.deltaTime * flucuateSpeed, Time.deltaTime * flucuateSpeed);
        forceY += Random.Range(-Time.deltaTime * flucuateSpeed, Time.deltaTime * flucuateSpeed);
        forceZ += Random.Range(-Time.deltaTime * flucuateSpeed, Time.deltaTime * flucuateSpeed);
    }

    void drawArrows()
    {
        arrowX.transform.localScale = new Vector3(radiusOfArrow, forceX * forceScaleBy, radiusOfArrow);
        arrowY.transform.localScale = new Vector3(radiusOfArrow, forceY * forceScaleBy, radiusOfArrow);
        arrowZ.transform.localScale = new Vector3(radiusOfArrow, forceZ * forceScaleBy, radiusOfArrow);

        arrowX.transform.position = new Vector3(arrowX.transform.localScale.y + transform.localScale.x / 2 * Mathf.Sign(forceX), 0, 0) + transform.position;
        arrowY.transform.position = new Vector3(0, arrowY.transform.localScale.y + transform.localScale.y / 2 * Mathf.Sign(forceY), 0) + transform.position;
        arrowZ.transform.position = new Vector3(0, 0, arrowZ.transform.localScale.y + transform.localScale.z / 2 * Mathf.Sign(forceZ)) + transform.position;

        /*
        Vector3 sumPointAway =  transform.localScale / 2 + new Vector3(strengthX, strengthY, strengthZ) * forceScaleBy * 2;
        awayPoint.transform.position = sumPointAway + transform.position;
        
        arrowAll.transform.position = transform.position;
        arrowAll.transform.LookAt(sumPointAway);
        arrowAll.transform.Rotate(90, 0, 0);
        arrowAll.transform.localScale = new Vector3(radiusOfArrow, Vector3.Distance(transform.position, sumPointAway), radiusOfArrow);
        */
    }
}
