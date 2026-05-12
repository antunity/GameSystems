using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public enum ComparisonOperation { Greater, GreaterOrEqual, Equal, LessOrEqual, Less, NotEqual }

    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct CompareToValueRuleStruct : IRule, IUseGameDataDrawer
    {
        [Tooltip("The source for the data to compare")]
        [SerializeField] private GameDataSource source;

        [Tooltip("The data to compare")]
        [SerializeField] private GameDataAsset<uint> data;

        [Tooltip("The comparison operation to perform")]
        [SerializeField] private ComparisonOperation operation;

        [Tooltip("The constant value to compare to")]
        [SerializeField] private float value;

        [Tooltip("Enable to invert the result of the comparison")]
        [SerializeField] private bool invert;

        public RuleResult Evaluate(IGameContext context)
        {
            var value = context.Resolve<float>(source, data);
            bool result;
            switch (operation)
            {
                case ComparisonOperation.Greater:
                    result = value > this.value;
                    break;
                case ComparisonOperation.GreaterOrEqual:
                    result = value >= this.value;
                    break;
                case ComparisonOperation.Equal:
                    result = Mathf.Abs(value - this.value) < 0.0001f;
                    break;
                case ComparisonOperation.LessOrEqual:
                    result = value <= this.value;
                    break;
                case ComparisonOperation.Less:
                    result = value < this.value;
                    break;
                case ComparisonOperation.NotEqual:
                    result = Mathf.Abs(value - this.value) >= 0.0001f;
                    break;
                default:
                    result = false;
                    break;
            }

            if (invert)
                result = !result;
            
            if (result)
                return RuleResult.Success();

            ComparisonOperation effectiveOperation = operation;
            if (invert)
                effectiveOperation = GetInverseOperation(operation);
            
            switch (effectiveOperation)
            {
                case ComparisonOperation.GreaterOrEqual:
                case ComparisonOperation.Greater:
                    return RuleResult.MinimumRequirementViolation(data, value);
                case ComparisonOperation.LessOrEqual:
                case ComparisonOperation.Less:
                    return RuleResult.MaximumRequirementViolation(data, value);
                case ComparisonOperation.Equal:
                    if (value > this.value)
                        return RuleResult.MaximumRequirementViolation(data, value);
                    else
                        return RuleResult.MinimumRequirementViolation(data, value);
                case ComparisonOperation.NotEqual:
                    return RuleResult.InvalidValue(data, value);
                default:
                    return RuleResult.UnknownFailure();
            }
        }

        // Helper function to calculate inverse operator
        private ComparisonOperation GetInverseOperation(ComparisonOperation op)
        {
            return op switch
            {
                ComparisonOperation.Greater => ComparisonOperation.LessOrEqual,
                ComparisonOperation.GreaterOrEqual => ComparisonOperation.Less,
                ComparisonOperation.Less => ComparisonOperation.GreaterOrEqual,
                ComparisonOperation.LessOrEqual => ComparisonOperation.Greater,
                ComparisonOperation.Equal => ComparisonOperation.NotEqual,
                ComparisonOperation.NotEqual => ComparisonOperation.Equal,
                _ => op,
            };
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_COMPARE_VALUE, menuName = MENU_PATH.RULE_COMPARE_VALUE)]
    public class CompareToValueRule : Rule
    {
        [SerializeField] private CompareToValueRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}