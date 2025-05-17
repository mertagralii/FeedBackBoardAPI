using AutoMapper;
using FeedBackBoardAPI.Data.Contex;
using FeedBackBoardAPI.Data.DTO.Register;
using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace FeedBackBoardAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Sadece Admin olanlar bu apiyi kullanabilir.
    public class UseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;

        public UseController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IWebHostEnvironment webHostEnvironment,AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
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
             var result =_mapper.Map<List<UserDto>>(userList);
            return Ok(result);
        }


        #endregion
        // Burası Kullanıcının Email adresine göre kullanıcı bilgisini getiriyor. 
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
            var result =_mapper.Map<UserDto>(user);
            return Ok(result);
        }
        #endregion
        // Burası Kullanıcının Silinmesi için gerçekleştirdiğimiz endpoint kısmı
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
        // Burası Kullanıcı Güncellenmesi gerçekleştirdiğimiz endpoint Kısmı
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
        // Burası Kullanıcıya Admin Rolu Sağladığımız endpoint Kısmı
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

        // Burası Kullanıcıya ve Adminlere User Rolu verdiğimiz Kısım.
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


       #region Kullanıcının Profil Resmini Güncelleme Kısmı (Yeni Öğrendim)
       
        [HttpPost("{userEmail}/[action]")]
        // Swagger veya benzeri araçlarda dosya yükleme formu görünmesi için gerekli attribute
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UpdateProfileImage(string userEmail, [FromForm] ImageUpdateUserDto model) // ImageFile kullanınca FromForm kullanmak zorundayız.
        {
            // 1. Kullanıcıyı bul
            
            // Gelen userEmail parametresi ile UserManager üzerinden veritabanından kullanıcıyı getiriyoruz.
            var user = await _userManager.FindByEmailAsync(userEmail);
            // Eğer kullanıcı bulunamadıysa NotFound (404) döndür.
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            // 2. Dosya olup olmadığını kontrol et
            
            // Gönderilen model içinde ImageFile (dosya) var mı, boş mu diye kontrol ediyoruz.
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                // Dosya yoksa BadRequest (400) ile kullanıcıya dosya seçmesini söylüyoruz.
                return BadRequest("Dosya seçilmedi.");
            }

            // 3. İzin verilen uzantılar
            
            // İzin verdiğimiz dosya türlerini liste olarak belirliyoruz.
            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
            // Gönderilen dosyanın uzantısını küçük harfe çevirerek alıyoruz.
            var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

            // Eğer dosya uzantısı izin verilenler arasında değilse hata dön.
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Sadece .jpg, .jpeg, .png ve .gif formatları kabul edilmektedir.");
            }

            // 4. Maksimum dosya boyutu (örneğin 2MB)
            
            // Dosya boyutu sınırı, burada 2 megabayt olarak belirlenmiş (byte cinsinden).
            long maxFileSize = 2 * 1024 * 1024; // 2MB

            // Eğer dosya boyutu sınırı aşıyorsa kullanıcıya uyarı ver.
            if (model.ImageFile.Length > maxFileSize)
            {
                return BadRequest("Dosya boyutu maksimum 2MB olabilir.");
            }

            // 5. Uploads klasör yolunu ayarla ve klasör yoksa oluştur
            
            // wwwroot altında uploads isimli klasörün tam yolunu alıyoruz.
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            // Eğer klasör yoksa bu satır klasörü oluşturuyor, böylece kaydetme sırasında hata olmaz.
            Directory.CreateDirectory(uploadsFolder);

            // 6. Dosya adını ve tam yolunu oluştur
            
            // Dosya adını benzersiz yapmak için GUID ile yeni bir isim oluşturuyoruz ve orijinal uzantıyı ekliyoruz.
            string fileName = Guid.NewGuid() + extension;
            // Klasör yolu ve dosya adı birleştirilerek tam dosya yolu elde ediliyor.
            string filePath = Path.Combine(uploadsFolder, fileName);

            // 7. Dosyayı kaydet
            
            // Dosyayı belirtilen yola yazmak için FileStream açıyoruz.
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                // Asenkron olarak dosya içeriğini kopyalıyoruz.
                await model.ImageFile.CopyToAsync(stream);
            }

            // 8. Kullanıcının profil fotoğrafı yolunu güncelle
            
            // Veritabanında kullanıcı objesinin Profil Resim yolunu uploads klasörüne göre güncelliyoruz.
            user.ProfileImagePath = "/uploads/" + fileName;
            // Güncellenen kullanıcıyı Entity Framework ile işaretliyoruz.
            _context.Users.Update(user);
            // Değişiklikleri veritabanına kaydediyoruz.
            await _context.SaveChangesAsync();

            // 9. Başarılı yanıt döndür
            
            // İşlem başarılı ise kullanıcıya mesaj ve yeni resim URL'sini JSON olarak döndürüyoruz.
            return Ok(new
            {
                message = "Resim başarıyla yüklendi",
                imageUrl = user.ProfileImagePath
            });
        }
        
        // NOT : ApplicationUser ve ImageUpdateUserDto incele. Program.cs'e de   app.UseStaticFiles(); ekledik.
        // wwwroot klasörünü açmak zorundasın
        // resimlerin de barınacağı kısmı açmak zorundasın

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
