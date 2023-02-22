namespace AngularLearningProjectBackEnd.Models
{
    public class MyConfig
    {
        internal string ConnectionString { get; }

        public MyConfig(IConfiguration configuration)
        {
            ConnectionString = configuration.GetValue<string>("ConnectionStrings:Default");
        }
    }
}
