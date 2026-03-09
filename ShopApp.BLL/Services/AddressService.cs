using ShopApp.BLL.DTOs;
using ShopApp.BLL.Mappers;
using ShopApp.BLL.Services.Interfaces;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;

        public AddressService(IAddressRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AddressDto>> GetByUserAsync(string userId)
        {
            var addresses = await _repo.GetByUserAsync(userId);
            return addresses.Select(a => a.ToDto());
        }

        public async Task<AddressDto?> GetByIdAsync(int id)
        {
            var address = await _repo.GetByIdAsync(id);
            return address?.ToDto();
        }

        public async Task<AddressDto> CreateAsync(string userId, AddressCreateDto dto)
        {
            var address = new Address
            {
                UserId    = userId,
                Country   = dto.Country,
                City      = dto.City,
                Street    = dto.Street,
                Zip       = dto.Zip,
                IsDefault = dto.IsDefault
            };
            await _repo.AddAsync(address);
            return address.ToDto();
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
