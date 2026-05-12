using System;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public enum RuleFailureCode
    {
        None,
        UnknownFailure,
        InvalidValue,
        MinimumRequirementViolation,
        MaximumRequirementViolation,
        DataPresenceViolation,
    }

    public struct RuleResult
    {
        public static bool operator true(RuleResult original) => original.IsSuccess;

        public static bool operator false(RuleResult original) => !original.IsSuccess;

        public static RuleResult operator !(RuleResult original) => original.IsSuccess ? InvalidValue(original.RequiredData, original.ActualValue) : original;

        public static RuleResult operator & (RuleResult left, RuleResult right) => !left.IsSuccess ? left : right;

        public static RuleResult operator | (RuleResult left, RuleResult right) => left.IsSuccess ? left : right;

        public readonly bool IsSuccess;

        public readonly RuleFailureCode FailureCode;
        
        public readonly IGameDataBase RequiredData; 

        public readonly float ActualValue; 

        public RuleResult(bool success, RuleFailureCode failureCode, IGameDataBase requiredData = null, float actualValue = 0f)
        {
            IsSuccess = success;
            FailureCode = failureCode;
            RequiredData = requiredData;
            ActualValue = actualValue;
        }

        // Static constructors for ease of use
        public static RuleResult Success() => new(true, RuleFailureCode.None, null, 0);
        
        public static RuleResult Fail(RuleFailureCode code, IGameDataBase requiredData, float actualValue) => new(false, code, requiredData, actualValue);

        public static RuleResult DataPresenceViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.DataPresenceViolation, data, actualValue);

        public static RuleResult MaximumRequirementViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.MaximumRequirementViolation, data, actualValue);

        public static RuleResult MinimumRequirementViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.MinimumRequirementViolation, data, actualValue);

        public static RuleResult InvalidValue(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.InvalidValue, data, actualValue);

        public static RuleResult UnknownFailure() => new(false, RuleFailureCode.UnknownFailure, null, 0);
    }

    public interface IRule
    {
        RuleResult Evaluate(IGameContext context);
    }

    [Serializable]
    public abstract class Rule : GameDataAsset<uint>, IRule
    {
        public abstract RuleResult Evaluate(IGameContext context);
    }
}