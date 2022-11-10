using System.Globalization;

namespace WebApiAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment environment;
        private readonly string nombreArchivo = "archivo-01.txt";
        private Timer timer;

        public EscribirEnArchivo(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        private void EscribirAArchivo(string mensaje)
        {
            var ruta = $@"{environment.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta,append: true))
            {
                writer.WriteLine(mensaje);
            }
        }

        private void DoWork(object state)
        {
            EscribirAArchivo($"::Proceso en ejecución: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}::");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork,null,TimeSpan.Zero,TimeSpan.FromSeconds(5));
            EscribirAArchivo("::Proceso Iniciado::");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            EscribirAArchivo("::Proceso Finalizado::");
            return Task.CompletedTask;
        }

    }
}
