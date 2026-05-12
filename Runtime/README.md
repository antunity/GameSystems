# antunity.GameSystems

**antunity.GameSystems** is a data-driven framework for the Unity Engine designed to handle complex action logic, requirement validation, and rule systems. Built on the **antunity.GameData** toolkit, it decouples gameplay logic from specific implementations, allowing for highly composable and designer-friendly systems.

### Dependencies
This toolkit utilizes and requires [antunity.GameData](https://github.com/antUnity/GameData) to be installed.

## Core Concepts

### 1. The Action System (`GameSystem<TAction>`)
The `GameSystem` is the central hub for managing gameplay logic. It maps specific actions—defined by a generic enum (`TAction`)—to `Rules`. This allows you to change the requirements for an action (e.g., "Can I cast this spell?") directly in the Inspector without modifying C# code.

### 2. The Action Context (`IGameContext`)
Rules query an `IGameContext` for game data. The built-in context tracks three potential sources of data for any given action:
* **Environment**: The global system or manager state.
* **Instigator**: The entity performing the action (e.g., a Player or Unit).
* **Subject**: The target of the action (e.g., a Chest, an Enemy, or an Item).

### 3. Data Querying (`IGameDataProvider`)
In order for a game system to interact with an entity, the entity must implement the `IGameDataProvider` interface. This allows the context to resolve values using `antunity.GameData` assets as keys:
```csharp
public T Query<T>();
public T Query<T>(IGameDataBase data);
public void Mutate<T>(IGameDataBase data, T value);
```

### 4. Rich Rule Results (`RuleResult`)
The `RuleResult` struct provides detailed feedback beyond a simple boolean:
* **Success State**: Evaluated via `if (result)`.
* **Failure Codes**: Specific reasons (e.g., `MinimumRequirementViolation`).
* **Violation Context**: References to the specific data asset and the actual value that caused the failure, making it easy to drive UI tooltips.

---

## Rule Types

The toolkit includes several built-in `Rule` types (ScriptableObjects) that can be combined:

* **CompareToValue**: Compares a context value against a constant (e.g., `Gold >= 100`).
* **CompareToData**: Compares two values within the context (e.g., `Strength > TargetDefense`).
* **HasData**: Checks for the existence of a specific data entry or a boolean flag.
* **CompositeRule**: Combines rules using **AND** / **OR** logic.
* **GroupRule**: A registry of rules where success is returned if *all* rules in the group pass.

These rules can be created as scriptable objects (`Rules`) or serialized within another scriptable object.

---

## Implementation Example

### 1. Defining a Data-Driven Entity
Implement `IGameDataProvider` on your MonoBehaviours to allow the Action System to "read" your entity's stats.



```csharp
using UnityEngine;
using antunity.GameData;
using antunity.GameSystems;

public class UnitStats : MonoBehaviour, IGameDataProvider
{
    [SerializeField] private GameDataValues<StatAsset, float> stats;

    public T Query<T>(IGameDataBase data)
    {
        // Resolve float stats from our internal registry
        if (typeof(T) == typeof(float) && stats.ContainsIndex(data))
            return (T)(object)stats[data];
        
        return default;
    }
}
```

### 2. Evaluating an Action
Trigger logic by passing the Instigator and Subject into your `GameSystem`.

```csharp
using antunity.GameSystems;
using antunity.GameSystems.Rules;

public enum Actions { Attack, Unlock }

public void OnInteract(Unit player, Chest target)
{
    // The system automatically assembles the context using the provided queryables
    RuleResult result = actionSystem.EvaluateAction(Actions.Unlock, target, player);

    if (result)
    {
        target.Open();
    }
    else
    {
        // Provide specific feedback to the player based on the result data
        Debug.Log($"Failed: {result.FailureCode} on {result.RequiredData.GetIndex()}");
    }
}
```

---

## Technical Features

### Operator Overloading
`RuleResult` supports logical operators, allowing you to compose complex code-based rules that still provide full failure context:
```csharp
// Returns the first failure context found, or success if both pass
RuleResult combined = ruleA.Evaluate(context) && ruleB.Evaluate(context);
```

### Decoupled Logic
Since `Rule` is a ScriptableObject, you can create a "CanMove" rule once and apply it universally to your game, or create different CanMove rules for different scenarios. This allows designers to work in the editor without any coding.
