using System;
using UnityEngine;
public class UnitStats : MonoBehaviour
{
    [Header("Unit State")]
    [SerializeField]
    private float baseMoveSpeed = 5f;
    [SerializeField]
    private float baseMaxHp = 100f;
    [SerializeField]
    private float baseCurHp = 100f;
    [SerializeField]
    private float baseArmor = 1f;
    [SerializeField]
    private float baseAttackPower = 1f;

    //프러퍼티
    public float MoveSpeed{get; private set;}
    public float MaxHp{get; private set;}

    public float CurrentHp{get; private set;}
    public float Armor {get; private set;}

    public float AttackPower{get; private set;} //외부 수정 불가능

    
    private void Start() //생성과 동시에 적용되도록 할거임 
    {
        InitializeStats();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeStats()
    {
        MoveSpeed = baseMoveSpeed;
        MaxHp = baseMaxHp;
        CurrentHp = MaxHp;
        Armor = baseArmor;
        AttackPower = baseAttackPower;
    }
    public void AddMoveSpeed(float _amount)
    {
        MoveSpeed += _amount;
        MoveSpeed = Mathf.Max(0,MoveSpeed);
    }

    public void AddMaxHP(float _amount)
    {

        float ratio = CurrentHp / MaxHp;

        MaxHp += _amount;
        MaxHp = Mathf.Max(1f,MaxHp);

        CurrentHp = MaxHp * ratio;

    }

    public void AddArmor(float _amount)
    {
        //나중에 곱적용으로 변경
        Armor += _amount;
        Armor = Mathf.Max(baseArmor,Armor);
    }


    public void TakeDamage(float _amount)
    {
        
    }


}
