using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface IVendorReviewService
    {
        VendorReviews Add(VendorReviews vr);
        List<VendorReviews> GetAll(int targetUserId);
    }
    public class VendorReviewService : IVendorReviewService
    {
        private DataContext _context;
        public VendorReviewService(DataContext context)
        {
            _context = context;
        }
        public VendorReviews Add(VendorReviews vr)
        {
            var vendorReview = _context.VendorReviews.SingleOrDefault(x => x.TargetUserId == vr.TargetUserId && x.UserId == vr.UserId);
            if (vendorReview == null)
            {
                VendorReviews newVR = new VendorReviews();
                newVR.TargetUserId = vr.TargetUserId;
                newVR.UserId = vr.UserId;
                newVR.Mark = vr.Mark;
                newVR.Message = vr.Message;
                _context.VendorReviews.Add(newVR);
                _context.SaveChanges();
                return newVR;
            }
            else
            {
                throw new AppException("This user has already commented target user");
            }
        }
        public List<VendorReviews> GetAll(int targetUserId) {
            return _context.VendorReviews.Where(x => x.TargetUserId == targetUserId).ToList();
        }
    }
}
