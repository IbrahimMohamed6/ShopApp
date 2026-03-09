using Microsoft.EntityFrameworkCore;
using ShopApp.DAL.Data;
using ShopApp.DAL.Models;
using ShopApp.DAL.Repositories.Interfaces;

namespace ShopApp.DAL.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext db) : base(db) { }

        public async Task<IEnumerable<Address>> GetByUserAsync(string userId) =>
            await _db.Addresses
                     .Where(a => a.UserId == userId)
                     .OrderByDescending(a => a.IsDefault)
                     .ToListAsync();

        // When adding/updating, ensure only one default address per user
        public override async Task AddAsync(Address address)
        {
            if (address.IsDefault)
                await ClearDefaultsAsync(address.UserId, excludeId: null);

            await base.AddAsync(address);
        }

        public override async Task UpdateAsync(Address address)
        {
            if (address.IsDefault)
                await ClearDefaultsAsync(address.UserId, excludeId: address.AddressId);

            await base.UpdateAsync(address);
        }

        private async Task ClearDefaultsAsync(string userId, int? excludeId)
        {
            var defaults = await _db.Addresses
                .Where(a => a.UserId == userId && a.IsDefault &&
                            (excludeId == null || a.AddressId != excludeId))
                .ToListAsync();

            defaults.ForEach(a => a.IsDefault = false);
            await _db.SaveChangesAsync();
        }
    }
}
