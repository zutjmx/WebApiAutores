namespace WebApiAutores.DTOs
{
    public class LimitarPeticionesConfiguracion
    {
        public int PeticionesDiariasGratuitas { get; set; }
        public string[] ListaBlancaRutas { get; set; }
    }
}
