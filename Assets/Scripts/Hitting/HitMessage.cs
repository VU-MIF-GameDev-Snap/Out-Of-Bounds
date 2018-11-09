using UnityEngine;

public class HitMessage
{
    public HitType HitType { get; set; }
    public int Damage { get;set; }
    public int KnockbackValue { get; set; }
    public Vector3 KnockbackDirection { get; set; }
}