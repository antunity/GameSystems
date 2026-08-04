using System;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    public enum RuleFailureCode
    {
        None,
        UnknownFailure,
        ValueInvalid,
        ValueTooLow,
        ValueTooHigh,
        DataMissing,
    }

    public struct RuleResult
    {
        public static bool operator true(RuleResult original) => original.IsSuccess;

        public static bool operator false(RuleResult original) => !original.IsSuccess;

        public static RuleResult operator !(RuleResult original) => original.IsSuccess ? ValueInvalid(original.CallerIndex, original.RequiredData, original.ActualValue) : original;

        public static RuleResult operator & (RuleResult left, RuleResult right) => !left.IsSuccess ? left : right;

        public static RuleResult operator | (RuleResult left, RuleResult right) => left.IsSuccess ? left : right;

        public readonly object CallerIndex;

        public readonly bool IsSuccess;

        public readonly RuleFailureCode FailureCode;
        
        public readonly IGameDataBase RequiredData; 

        public readonly float ActualValue;

        public readonly string Message;

        public RuleResult(object callerIndex, bool success, RuleFailureCode failureCode, IGameDataBase requiredData = null, float actualValue = 0f, string message = null)
        {
            CallerIndex = callerIndex;
            IsSuccess = success;
            FailureCode = failureCode;
            RequiredData = requiredData;
            ActualValue = actualValue;
            Message = message;
        }

        // Static constructors for ease of use
        public static RuleResult Success() => new(null, true, RuleFailureCode.None, null, 0, null);
        
        public static RuleResult Fail(object callerIndex, RuleFailureCode code, IGameDataBase requiredData, float actualValue, string message = null) => new(callerIndex, false, code, requiredData, actualValue, message);

        public static RuleResult DataMissing(object callerIndex, IGameDataBase data, float actualValue, string message = null) => new(callerIndex, false, RuleFailureCode.DataMissing, data, actualValue, message);

        public static RuleResult ValueTooHigh(object callerIndex, IGameDataBase data, float actualValue, string message = null) => new(callerIndex, false, RuleFailureCode.ValueTooHigh, data, actualValue, message);

        public static RuleResult ValueTooLow(object callerIndex, IGameDataBase data, float actualValue, string message = null) => new(callerIndex, false, RuleFailureCode.ValueTooLow, data, actualValue, message);

        public static RuleResult ValueInvalid(object callerIndex, IGameDataBase data, float actualValue, string message = null) => new(callerIndex, false, RuleFailureCode.ValueInvalid, data, actualValue, message);

        public static RuleResult UnknownFailure(object callerIndex, string message = null) => new(callerIndex, false, RuleFailureCode.UnknownFailure, null, 0, message);
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