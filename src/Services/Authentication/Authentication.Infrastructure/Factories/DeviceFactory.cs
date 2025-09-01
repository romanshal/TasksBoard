using Authentication.Domain.Interfaces.Factories;

namespace Authentication.Infrastructure.Factories
{
    internal class DeviceFactory : IDeviceFactory
    {
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
