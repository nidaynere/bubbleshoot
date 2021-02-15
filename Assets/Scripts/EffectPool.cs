using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EffectPool", menuName = "EffectPool", order = 1)]
public class EffectPool : ScriptableObject
{
    public struct Effect : System.ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }

        public Effect(Transform target)
        {
            main = target;
            audioSources = target.GetComponentsInChildren<AudioSource>(true);
            particleSystems = target.GetComponentsInChildren<ParticleSystem>(true);
        }

        private Transform main;
        private AudioSource[] audioSources;
        private ParticleSystem[] particleSystems;
    }

    public Transform[] Effects;

    private Dictionary<string, Effect[]> pool = new Dictionary<string, Effect[]>();

    public void Create(Transform holder, int poolSize)
    {
        foreach (var e in Effects)
        {
            pool.Add(e.name, new Effect[poolSize]);

            for (int i = 0; i < poolSize; i++)
            {
                
            }
        }
    }
}
