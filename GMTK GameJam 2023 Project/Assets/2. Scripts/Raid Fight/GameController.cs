using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private InputActions input = null;

    public List<StackZone> StackZones = new List<StackZone>();
    [HideInInspector] public StackZone FrontStackZone;
    [HideInInspector] public StackZone BackStackZone;
    [HideInInspector] public StackZone LeftStackZone;
    [HideInInspector] public StackZone RightStackZone;
    [HideInInspector] public StackZone OuterStackZone;

    public List<Raider> allRaiders = new List<Raider>();

    public List<GameObject> killedRaiders = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        input = new InputActions();

        FrontStackZone = StackZones[0];
        BackStackZone = StackZones[1];
        LeftStackZone = StackZones[2];
        RightStackZone = StackZones[3];
        OuterStackZone = StackZones[4];
    }
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (input.PlayerController.NUM1.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[0]);
            }
        }
        if (input.PlayerController.NUM2.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[1]);
            }
        }
        if (input.PlayerController.NUM3.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[2]);
            }
        }
        if (input.PlayerController.NUM4.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[3]);
            }
        }
        if (input.PlayerController.NUM5.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[4]);
            }
        }

        if (input.PlayerController.R.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.MoveToNewStackZone(StackZones[Random.Range(0, StackZones.Count)]);
            }
        }



        if (input.PlayerController.Up.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[0] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.Down.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[1] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.Left.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[2] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.Right.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[3] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.Q.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[0], StackZones[2] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.E.WasPressedThisFrame())
        {
            List<StackZone> damagedZones = new List<StackZone>() { StackZones[0], StackZones[3] };
            BossMechanics.Instance.DamageAllInZones(damagedZones, 2);
        }

        if (input.PlayerController.Space.WasPressedThisFrame())
        {
            foreach (Raider raider in allRaiders)
            {
                raider.PerformAction();
            }
        }
    }


    public void RemoveRaiderFromAllStacks(GameObject raider)
    {
        foreach (StackZone stackZone in StackZones)
        {
            if (stackZone.UnitsInZone.Contains(raider)) stackZone.UnitsInZone.Remove(raider);
        }
    }

    public void CullDeadRaidersFromAllStacks()
    {
        foreach (GameObject raider in killedRaiders)
        {
            RemoveRaiderFromAllStacks(raider);
            allRaiders.Remove(raider.GetComponent<Raider>());
            Destroy(raider);
        }
        killedRaiders.Clear();
    }

}
