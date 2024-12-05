﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCars : MonoBehaviour
{
    public float speed;
    bool GameStart;
    public GameObject CircleGizmo;
    AudioSource audioSource;
    float randTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameStart = false;
        StartCoroutine(WaitStart());
        StartCoroutine(Horn());

    }

    // Update is called once per frame
    void Update()
    {
        if(GameStart && !GameManager.instance.finished)
        transform.Translate(Vector3.forward * Time.deltaTime *speed);

    }

    IEnumerator WaitStart() {
        yield return new WaitForSeconds(2f);
        GameStart = true;
        CircleGizmo.SetActive(false);
    }

    IEnumerator Horn() {
        randTime = Random.Range(3, 10);
        yield return new WaitForSeconds(randTime);
        audioSource.Play();
        StartCoroutine(Horn());
    }
}
