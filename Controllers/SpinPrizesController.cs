using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.SpinPrize;
using TravelTechApi.Services.SpinPrize;

namespace TravelTechApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpinPrizesController : ControllerBase
    {
        private readonly ISpinPrizeService _spinPrizeService;

        public SpinPrizesController(ISpinPrizeService spinPrizeService)
        {
            _spinPrizeService = spinPrizeService;
        }

        // GET: api/SpinPrizes (Public, active only)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublic()
        {
            var prizes = await _spinPrizeService.GetAllPublicAsync();
            return this.Success(prizes, "Prizes retrieved successfully.");
        }

        // GET: api/SpinPrizes/admin (Admin only, all)
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var prizes = await _spinPrizeService.GetAllAdminAsync();
            return this.Success(prizes, "All prizes retrieved successfully.");
        }

        // GET: api/SpinPrizes/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var prize = await _spinPrizeService.GetByIdAsync(id);
            if (prize == null)
            {
                return this.Failed("Spin prize not found.");
            }

            // Optional: If public user calls this on inactive prize, could return 404. 
            // We'll return it as long as they know the ID, or you could restrict here.

            return this.Success(prize, "Spin prize retrieved successfully.");
        }

        // POST: api/SpinPrizes
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSpinPrizeDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.Failed("Invalid input data.");
            }

            var result = await _spinPrizeService.CreateAsync(createDto);
            if (!result)
            {
                return this.Failed("Failed to create spin prize.");
            }

            return this.Success("Spin prize created successfully.");
        }

        // PUT: api/SpinPrizes/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpinPrizeDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return this.Failed("Invalid input data.");
            }

            var result = await _spinPrizeService.UpdateAsync(id, updateDto);
            if (!result)
            {
                return this.Failed("Spin prize not found or failed to update.");
            }

            return this.Success("Spin prize updated successfully.");
        }

        // DELETE: api/SpinPrizes/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _spinPrizeService.DeleteAsync(id);
            if (!result)
            {
                return this.Failed("Spin prize not found or failed to delete.");
            }

            return this.Success("Spin prize deleted successfully.");
        }
    }
}
