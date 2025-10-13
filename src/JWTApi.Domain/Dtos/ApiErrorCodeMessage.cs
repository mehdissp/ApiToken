using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
    public static class ApiErrorCodeMessage
    {


        //----------------------------------------------------------------------------------------------------

 

        /// <summary>
        /// 4484 -  ویژگی درخواست و مدیریت سفته از مشتریان برای پذیرنده فعال نیست.
        /// </summary>
        public static ErrorCodeDto Error_Refrence = ErrorCodeDto.Create(4485, "این رکورد داری رفرنس می باشد ");


    }
}
