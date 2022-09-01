using WebApiAutores.Entidades;

namespace WebApiAutores.Common
{
    public class Util
    {
        private readonly ApplicationDbContext context;

        public Util(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Util()
        {

        }
        public List<Autor> GetAutors(int Cuantos) {
            List<Autor> autors = new List<Autor>();
            for (int j = 0; j <= Cuantos; j++)
            {
                Autor autor = new Autor();
                autor.Id = Faker.RandomNumber.Next(100);
                autor.Nombre = Faker.Name.FullName();
                autors.Add(autor);
            }
            return autors; 
        }

        public Autor GetAutor(int id) {
            Autor autor = new Autor();
            autor.Id = id;
            autor.Nombre = Faker.Name.FullName();
            return autor;
        }

        public bool SetSeed(int Cuantos) {
            for (int j = 0; j <= Cuantos; j++)
            {
                Autor autor = new Autor();
                autor.Nombre = Faker.Name.FullName();
                this.context.Add(autor);
                this.context.SaveChanges();
            }
            return true;
        }

        public bool BorrarTodos()
        {
            //TODO: Borrar todos los registros de la tabla. 
            return true;
        }
    }
}
