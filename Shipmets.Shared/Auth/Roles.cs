using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Auth
{
    public static class Roles
    {
        public const string Client = "Client";
        public const string Courier = "Courier";
        public const string Admin = "Admin";

        public static readonly string[] All = { Client, Courier, Admin };
    }
}
