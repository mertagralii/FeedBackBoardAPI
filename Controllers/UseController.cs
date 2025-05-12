using FeedBackBoardAPI.Data.DTO.Register;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace FeedBackBoardAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin" )]
    public class UseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UseController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // DTO Kısımları ve Map kısmı yapılacak.

        #region Tüm Kullanıcıları Getir
        
        
        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Tüm Kullanıcıları Getir. ", Description = "Bu Endpointi  Tüm Kullanıcıları getirmeye yarar.")]
        [SwaggerResponse(200, "Kullanıcılar getirildi.")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Tüm Kullanıcıların verisini alamazsınız.")]
        public async Task<IActionResult> GetUsersList()
        {
            var userList = await _userManager.Users.ToListAsync();
            if (userList == null)
            {
                return NotFound("User not found");
            }
            return Ok(userList);
        }


        #endregion

        #region Email'e sahip Kullanıcıyı Getir
        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Email'e ait kullanıcıyı getir. ", Description = "Bu Endpointi Email'e ait kullanıcı Bilgilerini getirmeye yarar.")]
        [SwaggerResponse(200, "Kullanıcı Verisi Getirildi..")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Kullanıcı verisini alamazsınız.")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            return Ok(user);
        }
        #endregion

        #region Kullanıcıyı Sil
        [HttpDelete("[action]")]
        [SwaggerOperation(Summary = "Kullanıcı Silme ", Description = "Bu Endpointi Kullanıcıları Silmeye Yarar yarar.")]
        [SwaggerResponse(200, "Kullanıcı Silindi.")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Silme yapamazsınız.")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Kullanıcı Silindi");
        }
        #endregion

        #region Kullanıcı Güncelle
        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Kullanıcı Güncelleme ", Description = "Bu Endpointi Kullanıcıları güncellemeye Yarar.")]
        [SwaggerResponse(200, "Kullanıcı Güncellendi.")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Güncelleme yapamazsınız.")]
        public async Task<IActionResult> UpdateUser(string email, [FromBody] UpdateUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Kullanıcı Güncellendi.");
        }

        #endregion

        #region Kullanıcıyı Admin Yapma

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Kullanıcı Admin Yapma ", Description = "Bu Endpointi Kullanıcıları Admin yapmaya Yarar.")]
        [SwaggerResponse(200, "Kullanıcı Admin yapıldı.")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Admin Rolu atayamazsınız.")]
        public async Task<IActionResult> MakeAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            if (!await _roleManager.RoleExistsAsync("Admin")) // Belirtilen rolün veritabanında olup olmadığını kontrol eder.
                await _roleManager.CreateAsync(new IdentityRole("Admin")); // 	Yeni bir rol oluşturur.

            var result = await _userManager.AddToRoleAsync(user, "Admin"); // Kullanıcıya belirtilen rolü atar.

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("Kullanıcı Admin yapıldı.");
        } 

        #endregion


        #region Kullanıcıyı User Yapma

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Kullanıcıyı User Yapma ", Description = "Bu Endpointi Kullanıcıları ve adminleri User Rolu verir.")]
        [SwaggerResponse(200, "Kullanıcı User Role atandı.")]
        [SwaggerResponse(404  , "Kullanıcı Bulunamadı.")]
        [SwaggerResponse(401  , "Admin Değilsiniz. Rol atayamazsınız.")]
        public async Task<IActionResult> MakeUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // Admin rolünden çıkar
            if (await _userManager.IsInRoleAsync(user, "Admin")) // IsInRoleAsync: Kullanıcının belirli bir rolde olup olmadığını kontrol eder.
                await _userManager.RemoveFromRoleAsync(user, "Admin"); // RemoveFromRoleAsync: Kullanıcıdan bir rolü kaldırır.

            // User rolü yoksa oluştur
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            // User rolüne ekle
            var result = await _userManager.AddToRoleAsync(user, "User"); // AddToRoleAsync: Kullanıcıya rol atar.

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Kullanıcı artık sadece 'User' rolündedir.");
        }

        #endregion
        
        
        //RoleManager<IdentityRole> Sık Kullanılan Metotlar
            
        // UpdateAsync(IdentityRole role)	Rol üzerinde güncelleme yapar.
        // RoleExistsAsync(string roleName)	Belirtilen rolün veritabanında olup olmadığını kontrol eder.
        // CreateAsync(IdentityRole role)	Yeni bir rol oluşturur.
        //DeleteAsync(IdentityRole role)	Rolü veritabanından siler.
        //FindByNameAsync(string roleName)	Belirtilen isme sahip rolü getirir.
        //UpdateAsync(IdentityRole role)	Rol üzerinde güncelleme yapar.
        // RoleManager, sadece IdentityRole nesneleriyle çalışır.

        // UserManager<ApplicationUser> Sık Kullanılan Rol Metotları
           
        //ddToRoleAsync(user, roleName)	Kullanıcıya belirtilen rolü atar.
        //RemoveFromRoleAsync(user, roleName)	Kullanıcıdan belirtilen rolü kaldırır.
        //GetRolesAsync(user)	Kullanıcının sahip olduğu tüm rolleri getirir.
        //IsInRoleAsync(user, roleName)	Kullanıcının belirtilen rolde olup olmadığını kontrol eder.
           
        // Diğer Faydalı UserManager Metotları
           
        // FindByEmailAsync(string email)	E-postaya göre kullanıcıyı getirir.
        // FindByIdAsync(string id)	        ID’ye göre kullanıcıyı getirir.
        // CreateAsync(user, password)	   Yeni bir kullanıcı oluşturur.
        // DeleteAsync(user)	                Kullanıcıyı siler.
        // CheckPasswordAsync(user, password)	Şifreyi kontrol eder.
        // GenerateEmailConfirmationTokenAsync(user)	E-posta onay token'ı üretir.
        // GeneratePasswordResetTokenAsync(user)	Şifre sıfırlama token'ı üretir.
        
        
    }
}
