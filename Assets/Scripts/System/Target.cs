using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Quaternion = UnityEngine.Quaternion;

public class Target : MonoBehaviour
{
    public GameObject ChipModel; //I added for drop chip

    public float health = 5.0f;
    public int pointValue;
    public bool canDamage;
    public float AlertDistance;

    public ParticleSystem DestroyedEffect;

    [Header("Audio")]
    public RandomPlayer HitPlayer;
    public AudioSource IdleSource;
    
    public bool Destroyed => m_Destroyed;

    bool m_Destroyed = false;
    float m_CurrentHealth;

    GameObject TargetObject;                // reference to player or target
    Vector3 moveVector;                     // current movement
    CharacterController TargetController;    // character controller for movement
    bool bAlive;                            // is target alive
    float moveSpeed = 2.0f;                        // how fast to move;

    void Awake()
    {
        Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Target"));
    }

    void Start()
    {
        if(DestroyedEffect)
            PoolSystem.Instance.InitPool(DestroyedEffect, 16);
        
        m_CurrentHealth = health;
        if(IdleSource != null)
            IdleSource.time = Random.Range(0.0f, IdleSource.clip.length);

        TargetObject = GameObject.FindGameObjectWithTag("Player");
        TargetController = GetComponent<CharacterController>();
        bAlive = true;
    }

    // Update is called once per frame
    private void Update()
    {
        // look at target X & Z, but enemy's Y
        Vector3 myLookVec = TargetObject.transform.position;
        myLookVec.y = transform.position.y;
        transform.LookAt(myLookVec);

        float TargetDistance = Vector3.Distance(this.transform.position, TargetObject.transform.position);

        if ((TargetDistance <= 2.0f) && bAlive && canDamage)
        {
            bAlive = false;

            Invoke("Reload", 2);  // wait 2 seconds, then reload level
        }

        if ((AlertDistance > 0) && (bAlive))  // only chase if it has an AlertDistance
        {
            // stay at location until Player moves into AlertDistance
            if (TargetDistance <= AlertDistance)
            {
                // set forward/backward movement based on input
                moveVector = new Vector3(0, 0, moveSpeed * Time.deltaTime);

                // move foward or backward based on rotation direction, up or down with gravity
                moveVector = transform.rotation * moveVector;   // multiply by rotation

                // move in the forward facing direction (already rotated toward player), by moveSpeed
                TargetController.Move(moveVector);
            }
        }
    }

    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
   

    public void Got(float damage)
    {
  

        m_CurrentHealth -= damage;
        
        if(HitPlayer != null)
            HitPlayer.PlayRandom();
        
        if(m_CurrentHealth > 0)
            return;

        Vector3 position = transform.position;

        void DropChip()
        {
            Vector3 position = transform.position; //position of enemy 
            GameObject chip = Instantiate(ChipModel, position + new Vector3(.0f, 2f, 0f), Quaternion.identity);
            chip.SetActive(true);
        }

        //the audiosource of the target will get destroyed, so we need to grab a world one and play the clip through it
        if (HitPlayer != null)
        {
            var source = WorldAudioPool.GetWorldSFXSource();
            source.transform.position = position;
            source.pitch = HitPlayer.source.pitch;
            source.PlayOneShot(HitPlayer.GetRandomClip());
        }

        if (DestroyedEffect != null)
        {
            var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
            effect.time = 0.0f;
            effect.Play();
            effect.transform.position = position;
        }
        DropChip();
        m_Destroyed = true;
        
        gameObject.SetActive(false);
       
        GameSystem.Instance.TargetDestroyed(pointValue);
    }
}
