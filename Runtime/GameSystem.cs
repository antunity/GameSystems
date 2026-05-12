using UnityEngine;
using antunity.GameData;
using antunity.GameSystems.Rules;

namespace antunity.GameSystems
{
    public interface IGameSystemBase
    {
        public IGameDataProvider Environment { get; set; }

        public void Initialize(GameSystemTemplate template);
    }

    public interface IGameSystem<TAction> : IGameSystemBase where TAction : struct
    {
        public RuleResult EvaluateAction(TAction action, IGameDataProvider subject, IGameDataProvider instigator = null);

        public IGameContext GetActionContext(TAction action);

        public void SetActionContext(IGameContext context);

        public void SetActionRule(TAction action, Rule rule);
    }

    public abstract class GameSystem<TAction> : MonoBehaviour, IGameSystem<TAction> where TAction : struct
    {
        private GameDataRegistry<IGameContext> actionContexts = new();

        [SerializeField] private EnumDataValues<TAction, Rule> rules = new();

        #region IGameSystemBase

        public IGameDataProvider Environment { get; set; }

        public abstract void Initialize(GameSystemTemplate template);

        #endregion IGameSystemBase

        #region IGameSystem

        public RuleResult EvaluateAction(TAction action, IGameDataProvider subject, IGameDataProvider instigator = null)
        {
            if (!rules.ContainsIndex(action))
                return RuleResult.Success();

            var actionRule = rules[action];

            if (!actionRule)
                return RuleResult.Success();

            if (!actionContexts.TryGetData(action, out IGameContext context))
            {
                context = new GameContext<TAction>(action);
                actionContexts.Add(context);
            }

            // Default to the system's environment
            context.Environment ??= Environment;

            // Assign subject
            context.Subject = subject;

            // Assign the instigator if defined
            if (instigator != null)
                context.Instigator = instigator;
            else
                context.Instigator = context.Environment;
    
            return actionRule.Evaluate(context);
        }

        public IGameContext GetActionContext(TAction action) => actionContexts.TryGetData(action, out var context) ? context : default;

        public void SetActionContext(IGameContext context) => actionContexts.Add(context);

        public void SetActionRule(TAction action, Rule rule)
        {
            if (rules.ContainsIndex(action))
                rules[action] = rule;
            else
                rules.Add(action, rule);
        }

        #endregion IGameSystem
    }
}