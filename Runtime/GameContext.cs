using System;
using antunity.GameData;
using antunity.GameSystems.Rules;

namespace antunity.GameSystems
{
    public enum GameDataSource
    {
        Environment,    // e.g. ability book, inventory owner
        Instigator,     // e.g. an external trigger for an action, e.g. a customer in a vendor transaction. if omitted, it matches the environment
        Subject,        // e.g. ability, item, ability
    }

    public interface IGameDataProvider
    {
        T Query<T>() => throw new NotImplementedException();

        T Query<T>(IGameDataBase data) => throw new NotImplementedException();

        void Mutate<T>(IGameDataBase data, T value) => throw new NotImplementedException();
    }

    public interface IGameContext : IGameDataBase
    {
        IGameDataProvider Instigator { get; set; }

        IGameDataProvider Subject { get; set; }

        IGameDataProvider Environment { get; set; }

        T Resolve<T>(GameDataSource source, IGameDataBase gameData);
    }

    public class GameContext<TAction> : GameData<TAction>, IGameContext where TAction : struct
    {
        public GameContext(TAction index) : base(index) { }

        #region IGameContext

        public IGameDataProvider Instigator { get; set; } = null;

        public IGameDataProvider Subject { get; set; } = null;

        public IGameDataProvider Environment { get; set; } = null;

        public TResult Resolve<TResult>(GameDataSource source, IGameDataBase data)
        {
            if (data is IRuleMetric<TResult> metric)
                return metric.Calculate(this);

            IGameDataProvider sourceTarget;
            switch (source)
            {
                case GameDataSource.Environment:
                    sourceTarget = Environment;
                    break;
                case GameDataSource.Instigator:
                    sourceTarget = Instigator;
                    break;
                case GameDataSource.Subject:
                    sourceTarget = Subject;
                    break;
                default:
                    throw new NotSupportedException(nameof(source));
            }

            if (sourceTarget == null)
                throw new NullReferenceException($"{Index}: {nameof(GameDataSource)} '{source}' is not specified in this context.");

            return sourceTarget.Query<TResult>(data);
        }

        #endregion IGameContext
    }
}