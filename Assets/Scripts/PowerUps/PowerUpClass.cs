using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpClass : MonoBehaviour
{
    bool inCooldown;
    [SerializeField] protected float duration = 10f;
    [SerializeField] protected float cooldown = 5f;
    float activeTime = 0;

    [SerializeField] protected int nivel = 1;

    private void Start() {
        nivel = 1;
        OnCooldownEnds();
    }

    private void Update() {
        activeTime += Time.deltaTime;

        if(!inCooldown && activeTime < duration) {
            OnUpdate();
            return;
        }

        if(!inCooldown && activeTime > duration) {
            inCooldown = true;
            OnCooldownStarts();

            activeTime = 0;
            return;
        }

        if(inCooldown && activeTime > cooldown) {
            inCooldown = false;
            OnCooldownEnds();

            activeTime = 0;
            return;
        }
    }

    protected virtual void OnCooldownStarts() { }
    protected virtual void OnCooldownEnds() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnUpgrade() { }

    public void Upgrade() {
        nivel++;
        OnUpgrade();
    }

    public void Upgrade(int _n) {
        nivel = _n;
        OnUpgrade();
    }
}
