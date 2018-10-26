using Autofac;
using AutoMapper;
using BlackJack.BusinessLogic.Automapper;
using BlackJack.BusinessLogic.Interfaces;
using BlackJack.BusinessLogic.Providers;
using BlackJack.BusinessLogic.Services;
using BlackJack.BusinessLogic.Utilities;
using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Interfaces;
using BlackJack.DataAccess.Repositories.DapperRepositories;
using BlackJack.DataAccess.Repositories.EFRepositories;
using BlackJack.Exceptions.Interfaces;
using BlackJack.Exceptions.Loggers;
using BlackJack.Shared.Configurations;
using BlackJack.Shared.Enums;
using System.Configuration;

namespace BlackJack.BusinessLogic.Configurations
{
    public class AutofacServiceConfig
    {
        private RepositoryConfig _repositoryConfig = RepositoryConfig.EF;

        private string _connectionStringName;

        public AutofacServiceConfig()
        {
            _connectionStringName = ConfigSettings.ConnectionStringName;
        }

        public ContainerBuilder ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            if (_repositoryConfig == RepositoryConfig.EF)
            {
                RegisterEFRepositories(builder);
            }

            if (_repositoryConfig == RepositoryConfig.Dapper)
            {
                RegisterDapperRepositories(builder);
            }
            
            builder.RegisterType<GameLogicService>().As<IGameLogicService>();
            builder.RegisterType<PlayerService>().As<IPlayerService>();

            builder.RegisterType<GameUtility>().As<IGameUtility>();
            builder.RegisterType<DeckProvider>().As<IDeckProvider>();

            builder.RegisterType<ExceptionFileLogger>().As<IExceptionFileLogger>();
            
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PlayerControllerProfile());
                cfg.AddProfile(new GameControllerProfile());
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>()
                .CreateMapper(c.Resolve))
                .As<IMapper>();

            return builder;
        }

        private void RegisterEFRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<GameContext>().WithParameter("connectionString", _connectionStringName).InstancePerRequest();

            builder.RegisterType<GameRepository>().As<IGameRepository>();
            builder.RegisterType<PlayerRepository>().As<IPlayerRepository>();
            builder.RegisterType<HandRepository>().As<IHandRepository>();
            builder.RegisterType<RoundRepository>().As<IRoundRepository>();
            builder.RegisterType<PlayerGameRepository>().As<IPlayerGameRepository>();
            builder.RegisterType<CardRepository>().As<ICardRepository>();
            builder.RegisterType<HandCardRepository>().As<IHandCardRepository>();
        }

        private void RegisterDapperRepositories(ContainerBuilder builder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;

            builder.RegisterType<GameRepositoryDapper>().As<IGameRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<PlayerRepositoryDapper>().As<IPlayerRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<HandRepositoryDapper>().As<IHandRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<RoundRepositoryDapper>().As<IRoundRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<PlayerGameRepositoryDapper>().As<IPlayerGameRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<CardRepositoryDapper>().As<ICardRepository>().WithParameter("connectionString", connectionString);
            builder.RegisterType<HandCardRepositoryDapper>().As<IHandCardRepository>().WithParameter("connectionString", connectionString);
        }
    }
}