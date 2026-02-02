using Doamin.Domain.ValueObjects;
using Domain.ValueObjects;
using SharedKernal;
using static Domain.ErrorsMenu.ErrorsMenu.Contacts;
using static Domain.ErrorsMenu.ErrorsMenu.Employee;


namespace Domain.Entites
{
    public enum enGender { Male = 1, Female = 2 };


    public enum enEmployeeStatus
    {
        Active = 1,
        Resigned = 2,
        Terminated = 3,
        Retired = 4,
        OnLeave = 5,
        Suspended = 6
    }


    public class Employee
    {
        public int EmployeeId { get; private set; }
        public string EmployeeFirstName { get; private set; } = null!;
        public string EmployeeSecondName { get; private set; } = null!;
        public string EmployeeThirdName { get; private set; } = null!;
        public string EmployeeLastName { get; private set; } = null!;
        public int EmployeeNationalityId { get; private set; }
        public enEmployeeStatus Status { get; private set; } = enEmployeeStatus.Active;
        public BirthDate EmployeeBirthDate { get; private set; }
        public DateTime HireDate { get; private set; } = DateTime.Now.Date;
        public DateTime? EmployeeResignationDate { get; private set; } = null;
        public Nationality Nationality { get; private set; } = null!;
        public string EmployeeCode { get; private set; } = null!;
        public NationalNumber EmployeeNationalNumber { get; private set; } = null!;
        public string EmployeeCurrentAddress { get; private set; } = null!;
        public enGender EmployeeGender { get; private set; } = enGender.Male;
        public bool IsForeign { get; private set; }


       
        public IReadOnlyCollection<EmployeePhone> Phones { get { return (IReadOnlyCollection<EmployeePhone>)_phones; } }
        public IReadOnlyCollection<EmployeeEmail> Emails { get { return (IReadOnlyCollection<EmployeeEmail>)_emails; } }
        public IReadOnlyCollection<EmployeeWorkInfo> WorkInfoHistory { get { return (IReadOnlyCollection<EmployeeWorkInfo>) _workInfoHistory; } }


        private ICollection<EmployeeWorkInfo> _workInfoHistory = new List<EmployeeWorkInfo>();
        private ICollection<EmployeePhone> _phones = new List<EmployeePhone>();
        private ICollection<EmployeeEmail> _emails = new List<EmployeeEmail>();
        

        
       


        public EmployeeWorkInfo CurrentWorkInfo
        {
            get { return WorkInfoHistory.FirstOrDefault(w => w.IsCurrent)!; }   
        }


        




        private Employee() { }







        private Result<Employee> CanTransitionTo(enEmployeeStatus nextStatus)
        {
            return (this.Status, nextStatus) switch
            {
                (enEmployeeStatus.Active, enEmployeeStatus.Suspended) or
                (enEmployeeStatus.Active, enEmployeeStatus.OnLeave) or
                (enEmployeeStatus.Active, enEmployeeStatus.Resigned) or
                (enEmployeeStatus.Active, enEmployeeStatus.Terminated) or
                (enEmployeeStatus.Active, enEmployeeStatus.Retired) or
                (enEmployeeStatus.Suspended, enEmployeeStatus.Active) or
                (enEmployeeStatus.Suspended, enEmployeeStatus.Terminated) or
                (enEmployeeStatus.OnLeave, enEmployeeStatus.Active) or
                (enEmployeeStatus.OnLeave, enEmployeeStatus.Resigned) => Result<Employee>.Successful(this),

                
                (enEmployeeStatus.Terminated, _) => Result<Employee>.Failure(Terminated),
                (enEmployeeStatus.Resigned, _) => Result<Employee>.Failure(ErrorsMenu.ErrorsMenu.Employee.Resigned),
                (enEmployeeStatus.Retired, _) => Result<Employee>.Failure(Retired),
                (enEmployeeStatus.Suspended, _) => Result<Employee>.Failure(ErrorsMenu.ErrorsMenu.Employee.Suspended),
                (enEmployeeStatus.OnLeave, _) => Result<Employee>.Failure(OnLeave),

                _ => Result<Employee>.Failure(NotActive)
            };
        }




        


        






        private Result<Employee> AddContact<TValue, TContact>(ICollection<TContact> collection, TContact contact) where TContact : IContact<TValue>
        {
            if (this.Status != enEmployeeStatus.Active)
                return Result<Employee>.Failure(NotActive);

          
            if (contact.IsPrimary)
            {
                var currentPrimary = collection.FirstOrDefault(c => c.IsPrimary);
                currentPrimary?.SetAsSecondary();
            }

            collection.Add(contact);
            return Result<Employee>.Successful(this);
        }





        private Result<Employee> ExecuteContactAction<TValue>(IContact<TValue> contact, Func<IContact<TValue>, Result<IContact<TValue>>> action)
        {
            if (this.Status != enEmployeeStatus.Active)
                return Result<Employee>.Failure(NotActive);


            var result = action(contact);

            if (!result.IsSuccess)
                return Result<Employee>.Failure(result.Error!);

            return Result<Employee>.Successful(this);
        }

        private Result<Employee> ExecuteOnFound<TValue>(
           IEnumerable<IContact<TValue>> list, int id, Error err, Func<IContact<TValue>, Result<IContact<TValue>>> action)
        {
            var contact = list.FirstOrDefault(c => c.Id == id);
            return contact == null ? Result<Employee>.Failure(err) : ExecuteContactAction(contact, action);
        }



        private Result<Employee> SetPrimaryContact<TValue>(IEnumerable<IContact<TValue>> contactsList, int contactId, Error notFoundError)
        {
           
            if (this.Status != enEmployeeStatus.Active)
                return Result<Employee>.Failure(NotActive);

            
            var newPrimary = contactsList.FirstOrDefault(c => c.Id == contactId);

            if (newPrimary == null)
                return Result<Employee>.Failure(notFoundError);

            
            var currentPrimary = contactsList.FirstOrDefault(c => c.IsPrimary);

           
            if (currentPrimary != null && !currentPrimary.Equals(newPrimary))
            {
                var demoteResult = currentPrimary.SetAsSecondary();
                if (!demoteResult.IsSuccess) return Result<Employee>.Failure(demoteResult.Error!);
            }

            var promoteResult = newPrimary.SetAsPrimary();
            if (!promoteResult.IsSuccess) return Result<Employee>.Failure(promoteResult.Error!);

            return Result<Employee>.Successful(this);
        }






        public static Employee Create(
        string firstName,
        string secondName,
        string thirdName,
        string lastName,
        int nationalityId,
        NationalNumber nationalNumber,
        string Code,
        BirthDate birthDate,
        EmployeeWorkInfo initialWorkInfo,
        EmployeePhone primaryPhone, 
        EmployeeEmail PrimaryEmail,
        string currentAddress
       )
        {

            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(secondName)) throw new ArgumentException("Second name is required.", nameof(secondName));
            if (string.IsNullOrWhiteSpace(thirdName)) throw new ArgumentException("Third name is required.", nameof(thirdName));
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Employee code is required.", nameof(Code));
            if (string.IsNullOrWhiteSpace(currentAddress)) throw new ArgumentException("Address is required.", nameof(currentAddress));

            if (nationalityId <= 0) throw new ArgumentException("nationality Id is required.",nameof(nationalityId));
            if (nationalNumber == null) throw new ArgumentNullException(nameof(nationalNumber));
            if (birthDate == null) throw new ArgumentNullException(nameof(birthDate));
            if (initialWorkInfo == null) throw new ArgumentNullException(nameof(initialWorkInfo));
            if (primaryPhone == null) throw new ArgumentNullException(nameof(primaryPhone));
            if (PrimaryEmail == null) throw new ArgumentNullException(nameof(PrimaryEmail));


            if (!PrimaryEmail.IsPrimary || !PrimaryEmail.IsActive)
                throw new InvalidOperationException("Initial Primary Email must be both Active and marked as Primary.");


            if (!primaryPhone.IsPrimary || !primaryPhone.IsActive)
                throw new InvalidOperationException("Initial Primary Phone must be both Active and marked as Primary.");



            var employee = new Employee
            {
                EmployeeFirstName = firstName.Trim(),
                EmployeeSecondName = secondName.Trim(),
                EmployeeThirdName = thirdName.Trim(),
                EmployeeLastName = lastName.Trim(),
                EmployeeNationalityId = nationalityId,
                EmployeeNationalNumber = nationalNumber,
                EmployeeBirthDate = birthDate,
                EmployeeGender = enGender.Male,
                EmployeeCurrentAddress = currentAddress,
                Status = enEmployeeStatus.Active,
                EmployeeCode = Code,
            };

            employee._workInfoHistory.Add(initialWorkInfo);
            employee._emails.Add(PrimaryEmail);
            employee._phones.Add(primaryPhone);

            return employee;
        }


        public Employee ChangeFirstName(string first)
        {
            if (string.IsNullOrWhiteSpace(first))
                throw new ArgumentException("First name cannot be empty.", nameof(first));

            EmployeeFirstName = first.Trim();
            return this;
        }

      
        public Employee ChangeLastName(string last)
        {
            if (string.IsNullOrWhiteSpace(last))
                throw new ArgumentException("last name cannot be empty.", nameof(last));

            EmployeeLastName = last.Trim();
            return this;
        }

       
        public Employee ChangeSecondName(string second)
        {
            if (string.IsNullOrWhiteSpace(second))
                throw new ArgumentException("second name cannot be empty.", nameof(second));

            EmployeeSecondName = second.Trim();
            return this;
        }

        public Employee ChangeThirdName(string third)
        {
            if (string.IsNullOrWhiteSpace(third))
                throw new ArgumentException("third name cannot be empty.", nameof(third));

            EmployeeThirdName = third.Trim();
            return this;
        }


        public Employee ChangeNationalNumber(NationalNumber NationalNumber)
        {
            if (NationalNumber == null)
                throw new ArgumentNullException(nameof(NationalNumber));

            this.EmployeeNationalNumber = NationalNumber;
            return this;
        }

        public Employee ChangeNationality(Nationality nationality)
        {
            if (nationality == null)
                throw new ArgumentNullException(nameof(nationality));

            this.Nationality = nationality;
            this.IsForeign = !nationality.IsLocal;
            return this;
        }


        public Employee ChangeAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("address cannot be empty", nameof(address));

            this.EmployeeCurrentAddress = address;
            return this;
        }

       

        internal Result<Employee> AddPhone(EmployeePhone phone) => AddContact<PhoneNumber, EmployeePhone>(_phones, phone);
        internal Result<Employee> AddEmail(EmployeeEmail email) => AddContact<EmailAddress, EmployeeEmail>(_emails, email);

       
        public Result<Employee> UpdatePhone(PhoneNumber phone, int id)
        {
            var contact = Phones.FirstOrDefault(c => c.Id == id);
            if(contact == null) return Result<Employee>.Failure(PhoneNotFound);

            contact.Change(phone);
            return Result<Employee>.Successful(this);
        }
           

        public Result<Employee> UpdateEmail(EmailAddress email, int id)
        {
            var contact = _emails.FirstOrDefault(c => c.Id == id);
            if(contact == null) return Result<Employee>.Failure(EmailNotFound);

            contact.Change(email);
            return Result<Employee>.Successful(this);
        } 
      

        public Result<Employee> FreezePhone(int id) =>
            ExecuteOnFound(_phones, id, CannotFreezePrimaryPhone, c => c.Freeze());

        public Result<Employee> FreezeEmail(int id) =>
            ExecuteOnFound(_emails, id, CannotFreezePrimaryEmail, c => c.Freeze());

      
        public Result<Employee> SetPrimaryPhone(int id) => SetPrimaryContact(_phones, id, PhoneNotFound);
        public Result<Employee> SetPrimaryEmail(int id) => SetPrimaryContact(_emails, id, EmailNotFound);
       



      
        public Employee ChangeBirthDate(BirthDate birthDate)
        {
            if (birthDate == null)
                throw new ArgumentNullException(nameof(birthDate));


            this.EmployeeBirthDate = birthDate;
            return this;
        }


        public void ChangeMale() => this.EmployeeGender = enGender.Male;
        public void ChangeFemale() => this.EmployeeGender = enGender.Female;












        public Result<Employee> Activate()
        {
            var CheckResult = CanTransitionTo(enEmployeeStatus.Active);

            if (!CheckResult.IsSuccess)
                return CheckResult;

            Status = enEmployeeStatus.Active;
            return Result<Employee>.Successful(this);
        }

        public Result<Employee> Resigned()
        {
            var CheckResult = CanTransitionTo(enEmployeeStatus.Resigned);

            if (!CheckResult.IsSuccess)
                return CheckResult;

            this.Status = enEmployeeStatus.Resigned;
            return Result<Employee>.Successful(this);
        }

        public Result<Employee> Terminate()
        {

            var CheckResult = CanTransitionTo(enEmployeeStatus.Terminated);

            if (!CheckResult.IsSuccess)
                return CheckResult;

            Status = enEmployeeStatus.Terminated;
            EmployeeResignationDate = DateTime.Now.Date;

            return Result<Employee>.Successful(this);
        }


        public Result<Employee> Retire()
        {

            var CheckResult = CanTransitionTo(enEmployeeStatus.Retired);

            if (!CheckResult.IsSuccess)
                return CheckResult;


            Status = enEmployeeStatus.Retired;
            EmployeeResignationDate = DateTime.Today;

            return Result<Employee>.Successful(this);
        }


        public Result<Employee> PutOnLeave()
        {
            var CheckResult = CanTransitionTo(enEmployeeStatus.OnLeave);

            if (!CheckResult.IsSuccess)
                return CheckResult;


            Status = enEmployeeStatus.OnLeave;
            return Result<Employee>.Successful(this);
        }

        public Result<Employee> Suspended()
        {
            var CheckResult = CanTransitionTo(enEmployeeStatus.Suspended);

            if (!CheckResult.IsSuccess)
                return CheckResult;

            this.Status = enEmployeeStatus.Suspended;
            return Result<Employee>.Successful(this);
        }




        internal Result<Employee> ChangeWorkInformation(EmployeeWorkInfo workInfo)
        {
            if (workInfo == null)
                throw new ArgumentNullException(nameof(workInfo));

            var current = WorkInfoHistory.FirstOrDefault(w => w.IsCurrent);

            if (current != null)
            {
                var ResultFrozenCurrentWorkInfoRecord = current.Freeze();

                if(!ResultFrozenCurrentWorkInfoRecord.IsSuccess)
                    return Result<Employee>.Failure(ResultFrozenCurrentWorkInfoRecord.Error!);
            }


            _workInfoHistory.Add(workInfo);
            return Result<Employee>.Successful(this);
        }





        public override bool Equals(object? obj)
        {
            if (obj is not Employee other)
                return false;

            
            if (ReferenceEquals(this, other))
                return true;

            if (this.EmployeeId == 0 && other.EmployeeId == 0)
                return this.EmployeeNationalNumber == other.EmployeeNationalNumber;

            return this.EmployeeId == other.EmployeeId;
        }

        public override int GetHashCode()
        {
            if(this.EmployeeId == 0)
                return HashCode.Combine(EmployeeNationalNumber);

            return HashCode.Combine(EmployeeId);
        }
    }
}