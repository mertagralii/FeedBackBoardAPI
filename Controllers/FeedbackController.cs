using AutoMapper;
using FeedBackBoardAPI.Data.Contex;
using FeedBackBoardAPI.Data.DTO.Comment;
using FeedBackBoardAPI.Data.DTO.Feedback;
using FeedBackBoardAPI.Data.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Swashbuckle.AspNetCore.Annotations;

namespace FeedBackBoardAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(AppDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;    
        }

        #region Tüm Feedbackleri Getir ✓
        
        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Tüm Başlıkları Getirir.", Description = "Bu Endpointi çalıştırdığınızda tüm konu başlıklarını getirecektik.")]
        [SwaggerResponse(200, "Tüm Başlıklar Geldi")]
        [SwaggerResponse(404, "Başlık Bulunamadı.")]
        public async Task<ActionResult<FeedbackDto[]>> GetAllFeedback() // Tüm FeedBackleri getir. ✓
        { 
            var allFeedBack = await _context.Feedbacks
                .Include(x => x.Category)
                .Include(x => x.Status)
                .Include(z=> z.ApplicationUser)
                .ToArrayAsync();
                
            if (allFeedBack.Length ==0)
            { 
                return NotFound("No feedback found");
            }
            var resume = _mapper.Map<FeedbackDto[]>(allFeedBack);
            return Ok(resume);
        }


        #endregion
        
        #region Sadece Seçili Kategorilerin Feedbacklerini getir ✓

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Seçili Kategorileri FeedBack Getirir.", Description = "Bu Endpointi Sadece Seçili Kategorilerin FeedBacklerini Getir")]
        [SwaggerResponse(200, "Seçili Kategoriye ait FeedBack Bulundu.")]
        [SwaggerResponse(404, "Kategoriye ait Feedback bulunamadı.")]
        public async Task<ActionResult<FeedbackDto>> GetFeedbackByCategory(int id) // Sadece Seçili Kategorilerin FeedBacklerini Getir ✓
        {
            var allFeedBack = await _context.Feedbacks
                .Include(x => x.Category)
                .Include(x => x.Status)
                .Include(z=> z.ApplicationUser)
                .Where(x=> x.Category.Id == id)
                .ToArrayAsync();
            if (allFeedBack.Length == 0)
            {
                return NotFound("No feedback found");
            }
            var resume = _mapper.Map<FeedbackDto>(allFeedBack[0]);
            return Ok(resume);
        }

        #endregion
       

        #region  İstenilen FeedBack getir ✓

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Seçili FeedBack Getirir.", Description = "Bu Endpointi Sadece istenilen FeedBack'i getir.")]
        [SwaggerResponse(200, "Seçili FeedBack Bulundu.")]
        [SwaggerResponse(404, "Seçili Feedback bulunamadı.")]
        public async Task<ActionResult<DetailsFeetbackDto>> GetFeedbackDetailsAndComment(int id) // Sadece istenilen FeedBack'i getir. ✓
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Category)
                .Include(f => f.Status)
                .Include(f => f.ApplicationUser)
                // Sadece ParentCommentId == null yorumları include et
                .Include(f => f.Comments.Where(c => c.ParentCommentId == null))
                .ThenInclude(c => c.Replies) 
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
                return NotFound("No feedback found");

            return Ok(_mapper.Map<DetailsFeetbackDto>(feedback));
        }

        #endregion
        

        #region FeedBack Ekle Kısmı ✓

        [Authorize]
        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "FeedBack Ekle.", Description = "Bu Endpointi FeedBack eklemeye yarar.")]
        [SwaggerResponse(200, "FeedBack Eklendi.")]
        [SwaggerResponse(400 , "Formata uygun veri gönderilmedi. Geçersiz veri gönderildi.")]
        public ActionResult<AddFeedbackDto> AddFeedback([FromBody] AddFeedbackDto feedback) // FeedBack Ekle ✓
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = _userManager.GetUserId(User);
            var feedbackToAdd = _mapper.Map<Feedback>(feedback);
            feedbackToAdd.ApplicationUserId = userId;
            _context.Feedbacks.Add(feedbackToAdd);
            _context.SaveChanges();
            var resume = _mapper.Map<AddFeedbackDto>(feedback);
            return Ok(resume);
        }

        #endregion
       

        #region FeetBack Güncelleme ✓

        [Authorize]
        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Feetback Güncelle", Description = "Bu Endpointi FeedBack Güncellemeye yarar.")]
        [SwaggerResponse(200, "FeedBack Güncellendi.")]
        [SwaggerResponse(400 , "Formata uygun veri gönderilmedi. Geçersiz veri gönderildi.")]
        [SwaggerResponse(401  , "FeddBack sahibi siz değilsiniz. Güncelleme yapamazsınız.")]
        [SwaggerResponse(404  , "FeedBack Bulunamadı.")]
        public async Task<ActionResult<UpdateFeedbackDto>> UpdateFeedback([FromBody] UpdateFeedbackDto updateFeedbackDto) // Feetback Güncelle ✓
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = _userManager.GetUserId(User);
            var feedbackToUpdate = await _context.Feedbacks.
                Include(x=> x.Category)
                .Include(x=>x.Status).
                FirstOrDefaultAsync(x=> x.Id == updateFeedbackDto.Id);
            if (feedbackToUpdate.ApplicationUserId != userId)
            {
                return Unauthorized();
            }

            if (feedbackToUpdate == null)
            {
                return NotFound("No feedback found");
            } 
            var updateFeedback =  _mapper.Map(updateFeedbackDto, feedbackToUpdate);
            await _context.SaveChangesAsync();
            var resume = _mapper.Map<FeedbackDto>(feedbackToUpdate);
            return Ok(resume);
        }

        #endregion
      
        
        # region FeedBack Silme Kısmı ✓
        [Authorize]
        [HttpDelete("[action]")]
        [SwaggerOperation(Summary = "Feetback Sil", Description = "Bu Endpointi FeedBack Silmeye yarar.")]
        [SwaggerResponse(200, "FeedBack Silindi.")]
        [SwaggerResponse(401  , "FeddBack sahibi siz değilsiniz. Silme yapamazsınız.")]
        [SwaggerResponse(404  , "FeedBack Bulunamadı.")]
        public async Task<ActionResult<FeedbackDto>> DeleteFeedback(int id) // Feetback Sil ✓
        {
            var userId = _userManager.GetUserId(User);
            var feedbackToDelete = await _context.Feedbacks.FindAsync(id);
            if (feedbackToDelete == null)
            {
                return NotFound("No feedback found");
            }
            if (feedbackToDelete.ApplicationUserId != userId)
            {
                return Unauthorized();
            }
            _context.Feedbacks.Remove(feedbackToDelete);
            await _context.SaveChangesAsync();
            var resume = _mapper.Map<FeedbackDto>(feedbackToDelete);
            return Ok(resume);
        }
        #endregion

        #region  Yorum atma Kısmı ✓

        [Authorize]
        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Yorum at", Description = "Bu Endpointi FeedBack'lere yorum atmaya yarar.")]
        [SwaggerResponse(200, "Yorum Atıldı.")]
        [SwaggerResponse(404  , "Uygun formatta veri gönderilmedi. Geçersiz veri gönderildi.")]
        public ActionResult<AddCommentDto> AddComment([FromBody] AddCommentDto feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            var comment = _mapper.Map<Comment>(feedback);
            comment.ApplicationUserId = userId;
            comment.DateCreated = DateTime.Now;

            _context.Comments.Add(comment);
            _context.SaveChanges();

            var result = _mapper.Map<AddCommentDto>(comment);
            return Ok(result);
        }


        #endregion

        #region Yorum Güncelleme Kısmı ✓

        [Authorize]
        [HttpPut("[action]")] // Yorum Güncellemesi✓
        [SwaggerOperation(Summary = "Yorum Güncellemesi", Description = "Bu Endpointi Yorum Güncellemeye yarar.")]
        [SwaggerResponse(200, "Yorum Güncellendi.")]
        [SwaggerResponse(400 , "Formata uygun veri gönderilmedi. Geçersiz veri gönderildi.")]
        [SwaggerResponse(404  , "Yorum Bulunamadı.")]
        [SwaggerResponse(401  , "Yorum sahibi siz değilsiniz. Güncelleme yapamazsınız.")]
        public async Task<ActionResult<UpdateCommentDto>>UpdateCommentDto([FromBody] UpdateCommentDto feedbackUpdateCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = _userManager.GetUserId(User);
            
            var getComment = await _context.Comments.FindAsync(feedbackUpdateCommentDto.Id);
            if (getComment == null)
            {
                return NotFound("No feedback found");
            }

            if (getComment.ApplicationUserId != userId)
            {
                return Unauthorized();
            }
            getComment.Content = feedbackUpdateCommentDto.Content;
            _context.Comments.Update(getComment);
            await _context.SaveChangesAsync();
            return Ok("Updated Comment");
        }

        #endregion
       

        #region Yorum Silme Kısmı ✓

        [Authorize]
        [HttpDelete("[action]")] // Yorum Silme ✓
        [SwaggerOperation(Summary = "Yorum Sil", Description = "Bu Endpointi Yorum Silmeye yarar.")]
        [SwaggerResponse(200, "Yorum Silindi.")]
        [SwaggerResponse(404  , "Yorum Bulunamadı.")]
        public async Task<ActionResult<CommentDto>> DeleteComment(int id) // Bu Normal Yorumları Silmek için 
        {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                return NotFound();

            // Önce alt yorumları sil
            if (comment.Replies != null && comment.Replies.Any())
                _context.Comments.RemoveRange(comment.Replies);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion
        
        #region Feedback yoruma yorum atma kısmı  ✓
        [Authorize]
        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Yoruma alt yorum atma", Description = "Bu Endpointi FeedBack'lere yorumlara cevap vermeye yarar.")]
        [SwaggerResponse(200, "Yorum Atıldı.")]
        [SwaggerResponse(404  , "Parent comment not found.")]
        public ActionResult<CommentDto> CommentOnTheComment([FromBody] CommentOnTheCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);

            // Parent yorumu getir
            var parent = _context.Comments
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == dto.ParentCommentId);
            if (parent == null)
                return NotFound("Parent comment not found");

            // Yeni alt yorumu oluştur
            var reply = _mapper.Map<Comment>(dto);
            reply.ApplicationUserId = userId;
            reply.ParentCommentId   = dto.ParentCommentId;
            reply.FeedbackId        = parent.FeedbackId;  // Burada kritik: hangi feedback'e ait

            _context.Comments.Add(reply);
            _context.SaveChanges();

            // Eklenen yorumu DTO’ya çevirip dön
            var result = _mapper.Map<CommentDto>(reply);
            return Ok(result);
        }
        #endregion

        #region Yoruma Cevap Verme Güncelleme  ✓

        [Authorize]
        [HttpPut("[action]")] // Yoruma Cevap verme Güncelleme✓
        [SwaggerOperation(Summary = "Yorumların alt yorumlarını güncelleme ", Description = "Bu Endpointi Yorumların alt yorumlarını Güncellemeye yarar.")]
        [SwaggerResponse(200, "Alt yorum Güncellendi.")]
        [SwaggerResponse(400 , "Formata uygun veri gönderilmedi. Geçersiz veri gönderildi.")]
        [SwaggerResponse(404  , "Yorum Bulunamadı.")]
        [SwaggerResponse(401  , "Yorum sahibi siz değilsiniz. Güncelleme yapamazsınız.")]
        public async Task<ActionResult<UpdateCommentOnTheCommentDto>> UpdateCommentOnTheComment([FromBody] UpdateCommentOnTheCommentDto updateCommentOnTheCommentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            var comment = await _context.Comments.FindAsync(updateCommentOnTheCommentDto.Id);

            if (comment == null)
                return NotFound("Reply comment not found");

            if (comment.ApplicationUserId != userId)
                return Unauthorized("You can only edit your own comment.");

            // Sadece mesaj güncellenecek
            comment.Content = updateCommentOnTheCommentDto.Content;

            await _context.SaveChangesAsync();

            var updatedDto = _mapper.Map<CommentDto>(comment);
            return Ok(updatedDto);
        } 

        #endregion

        #region Yoruma Cevap Verme Silme ✓

        [Authorize]
        [HttpDelete("[action]")] // Yoruma cevap verme Silme ✓
        [SwaggerOperation(Summary = "Yorum'un alt yorumlarını Sil", Description = "Bu Endpointi Yorumların alt yorumlarını Silmeye yarar.")]
        [SwaggerResponse(200, "Yorum Silindi.")]
        [SwaggerResponse(404  , "Yorum Bulunamadı.")]
        [SwaggerResponse(401  , "Yorum sahibi siz değilsiniz. Silme yapamazsınız.")]
        [SwaggerResponse(400 , "This is not a reply comment.")]
        public async Task<ActionResult<CommentDto>> DeleteCommentOnTheComment(int id)
        {
            
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
                return NotFound("Reply comment not found");

            var userId = _userManager.GetUserId(User);
            if (comment.ApplicationUserId != userId)
                return Unauthorized("You can only delete your own comment.");

            // ParentCommentId dolu ise bu bir reply’dir
            if (comment.ParentCommentId == null)
                return BadRequest("This is not a reply comment.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Reply comment deleted.");
        }  

        #endregion
      
       
    }
}
