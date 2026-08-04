using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct RuleValues
    {
        [SerializeField] private HasDataStruct hasData;

        [SerializeField] private CompareToValueStruct compareToValue;

        [SerializeField] private CompareToDataStruct compareToData;

        public HasDataStruct HasData => hasData;

        public CompareToValueStruct CompareToValue => compareToValue;

        public CompareToDataStruct CompareToData => compareToData;
    }

    [Serializable]
    public struct RuleInfo<TAction> : IRule, ICopyable<RuleInfo<TAction>> where TAction : struct
    {
        #region ICopyable

        public RuleInfo<TAction> Copy()
        {
            return new RuleInfo<TAction>
            {
                rules = rules.Copy()
            };
        }

        #endregion ICopyable

        [SerializeField] private EnumDataValues<TAction, RuleValues> rules;

        #region IRule

        public RuleResult Evaluate(IGameContext context)
        {
            foreach (var rule in rules.Values)
            {
                var result = rule.HasData.Evaluate(context);
                if (!result.IsSuccess)
                    return result;

                result = rule.CompareToValue.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
                
                result = rule.CompareToData.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
            }

            return RuleResult.Success();
        }

        #endregion IRule
    }
}