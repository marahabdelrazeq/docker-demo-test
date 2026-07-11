using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Responses
{
    public class ApiResponseNotifications<T> : ApiResponse<T>
    {
        /// <summary>  
        /// unread notifications count  
        /// </summary>  
        public int UnreadNotificationsCount { get; set; } = 0;

        /// <summary>  
        /// Constructor for notification response  
        /// </summary>   
        public ApiResponseNotifications((IEnumerable<T>, int) data, int unreadNotificationsCount = 0, List<Error> errors = null)
            : base(data, errors)
        {
            Data = new PaginatedResponse<T>(data);
            Errors = errors;
            UnreadNotificationsCount = unreadNotificationsCount;
        }
    }
}
