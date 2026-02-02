using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;

namespace Domain.Entites
{
    public class Nationality
    {
        public int NationalityId { get; private set; }
        public string NationalityName { get; private set; } = null!;
        public bool IsLocal => NationalityId == 1;


        private Nationality() { }



        
        public static Result<Nationality> create(string NationalityName)
        {
            if (string.IsNullOrWhiteSpace(NationalityName))
                return Result<Nationality>.Failure(NationalityNameEmpty);

            
            var Nationality = new Nationality()
            {
               NationalityName = NationalityName
            };

            return Result<Nationality>.Successful(Nationality);
        }
    }
}