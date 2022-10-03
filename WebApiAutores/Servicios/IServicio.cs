namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        Guid GetScoped();
        Guid GetSingleton();
        Guid GetTransient();
        void RealizarTarea();
    }

    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;

        public ServicioA(ILogger<ServicioA> logger,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton
        )
        {
            this.logger = logger;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
        }

        public Guid GetTransient() { return servicioTransient.Guid; }
        public Guid GetScoped() { return servicioScoped.Guid; }
        public Guid GetSingleton() { return servicioSingleton.Guid; }

        public void RealizarTarea()
        {
            this.logger.Log(LogLevel.Information, "Método RealizarTarea de ServicioA");
        }
    }

    public class ServicioB : IServicio
    {
        public Guid GetScoped()
        {
            throw new NotImplementedException();
        }

        public Guid GetSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid GetTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            System.Console.WriteLine("Método RealizarTarea de ServicioB");
        }
    }

    public class ServicioTransient
    {
        public Guid Guid = Guid.NewGuid();
    }
    
    public class ServicioScoped
    {
        public Guid Guid = Guid.NewGuid();
    }
    
    public class ServicioSingleton
    {
        public Guid Guid = Guid.NewGuid();
    }

}
