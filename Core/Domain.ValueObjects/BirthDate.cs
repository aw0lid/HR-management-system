using Domain.ErrorsMenu;
using SharedKernal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doamin.Domain.ValueObjects
{
    [ComplexType]
    public record BirthDate
    {
        public DateTime Value { get; }
        public short Age { get { return (short)AgeCalculatur(Value); } }


        private BirthDate(DateTime value) => Value = value;

        public static bool ValidValue(DateTime birthDate)
        {
           return birthDate < DateTime.Today &&  AgeCalculatur(birthDate) < 18 && AgeCalculatur(birthDate) > 65;  
        }

        private static int AgeCalculatur(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }


        public static Result<BirthDate> Create(DateTime date)
        {
            if (date >= DateTime.Today)
                throw new ArgumentException("Birth date cannot be in the future or today.");

            var age = AgeCalculatur(date);

            if (age < 18) return Result<BirthDate>.Failure(ErrorsMenu.Employee.UnderAge);
            if (age > 65) return Result<BirthDate>.Failure(ErrorsMenu.Employee.OverAge);

            return Result<BirthDate>.Successful(new BirthDate(date));
        }
    }
}
