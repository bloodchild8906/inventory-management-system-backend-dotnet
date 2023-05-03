using Inventory.Application.Domain.Identity;

namespace Inventory.Application.Shared.Permissions
{
    public class PermissionsFactory
    {
        private static Module Dashboard
        {
            get
            {
                return new Module
                {
                    Id = Modules.Dashboard,
                    Name = "Dashboard",
                    Order = 1,
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Id = Permissions.Dashboard.View,
                            ModuleId = Modules.Dashboard,
                            Name = "Dashboard",
                            Order = 1,
                            Required = false
                        }
                    }
                };
            }
        }

        private static Module Users
        {
            get
            {
                return new Module
                {
                    Id = Modules.Users,
                    Name = "Users",
                    Order = 2,
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Id = Permissions.Users.View,
                            ModuleId = Modules.Users,
                            Name = "View",
                            Order = 1,
                            Required = true
                        },
                        new Permission
                        {
                            Id = Permissions.Users.Create,
                            ModuleId = Modules.Users,
                            Name = "Create",
                            Order = 2,
                            Required = false

                        },
                        new Permission
                        {
                            Id = Permissions.Users.Edit,
                            ModuleId = Modules.Users,
                            Name = "Edit",
                            Order = 3,
                            Required = false
                        },
                        new Permission
                        {
                            Id = Permissions.Users.Delete,
                            ModuleId = Modules.Users,
                            Name = "Delete",
                            Order = 4,
                            Required = false
                        }
                    }
                };
            }
        }

        private static Module Roles
        {
            get
            {
                return new Module
                {
                    Id = Modules.Roles,
                    Name = "Roles",
                    Order = 3,
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Id = Permissions.Roles.View,
                            ModuleId = Modules.Roles,
                            Name = "View",
                            Order = 1,
                            Required = true
                        },
                        new Permission
                        {
                            Id = Permissions.Roles.Create,
                            ModuleId = Modules.Roles,
                            Name = "Create",
                            Order = 2,
                            Required = false
                        },
                        new Permission
                        {
                            Id = Permissions.Roles.Edit,
                            ModuleId = Modules.Roles,
                            Name = "Edit",
                            Order = 3,
                            Required = false
                        },
                        new Permission
                        {
                            Id = Permissions.Roles.Delete,
                            ModuleId = Modules.Roles,
                            Name = "Delete",
                            Order = 4,
                            Required = false
                        }
                    }
                };
            }
        }

        private static Module Products
        {
            get
            {
                return new Module
                {
                    Id = Modules.Products,
                    Name = "Products",
                    Order = 4,
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Id = Permissions.Products.View,
                            ModuleId = Modules.Products,
                            Name = "View",
                            Order = 1,
                            Required = true
                        },
                        new Permission
                        {
                            Id = Permissions.Products.Create,
                            ModuleId = Modules.Products,
                            Name = "Create",
                            Order = 2,
                            Required = false
                        },
                        new Permission
                        {
                            Id = Permissions.Products.Edit,
                            ModuleId = Modules.Products,
                            Name = "Edit",
                            Order = 3,
                            Required = false
                        },
                        new Permission
                        {
                            Id = Permissions.Products.Delete,
                            ModuleId = Modules.Products,
                            Name = "Delete",
                            Order = 4,
                            Required = false
                        }
                    }
                };
            }
        }

        public static IReadOnlyCollection<Module> CreateModulesWithPermissions()
        {
            return new List<Module>
            {
               Dashboard,
               Users,
               Roles,
               Products
            }.AsReadOnly();
        }
    }
}
