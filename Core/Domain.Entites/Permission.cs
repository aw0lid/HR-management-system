namespace Domain.Entites
{
    public enum enEmployeePermissions : long
    {
        Admin = -1,
        None = 0,
        ViewEmployees = 1,
        AddEmployees = 2,
        EditEmployees = 4,
        ViewOrganizations = 8
    }




    public class Permission
    {
        public int PermissionId { get; private set; }
        public string PermissionName { get; private set; } = null!;
        public long BitValue { get; private set; }


        private Permission() { } 


        public static Permission Create(int id, string name, enEmployeePermissions bitValue)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            
            return new Permission 
            { 
                PermissionId = id, 
                PermissionName = name, 
                BitValue = (long)bitValue 
            };
        }

        public static bool IsValidPermissionMask(long mask)
        {
            if(mask == (long)enEmployeePermissions.Admin || 
               mask == (long)enEmployeePermissions.None) return true;

            
            long allPossiblePermissions = 0;

            foreach(var Per in Enum.GetValues(typeof(enEmployeePermissions)))
            {
                allPossiblePermissions |= (long)Per;
            }

            return (mask & allPossiblePermissions) == mask;
        }
    }
}