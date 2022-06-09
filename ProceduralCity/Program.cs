using Autofac;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

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
            using var scope = container.BeginLifetimeScope();
            using var game = scope.Resolve<IGame>();
            game.RunGame();
        }

        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            var appConfig = new AppConfig();

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .CreateLogger();
            Log.Logger = logger;

            var gs = new GameWindowSettings
            {
                RenderFrequency = appConfig.FrameRate
            };

            var ns = new NativeWindowSettings
            {
                Size = new Vector2i(640, 480),
                Title = "DummyContext"
            };

            var context = new OpenGlContext(gs, ns, logger); //this is a hack to create a context before any opengl calls

            builder.Register(c => context).As<OpenGlContext>().SingleInstance();
            builder.Register(c => logger).As<ILogger>().SingleInstance();
            builder.RegisterType<Game>().As<IGame>().OnRelease(game => game.Dispose()).InstancePerLifetimeScope();
            builder.Register(c => appConfig).As<IAppConfig>().SingleInstance();
            builder.Register(c => new Camera(new Vector3(-1, 10, -1), 90, 0)).As<ICamera>().SingleInstance();
            builder.RegisterType<GroundGenerator>().As<IGroundGenerator>().SingleInstance();
            builder.RegisterType<BuildingGenerator>().As<IBuildingGenerator>().SingleInstance();
            builder.RegisterType<World>().As<IWorld>().SingleInstance();
            builder.RegisterType<Renderer.Renderer>().As<IRenderer>().InstancePerDependency();
            builder.RegisterType<ProceduralSkybox>().As<ISkybox>().SingleInstance();
            builder.RegisterType<BillboardTextureGenerator>().As<IBillboardTextureGenerator>().SingleInstance();
            builder.RegisterType<BillboardBuilder>().As<IBillboardBuilder>().SingleInstance();

            return builder.Build();
        }
    }
}

