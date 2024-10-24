namespace UdemyRealWorldUnitTest.WEB.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll(); //liste olarak dön

        Task<TEntity> GetById(int id); //id değerine göre getir 

        Task Create(TEntity entity); //parametreden gelen değere göre yeni ürün oluştur 
        void Update(TEntity entity); //entity parametresinden gelen değere göre ürünü güncelle

        void Delete(TEntity entity); // geriye değer döndürmeyecek şekilde veriyi sil
    }
}
//Update ve delete metotlarının async metotları yoktur 