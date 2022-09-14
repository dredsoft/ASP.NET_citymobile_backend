using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services
{
    /// <summary>
    /// Marker interface to make it easier to register many services with the DI container. This interface
    /// is for services defined in MultiTenant.Services. Note that not all services in this project will
    /// implement this interface (e.g., services that require singleton scope).
    /// </summary>
    public interface ICustomService
    {
    }
}
