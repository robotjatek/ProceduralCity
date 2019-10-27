using Autofac;
using OpenTK;
using OpenTK.Graphics;
using ProceduralCity.Config;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using Serilog;

namespace ProceduralCity
{
    public static class Program
    {
        public static void Main()
        {
            var container = RegisterDependencies();
            using (var scope = container.BeginLifetimeScope())
            {
                var g = scope.Resolve<IGame>();
                g.RunGame();
            }
        }

        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .CreateLogger();
            Log.Logger = logger;

            var context = new OpenGlContext(640, 480, GraphicsMode.Default, "Dummy Context", logger); //this is a hack to create a context before any opengl calls

            builder.Register(c => context).As<OpenGlContext>().SingleInstance();
            builder.Register(c => logger).As<ILogger>().SingleInstance();
            builder.RegisterType<Game>().As<IGame>().OnRelease(game => game.Dispose()).InstancePerLifetimeScope();
            builder.RegisterType<AppConfig>().As<IAppConfig>().SingleInstance();
            builder.Register(c => new Camera(new Vector3(-1, -1, -1), 90, 0)).As<ICamera>().SingleInstance();
            builder.RegisterType<GroundGenerator>().As<IGroundGenerator>().SingleInstance();
            builder.RegisterType<BuildingGenerator>().As<IBuildingGenerator>().SingleInstance();
            builder.RegisterType<World>().As<IWorld>().SingleInstance();
            builder.RegisterType<Renderer.Renderer>().As<IRenderer>().InstancePerDependency();
            builder.RegisterType<Skybox>().As<ISkybox>().SingleInstance();

            return builder.Build();
        }
    }
}

