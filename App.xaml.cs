using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.IO;

// New mechanism to render stuff: HTML
// Use an embedded HTML server to serve up a page that responds to SignalR.
// https://levelup.gitconnected.com/how-to-embed-a-web-server-inside-desktop-applications-643ce3cb51bf
// We're already doing this. Shouldn't be much to make it work.
//
// That way the web page could be customized too.

namespace PngTuber.Pupper
{
    public class WebServerEngine
    {
        private IHost aspNetHost;

        public static WebServerEngine Instance { get; set; }

        public string AssetRootPath { get; set; }

        private void Start()
        {
            if(this.aspNetHost is not null)
            {
                return;
            }

            var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(w =>
                {
                    w.UseUrls("http://localhost:65000")
                        .ConfigureServices(s => 
                        {
                            s.AddCors();
                            s.AddSignalR();
                        })
                        .Configure(a =>
                        {
                            a.UseCors(p =>
                            {
                                p.AllowAnyMethod();
                                p.AllowAnyHeader();
                                p.AllowCredentials();
                                p.WithOrigins("http://localhost:8000");
                            });

                            a.UseRouting();
                            a.UseEndpoints(e =>
                            {
                                e.MapGet("/viewer", async context =>
                                {
                                    context.Response.StatusCode = 200;
                                    context.Response.Headers.Add("Cache-Control", "no-cache");


                                    var file = await File.ReadAllBytesAsync("viewer.html");

                                    await context.Response.BodyWriter.WriteAsync(file);

                                });

                                 e.MapGet("/asset/{emotion}/{asset}", async context =>
                                {
                                    if (string.IsNullOrEmpty(this.AssetRootPath))
                                    {
                                        context.Response.StatusCode = 400;
                                        await context.Response.WriteAsync("An avatar has not been selected.");
                                        return;
                                    }

                                    context.Response.StatusCode = 200;
                                    context.Response.Headers.Add("Cache-Control", "public, max-age=604800, immutable");

                                    var assetFileName = context.Request.RouteValues["asset"] as string;
                                    var emotion = context.Request.RouteValues["emotion"] as string;

                                    var file = await File.ReadAllBytesAsync(
                                        Path.Combine(
                                            this.AssetRootPath, 
                                            emotion,
                                            assetFileName));

                                    await context.Response.BodyWriter.WriteAsync(file);
                                });

                                e.MapHub<PuppeteerHub>("/puppethub");
                            });
                        });
                }).Build();

            host.Start();

            this.aspNetHost = host;
        }

        public static void CreateInstance()
        {
            if(Instance is not null)
            {
                return;
            }

            var instance = new WebServerEngine();
            instance.Start();
            Instance = instance;
        }

        public static void Stop()
        {
            if(Instance is null)
            {
                return;
            }

            Instance.aspNetHost.Dispose();
            Instance = null;
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WebServerEngine.CreateInstance();
            Application.Current.MainWindow = new MainWindow();

            Application.Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            WebServerEngine.Stop();
        }
    }

    public class PuppeteerHub : Hub
    {
        public async Task AnimationUpdate(AnimationMessage message)
        {
            await this.Clients.All.SendAsync("AnimationUpdate", message);
        }
    }

}
