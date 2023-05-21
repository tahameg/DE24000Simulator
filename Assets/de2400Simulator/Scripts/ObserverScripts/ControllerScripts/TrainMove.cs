using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMove : MonoBehaviour
{
    public List<Transform> followTransformList;
    public int trainSpeed;
    int firstTrainSpeed;
    List<Point> movePointList;
    Point p1, p2;
    private bool firstState;
    private float distance;
    private float timeToReachTarget;
    int nextPoint = -1;
    private float timeElapsed;
    bool endOfPoints = false;
    public bool Isforward = true;
    public bool IsStop = false;

    // Start is called before the first frame update
    void Start()
    {
        firstTrainSpeed = trainSpeed;
        movePointList = new List<Point>();
        foreach (var item in followTransformList)
        {
            Point newPoint = new Point();
            newPoint.pointSpeed = trainSpeed;
            newPoint.positionPoint = item.position;
            movePointList.Add(newPoint);
        }
    }

    void GetNewPoint()
    {
        if (!(movePointList.Count == nextPoint + 1))
        {
            if (Isforward)
            {
                nextPoint++;
            }
            else if (nextPoint > 0)
            {
                nextPoint--;
            }
        }
        if ((movePointList.Count == nextPoint + 1) && !Isforward)
            nextPoint--;
        //else
        //{
        //    endOfPoints = true;
        //}
        //this.p1 = movePointList[nextPoint];
        this.p1 = new Point();
        this.p1.positionPoint = transform.position;
        this.p2 = movePointList[nextPoint];
        distance = Vector3.Distance(p1.positionPoint, p2.positionPoint);
        timeToReachTarget = distance / trainSpeed;

        timeElapsed = 0;
    }

    void MoveForwardTrain()
    {
        if (Isforward != firstState)
        {
            p1 = null;
            p2 = null;
            firstState = Isforward;
        }

        if (p1 == null || p2 == null)
        {
            GetNewPoint();
        }

        if (timeElapsed < timeToReachTarget)
        {
            timeElapsed += Time.deltaTime;

            //Move Train
            if (firstTrainSpeed != trainSpeed)
            {
                firstTrainSpeed = trainSpeed;
                distance = Vector3.Distance(transform.position, p2.positionPoint);
                timeToReachTarget = distance / trainSpeed;
                timeElapsed = 0;
                p1.positionPoint = gameObject.transform.position; 
                return;
            }

            //Lerp Rotatio
            transform.LookAt(p2.positionPoint);
         
            gameObject.transform.position = Vector3.Lerp(p1.positionPoint, p2.positionPoint, timeElapsed / timeToReachTarget);
            //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, p2.positionPoint, timeElapsed / timeToReachTarget);
        }
        else
        {
            timeElapsed = 0;
            GetNewPoint();
        }

        if (p1.positionPoint == p2.positionPoint)
        {
            GetNewPoint();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsStop)
        {
            MoveForwardTrain();
        }
    }
    class Point
    {
        public Vector3 positionPoint;
        public int pointSpeed;
    }
}
