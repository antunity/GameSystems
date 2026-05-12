using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    [GameDataDrawer(GameDataLayout.Horizontal)]
    public struct CompareToDataRuleStruct : IRule, IUseGameDataDrawer
    {
        [Tooltip("The source for the first data to compare")]
        [SerializeField] private GameDataSource source1;

        [Tooltip("The first data to compare")]
        [SerializeField] private GameDataAsset<uint> data1;

        [Tooltip("The comparison operation to perform")]
        [SerializeField] private ComparisonOperation operation;

        [Tooltip("The source for the second data to compare")]
        [SerializeField] private GameDataSource source2;

        [Tooltip("The second data to compare")]
        [SerializeField] private GameDataAsset<uint> data2;

        [Tooltip("Enable to invert the result of the comparison")]
        [SerializeField] private bool invert;

        public IGameDataBase Data1 => data1;

        public IGameDataBase Data2 => data2;

        public ComparisonOperation Operation => operation;

        public GameDataSource Source1 => source1;

        public GameDataSource Source2 => source2;

        public RuleResult Evaluate(IGameContext context)
        {
            var value1 = context.Resolve<float>(source1, data1);
            var value2 = context.Resolve<float>(source2, data2);

            bool result;
            switch (operation)
            {
                case ComparisonOperation.Greater:
                    result = value1 > value2;
                    break;
                case ComparisonOperation.GreaterOrEqual:
                    result = value1 >= value2;
                    break;
                case ComparisonOperation.Equal:
                    result = Mathf.Abs(value1 - value2) < 0.0001f;
                    break;
                case ComparisonOperation.LessOrEqual:
                    result = value1 <= value2;
                    break;
                case ComparisonOperation.Less:
                    result = value1 < value2;
                    break;
                case ComparisonOperation.NotEqual:
                    result = Mathf.Abs(value1 - value2) >= 0.0001f;
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
                    return RuleResult.MinimumRequirementViolation(data1, value1);
                case ComparisonOperation.LessOrEqual:
                case ComparisonOperation.Less:
                    return RuleResult.MaximumRequirementViolation(data1, value1);
                case ComparisonOperation.Equal:
                    if (value1 > value2)
                        return RuleResult.MaximumRequirementViolation(data1, value1);
                    else
                        return RuleResult.MinimumRequirementViolation(data1, value1);
                case ComparisonOperation.NotEqual:
                    return RuleResult.InvalidValue(data1, value1);
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
    [CreateAssetMenu(fileName = FILE_NAME.RULE_COMPARE_DATA, menuName = MENU_PATH.RULE_COMPARE_DATA)]
    public class CompareToDataRule : Rule
    {
        [SerializeField] private CompareToDataRuleStruct rule;

        public override RuleResult Evaluate(IGameContext context) => rule.Evaluate(context);
    }
}