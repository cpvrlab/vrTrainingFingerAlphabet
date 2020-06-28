using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballToSpawn;
    public Material materialOfBall;
    public bool randomizeScale;
    public bool randomizeMovement;

    public VRUISliderBehaviour slider;

    [SerializeField]
    private float power;

    public void SpawnBall()
    {
        GameObject ball = Instantiate(ballToSpawn, transform.position, Quaternion.identity);
        if (randomizeScale)
            ball.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
        if (randomizeMovement && ball.GetComponent<Rigidbody>())
            ball.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.1f, 1f), Random.Range(0.1f, 1f), Random.Range(0.1f, 1f)) * power, ForceMode.Impulse);
        ball.GetComponent<MeshRenderer>().material = materialOfBall;
        Destroy(ball, 1f);
    }

    public void SpawnBallWithOwnMaterial(Material material)
    {
        GameObject ball = Instantiate(ballToSpawn, transform.position, Quaternion.identity);
        if (randomizeScale)
            ball.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
        if (randomizeMovement && ball.GetComponent<Rigidbody>())
        {
            ball.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.1f, 1f), Random.Range(0.1f, 1f), Random.Range(0.1f, 1f)) * power, ForceMode.Impulse);
        }
        ball.GetComponent<MeshRenderer>().material = material;
        Destroy(ball, 3f);
    }

    public void SpawnBallWithOwnMaterialAndPower(Material material)
    {
        GameObject ball = Instantiate(ballToSpawn, transform.position, Quaternion.identity);
        if (randomizeScale)
            ball.transform.localScale = Vector3.one * Random.Range(0.1f, 1f);
        ball.GetComponent<Rigidbody>().AddForce(transform.forward * power, ForceMode.Impulse);
        ball.GetComponent<MeshRenderer>().material = material;
        Destroy(ball, 10f);
    }

    public float Power
    {
        get { return power; }
        set { power = value; }
    }

    public void SetPowerWithSlider()
    {
        power = slider.CurrentValue;
    }
}
