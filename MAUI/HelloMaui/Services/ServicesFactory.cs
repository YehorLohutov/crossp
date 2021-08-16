using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI.Services
{
    public static class ServicesFactory
    {
        public static IAuthorizationService GetAuthorizationService() => new DataAccessService();
    }
}
