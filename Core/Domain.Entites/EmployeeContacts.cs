using Domain.ValueObjects;
using SharedKernal;
using System.Numerics;
using static Domain.ErrorsMenu.ErrorsMenu.Contacts;

namespace Domain.Entites
{

    public interface IContact<TValue>
    {
        int Id { get; }
        int EmployeeId { get; }
        TValue Value { get; }
        bool IsPrimary { get; }
        bool IsActive { get; }

        public IContact<TValue> Change(TValue value);
        Result<IContact<TValue>> SetAsPrimary();
        Result<IContact<TValue>> Freeze();
        Result<IContact<TValue>> SetAsSecondary();
    }




    public class EmployeePhone : IContact<PhoneNumber>
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public PhoneNumber Value { get; private set; } = null!;
        public bool IsPrimary { get; private set; }
        public bool IsActive { get; private set; }

        private EmployeePhone() { }


        private EmployeePhone(PhoneNumber phone, bool isPrimary, bool isActive)
        {
            Value = phone;
            IsPrimary = isPrimary;
            IsActive = isActive;
        }




        public static EmployeePhone CreatePrimary(PhoneNumber phone) =>
            phone == null ? throw new ArgumentNullException(nameof(phone)) : new EmployeePhone(phone, true, true);

        public static EmployeePhone Create(PhoneNumber phone) =>
            phone == null ? throw new ArgumentNullException(nameof(phone)) : new EmployeePhone(phone, false, true);






        public EmployeePhone Change(PhoneNumber newValue)
        {
            if (newValue == null) throw new ArgumentNullException(nameof(PhoneNumber));
            Value = newValue;
            return this;
        }

        public Result<IContact<PhoneNumber>> SetAsPrimary()
        {
            if (!IsActive) return Result<IContact<PhoneNumber>>.Failure(CannotSetInactivePhoneAsPrimary);
            IsPrimary = true;
            return Result<IContact<PhoneNumber>>.Successful(this);
        }

        public Result<IContact<PhoneNumber>> SetAsSecondary()
        {
            if (!IsPrimary) return Result<IContact<PhoneNumber>>.Failure(IsNotPrimaryPhone);
            IsPrimary = false;
            return Result<IContact<PhoneNumber>>.Successful(this);
        }

        public Result<IContact<PhoneNumber>> Freeze()
        {
            if (IsPrimary) return Result<IContact<PhoneNumber>>.Failure(CannotFreezePrimaryPhone);
            IsActive = false;
            return Result<IContact<PhoneNumber>>.Successful(this);
        }

        public override bool Equals(object? obj) => obj is EmployeePhone other && (Id != 0 && other.Id != 0 ? Id == other.Id : Value.Equals(other.Value));
        public override int GetHashCode() => Id != 0 ? HashCode.Combine(Id) : HashCode.Combine(Value);



        IContact<PhoneNumber> IContact<PhoneNumber>.Change(PhoneNumber value)
        {
            return Change(value);
        }
    }




    public class EmployeeEmail : IContact<EmailAddress>
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public EmailAddress Value { get; private set; } = null!;
        public bool IsPrimary { get; private set; }
        public bool IsActive { get; private set; }

        private EmployeeEmail() { }

        private EmployeeEmail(EmailAddress email, bool isPrimary, bool isActive)
        {
            Value = email;
            IsPrimary = isPrimary;
            IsActive = isActive;
        }

        
        public static EmployeeEmail CreatePrimary(EmailAddress email)
        {
            if (email == null) throw new ArgumentNullException(nameof(EmailAddress));
            return new EmployeeEmail(email, true, true);
        }

        public static EmployeeEmail Create(EmailAddress email)
        {
            if (email == null) throw new ArgumentNullException(nameof(EmailAddress));
            return new EmployeeEmail(email, false, true);
        }

        

        public EmployeeEmail Change(EmailAddress newValue)
        {
            if (newValue == null)
                 throw new ArgumentNullException(nameof(EmailAddress));

            Value = newValue;
            return this;
        }

        public Result<IContact<EmailAddress>> SetAsPrimary()
        {
            if (!IsActive)
                return Result<IContact<EmailAddress>>.Failure(CannotSetInactiveEmailAsPrimary);

            IsPrimary = true;
            return Result<IContact<EmailAddress>>.Successful(this);
        }

        public Result<IContact<EmailAddress>> SetAsSecondary()
        {
            if (!IsPrimary)
                return Result<IContact<EmailAddress>>.Failure(IsNotPrimaryEmail);

            IsPrimary = false;
            return Result<IContact<EmailAddress>>.Successful(this);
        }

        public Result<IContact<EmailAddress>> Freeze()
        {
            if (IsPrimary)
                return Result<IContact<EmailAddress>>.Failure(CannotFreezePrimaryEmail);

            IsActive = false;
            return Result<IContact<EmailAddress>>.Successful(this);
        }

       



        public override bool Equals(object? obj)
        {
            if (obj is not EmployeeEmail other) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Id != 0 && other.Id != 0)
                return Id == other.Id;

            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Id != 0 ? HashCode.Combine(Id) : HashCode.Combine(Value);
        }



        IContact<EmailAddress> IContact<EmailAddress>.Change(EmailAddress value)
        {
            return Change(value);
        }
    }
}