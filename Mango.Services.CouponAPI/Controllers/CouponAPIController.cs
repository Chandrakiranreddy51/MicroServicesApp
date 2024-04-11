using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext context;
        private IMapper mapper;
        private ResponseDto response;

        public CouponAPIController(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = context.Coupons.ToList();
                response.Result = mapper.Map<IEnumerable<CouponDto>>(coupons);
                //response.Result = coupons;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var coupon = context.Coupons.First(x => x.CouponId == id);

                response.Result = mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                var coupon = context.Coupons.First(x => x.CouponCode.ToLower() == code.ToLower());

                response.Result = mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto req)
        {
            try
            {
                var obj = mapper.Map<Coupon>(req);
                context.Coupons.Add(obj);
                context.SaveChanges();

                response.Result = mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto req)
        {
            try
            {
                var obj = mapper.Map<Coupon>(req);
                context.Coupons.Update(obj);
                context.SaveChanges();

                response.Result = mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                var obj = context.Coupons.First(x => x.CouponId == id);

                context.Coupons.Remove(obj);
                context.SaveChanges();

                response.Result = mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
