using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Jnk.TinyContainer {
public enum RegistrationScope {
    Global,
    Scene,
    GameObject
}

public enum RegistrationType {
    TypeOnly,
    InterfacesOnly,
    TypeAndInterfaces
}

public class TinyContainerRegistrar : MonoBehaviour {
    [SerializeField]
    private RegistrationScope registrationScope;
    [SerializeField]
    private RegistrationType registrationType = RegistrationType.TypeAndInterfaces;
    [SerializeField, CanBeNull]
    private MonoBehaviour monoBehaviour;

    private void Awake() {
        if (monoBehaviour == null) {
            Debug.LogError($"[{nameof(TinyContainerRegistrar)}] MonoBehaviour is not assigned on {gameObject.name}.", this);
            return;
        }
        
        var types = registrationType switch {
            RegistrationType.TypeOnly => new[] { monoBehaviour.GetType() },
            RegistrationType.InterfacesOnly => monoBehaviour.GetType().GetInterfaces(),
            RegistrationType.TypeAndInterfaces => new[] { monoBehaviour.GetType() }.Concat(monoBehaviour.GetType().GetInterfaces()).ToArray(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var container = registrationScope switch {
            RegistrationScope.Global => TinyContainer.Global,
            RegistrationScope.Scene => TinyContainer.ForSceneOf(monoBehaviour),
            RegistrationScope.GameObject => TinyContainer.For(monoBehaviour),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var type in types) {
            container.Register(type, monoBehaviour);
        }
    }
}
}