﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PingNet.Dialogs;
using PingNet.Services;
using PingNet.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PingNet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILogger _logger;
        private readonly IHost _host;

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            _logger.Information("Configuring host");

            _host = Host.CreateDefaultBuilder()
               .ConfigureServices((context, services) =>
               {
                   ConfigureServices(context.Configuration, services);
               })
               .UseSerilog(_logger) //TODO: NOT LOGGING VIEWMODEL INFORMATION?!?!?!
               .Build();

            ServiceProvider = _host.Services;
        }

        /// <summary>
        /// Adds services to the IoC container
        /// </summary>
        /// <param name="services">Service collection</param>
        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            //Add the settings read from the appsettings.json
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            //Register services
            //services.AddSingleton(_logger);
            services.AddSingleton<INetworkAnalyser, NetworkAnalyser>();

            //Register viewmodels
            services.AddTransient<MainWindowViewModel>();

            //Register the Main window
            services.AddTransient(typeof(MainWindow));
        }

        /// <summary>
        /// Triggered on application startup 
        /// </summary>
        /// <param name="e">Startup event arguments</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        /// <summary>
        /// Triggered on application exit
        /// </summary>
        /// <param name="e">Exit event arguments</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }

        /// <summary>
        /// Global exception handler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DialogBox dialogBox = new DialogBox("Error", e.Exception.Message, true);

            if (dialogBox.ShowDialog() == false)
                e.Handled = true;
        }
    }
}
