using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;
    float progressFactor;
    float progress;
    [SerializeField] Tile tileFrom;
    Tile tileTo;
    Vector3 positionFrom;
    Vector3 positionTo;
    Quaternion rotationFrom;
    Quaternion rotationTo;
    Direction directionFrom;
    Direction directionTo;
    DirectionChange directionChange;
    float positionOffset;

    bool gameStarted = false;

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float offset = Random.value - 0.5f;
            OnSpawn(offset);
            gameStarted = true;
        }
        if (gameStarted)
        {
            if (tileFrom.NextOnPath == null)
            {
                Destroy(gameObject);
                return;
            }
            progress += Time.deltaTime * progressFactor * speed;
            if (progress > 1)
            {
                tileFrom = tileTo;
                tileTo = tileFrom.NextOnPath;
                PrepareNextMovement();
                progress = 0;
            }
            if (directionChange == DirectionChange.None)
            {
                transform.position = Vector3.Lerp(positionFrom, positionTo, progress);
            }
            else
            {
                float rotation = Quaternion.Lerp(rotationFrom, rotationTo, progress).eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }
        }
        
    }

    public void OnSpawn(/*Tile tile,*/ float positionOffset)
    {
        //tileFrom = tile;
        tileTo = tileFrom.NextOnPath;
        this.positionOffset = positionOffset;
        progress = 0;
        PrepareInitialMovement();
    }

    public void PrepareInitialMovement()
    {
        progressFactor = 2;
        positionFrom = tileFrom.transform.position;
        positionTo = tileFrom.exitPoint;
        directionFrom = directionTo = tileFrom.pathDirection;
        directionChange = DirectionChange.None;
        rotationFrom = rotationTo = transform.rotation;
    }

    public void PrepareNextMovement()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.exitPoint;
    }
    public void PrepareMoveForward()
    {

    }
    public void PrepareTurnRight()
    {

    }
    public void PrepareTurnLeft()
    {

    }
    public void PrepareTurnAround()
    {

    }
}
