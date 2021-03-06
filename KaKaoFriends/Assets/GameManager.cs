﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject tile;
    public int m;
    public int n;
    Queue<Vector2> q = new Queue<Vector2>();
    Transform[,] block;
    Vector2[,] blockPos;
    int[,] map;
    public string[] str;
    string[] splitStr;
    bool[,] isSquare;

    //[0]→ : (1, 0), [4]← : (-1, 0), [6]↑ : (0, 1), [2]↓ : (0, -1)
    //[1]↘ : (1, -1), [7]↗ : (1, 1), [5]↖ : (-1, 1), [3]↙ : (-1, -1)
    int[] directionX = { 1, 1, 0, -1, -1, -1, 0, 1 };
    int[] directionY = { 0,-1,-1, -1,  0,  1, 1, 1 };

    Vector2 hitPoint = new Vector2();

    int count = 0;

    int num;

    private void Awake()
    {
        block = new Transform[m, n];
        blockPos = new Vector2[m, n];
        map = new int[m, n];
        isSquare = new bool[m, n];

        num = m - 1;
    }
    // Use this for initialization
    void Start () {
        for (int i = 0; i < m; i++)
        {
            if(num >= 0)
            { 
                splitStr = str[num].Split(',');
                num--;
            }
            for(int j = 0; j < n; j++)
            {
                GameObject instance = Instantiate(tile);
                instance.transform.position = new Vector2(j, i);
                //block[i, j] = GameObject.Find("GameObject (" + i + ")").transform.Find("Quad (" + j + ")");
                //block[i, j] = instance.transform.Find("Quad (" + j + ")");
                block[i, j] = instance.transform;
                blockPos[i, j] = block[i, j].position;
                block[i, j].gameObject.tag = splitStr[j];
                map[i, j] = 0;
                isSquare[i, j] = false;

                if (block[i, j].gameObject.CompareTag("T"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 1);
                if (block[i, j].gameObject.CompareTag("A"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1);
                if (block[i, j].gameObject.CompareTag("N"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 1);
                if (block[i, j].gameObject.CompareTag("R"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 0);
                if (block[i, j].gameObject.CompareTag("F"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
                if (block[i, j].gameObject.CompareTag("C"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
                if (block[i, j].gameObject.CompareTag("M"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(25, 0, 0);
                if (block[i, j].gameObject.CompareTag("J"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 25, 0);
                if (block[i, j].gameObject.CompareTag("B"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 25);
                if (block[i, j].gameObject.CompareTag("D"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(25, 0, 25);
                if (block[i, j].gameObject.CompareTag("E"))
                    block[i, j].gameObject.GetComponent<MeshRenderer>().material.color = new Color(25, 25, 25);
            }
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                hitPoint = hit.collider.gameObject.transform.position;
                //hitPoint.y = hit.collider.gameObject.transform.position.y;
                q.Enqueue(hitPoint);
                map[(int)hitPoint.y, (int)hitPoint.x] = 1;
                

                //print("(" + hitPoint.x + ", " + hitPoint.y + ")");

                while(q.Count != 0)
                {
                    hitPoint = q.Dequeue();

                    for (int i = 0; i < 8; i++)
                    {
                        int newHitPointX = (int)hitPoint.x + directionX[i];
                        int newHitPointY = (int)hitPoint.y + directionY[i];
                        Vector2 newPoint = new Vector2(newHitPointX, newHitPointY);

                        if (newHitPointX >= 0 && newHitPointX <= m - 1 && newHitPointY >= 0 && newHitPointY <= m - 1)
                        {
                            if (block[newHitPointY, newHitPointX].gameObject.activeSelf)
                            {
                                if (block[newHitPointY, newHitPointX].CompareTag(hit.collider.gameObject.tag) && map[newHitPointY, newHitPointX] != 1)
                                {
                                    q.Enqueue(newPoint);
                                    map[newHitPointY, newHitPointX] = 1;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < m - 1; j++)
                {
                    if(map[i,j] != 0 && map[i,j] == map[i,j + 1] && map[i,j] == map[i+1, j] && map[i,j] == map[i+1, j+1])
                    {
                        isSquare[i, j] = true;
                        isSquare[i, j + 1] = true;
                        isSquare[i + 1, j] = true;
                        isSquare[i + 1, j + 1] = true;
                    }
                }
            }

            
            
        }
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (isSquare[i, j] == true)
                {
                    count++;
                    block[i, j].gameObject.SetActive(false);
                    isSquare[i, j] = false;
                }
                map[i, j] = 0;
            }
        }
        for (int i = m - 1; i > 0; i--)
        {
            for (int j = 0; j < m; j++)
            {
                if (block[i, j].gameObject.activeSelf == true)
                {
                    if (block[i - 1, j].gameObject.activeSelf == false)
                    {
                        Transform temp;
                        
                        block[i, j].Translate(0, -0.1f, 0);

                        if ((Vector2)block[i, j].position == blockPos[i - 1, j])
                        {
                            temp = block[i - 1, j];
                            block[i - 1, j] = block[i, j];
                            block[i, j] = temp;
                            break;
                        }
                    }
                }
            }
        }
        
        //print(count);
    }
    
}
