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
            var logger = CreateLogger();
            var context = CreateContext(appConfig, logger);

            builder.Register(c => logger).As<ILogger>().SingleInstance();
            builder.Register(c => appConfig).As<IAppConfig>().SingleInstance();
            builder.Register(c => context).As<OpenGlContext>().SingleInstance();
            builder.RegisterType<Game>().As<IGame>().OnRelease(game => game.Dispose()).InstancePerLifetimeScope();
            builder.Register(c => new Camera(new Vector3(-1, 120, -1), 135, 0)).As<ICamera>().SingleInstance();
            builder.RegisterType<CameraController>().SingleInstance();
            builder.RegisterType<GroundGenerator>().As<IGroundGenerator>().SingleInstance();
            builder.RegisterType<BuildingGenerator>().As<IBuildingGenerator>().SingleInstance();
            builder.RegisterType<World>().As<IWorld>().SingleInstance();
            builder.RegisterType<Renderer.Renderer>().As<IRenderer>().InstancePerDependency();
            builder.RegisterType<ProceduralSkybox>().As<ISkybox>().SingleInstance();
            builder.RegisterType<BillboardTextureGenerator>().As<IBillboardTextureGenerator>().SingleInstance();
            builder.RegisterType<BillboardBuilder>().As<IBillboardBuilder>().SingleInstance();

            return builder.Build();
        }

        private static Serilog.Core.Logger CreateLogger()
        {
            var logger = new LoggerConfiguration()
                            .WriteTo.Async(writeTo => writeTo.Console())
                            .MinimumLevel.Verbose()
                            .Enrich.FromLogContext()
                            .CreateLogger();
            Log.Logger = logger;
            return logger;
        }

        private static OpenGlContext CreateContext(AppConfig appConfig, Serilog.Core.Logger logger)
        {
            var gameWindowSettings = new GameWindowSettings
            {
                UpdateFrequency = appConfig.FrameRate
            };

            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(appConfig.ResolutionWidth, appConfig.ResolutionHeight),
                Title = appConfig.WindowTitle,
            };

            var context = new OpenGlContext(gameWindowSettings, nativeWindowSettings, logger);
            return context;
        }
    }
}

